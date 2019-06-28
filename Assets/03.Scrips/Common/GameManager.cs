using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Create Info")]
    //적 캐릭터가 출현할 위치 담은 배열
    public Transform[] points;
    //적 캐릭터 프리팹을 저장
    public GameObject[] enemy;
    //적 캐릭터 생성 시간
    public float createTime = 1f;
    //적 캐릭터의 최대 생성
    public int maxEnemy = 50;
    //게임 종교 여부 판단
    public bool isGameOver = false;

    //싱글턴에 접근하기 위한 Static선언
    public static GameManager instance;

    [Header("Object Pool")]
    //생성할 총알 프리팹
    //public GameObject bullet;
    //public GameObject shotbullet;
   //오브젝트 풀안에 생성갯수
    public int maxPool = 30;
    public List<GameObject> bulletPool = new List<GameObject>();

    //일시 정지 여부
    bool isPaused;

    bool isMouse;

    public CanvasGroup inventoryCG;

    //주인공이 죽인 적 캐릭터 수
    [HideInInspector] public int killCount;
    //적 캐릭터를 죽인 횟수를 표시함
    public Text killCountTxt;

    public GameObject[] doors;
    public int index = 0;

    public GameObject pDamage;
    public GameObject sceneChage;
    public GameObject menu;

    public bool isOver = false;

    public GameObject kill;
    public GameObject next;

    //일시정지 버튼 클릭시 호출
    public void OnPauseClick()
    {
        //일시 정지 값을 토글시킴
        isPaused = !isPaused;
        //Time Scale이 0이면 정지, 1이면 정상
        Time.timeScale = (isPaused) ? 0f : 1f;
        //주인공 객체를 추출
        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        //주인공 캐릭터에 추가된 모든 스크립트
        var scripts = playerObj.GetComponents<MonoBehaviour>();
        //주인공 캐릭터의 모든 스크립트를 활성/비활성
        foreach(var script in scripts)
        {
            script.enabled = !isPaused;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //instance가 할당된 클래스의 인스턴스가 다를 경우 새로 생성한다
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }        
        //오브젝트 풀링 생성함수
        //CeatePooling();
    }
    
    void GameStart(GameObject[] Door, int i)
    {
        for (i = 0; i < Door.Length; i++)
        {            
            Destroy(Door[i], 6);
        }
    }

    //적 캐릭터가 죽을 때마다 호출
    public void IncKillCount()
    {
        ++killCount;
        killCountTxt.text = "KILL" + killCount.ToString("00");
        //죽인 횟수
        PlayerPrefs.SetInt("KILL_COUNT", killCount);
        BosSence();
    }


    //보스로 넘아가는 신
    public void BosSence()
    {
        if (killCount >= 30)
        {
            sceneChage.SetActive(true);
            //SceneManager.LoadScene("Loading");
            kill.SetActive(false);
            next.SetActive(true);
        }
    }

    //엔딩신
    public void EndingScene()
    {
        sceneChage.SetActive(true);
    }



    public void YouDead()
    {        
         SceneManager.LoadScene("Start");
    }   

    public void ReStart()
    {        
         SceneManager.LoadScene("Loading");
    }

    void Start()
    {
        next.gameObject.SetActive(false);
        OnInventoryOpen(false);
        //스폰 위치를 찾음
        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();        
        if (points.Length > 0)
        {
            StartCoroutine(this.CreateEnemy());
        }

        doors = GameObject.FindGameObjectsWithTag("DOOR");
        if (doors.Length >= 2)
        {
            GameStart(doors, 8);
        }

        menu.SetActive(false);
        sceneChage = GameObject.FindGameObjectWithTag("Scene");
        sceneChage.SetActive(false);

        MounsLock();
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {            
            //Application.Quit();
        }
        */
        Menu();
    }   

    public void Menu()
    {        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnPauseClick();
            MouseNone();
            menu.SetActive(true);
            
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Cancel();
        }
    }
    
    public void End()
    {
        Application.Quit();
    }

    public void Cancel()
    {
        OnPauseClick();
        menu.SetActive(false);
        MounsLock();
    }

    public void OnmouseLock()
    {
        //일시 정지 값을 토글시킴
        isMouse = !isMouse;
        //Time Scale이 0이면 정지, 1이면 정상
        Time.timeScale = (isMouse) ? 0f : 1f;
        //주인공 객체를 추출
        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        //주인공 캐릭터에 추가된 모든 스크립트
        var scripts = playerObj.GetComponents<MonoBehaviour>();
        //주인공 캐릭터의 모든 스크립트를 활성/비활성
        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }
    }

    //마우스 숨기기
    void MounsLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //마우스 보이기
    public void MouseNone()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //인벤토리를 활성화/비활성화
    public void OnInventoryOpen(bool isOpened)
    {
        inventoryCG.alpha = (isOpened) ? 1.0f : 0.0f;
        inventoryCG.interactable = isOpened;
        inventoryCG.blocksRaycasts = isOpened;
    }

    IEnumerator CreateEnemy()
    {
        while (!isGameOver)
        {
            //게임 종료까지 무한루프
            int enemyCount = (int)GameObject.FindGameObjectsWithTag("ENEMY").Length;
            //적 캐릭터의 최대 생성 개수보다 작을 때만 적 캐릭터 생성
            if (enemyCount < maxEnemy)
            {
                //적 캐릭터의 생성 주기 시간
                yield return new WaitForSeconds(createTime);

                //불규칙적인 위치 산출
                int idx = Random.Range(1, points.Length);
                //불규칙적인 enemy 산출
                int idxe = Random.Range(1, enemy.Length);
                //적 캐릭터의 동적
                Instantiate(enemy[idxe], points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }
       

    //오브젝트 풀에서 사용 가능한 총알
    public GameObject GetBullet()
    {
        for(int i = 0; i < bulletPool.Count; i++)
        {
            //비활성화 여부로 사용 가능한 총알인지 판단
            if (bulletPool[i].activeSelf == false)
            {
                return bulletPool[i];
            }
        }
        return null;
    }
    
    //오브젝트 풀에 총알을 생성
   /* public void CeatePooling()
    {
        //총알을 생성해 차일드화할 페어런트 생성
        GameObject objectPools = new GameObject("ObjectPools");

        //풀림 개수만큼 미리 총알 생성
        for(int i = 0; i < maxPool; i++)
        {
            var obj = Instantiate<GameObject>(bullet, objectPools.transform);
            obj.name = "Bullet_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 생성할 총알
            bulletPool.Add(obj);
        }
    }*/

}

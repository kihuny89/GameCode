using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Damage : MonoBehaviour
{
    const string AttackTag = "PUNCH";
    float initHp = 100;
    public float currHp;
    //BloodScreen 텍스처
    public Image bloodScreen;

    Transform enemyTr;

    //HpbarImage
    public Image hpBar;
    //생명게이지 녹색
    readonly Color initColor = new Vector4(0f, 1f, 0f, 1f);
    Color currColor;

    Shake shake;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    public GameObject lost;

    public GameManager manager;
    FireCtrl fire;

    void Start()
    {            
        currHp = initHp;
        //생명게이지 초기 색상
        hpBar.color = initColor;
        currColor = initColor;
        lost.gameObject.SetActive(false);
        shake = GetComponentInChildren<Shake>();
    }

    private void OnCollisionEnter(Collision other)
    {
        //충돌한 Collider의 태그가 BULLET이면 Player의 currHp차감
        if (other.gameObject.tag == AttackTag)
        {
            //혈흔 효과
            StartCoroutine(ShowBloodScreen());
            currHp -= 5f;
            print(currHp.ToString());
            GetComponent<PlayerCtrl>().HitDamage();
            GetComponent<FireCtrl>().HitDam();
            DisplayHpBar();
            shake.StartCoroutine(shake.ShakeCamera());

            //Player의 생명이 0이하면 Die처리
            if (currHp <= 0)
            {
                PlayerDie();                
                currHp = 0;
                lost.gameObject.SetActive(true);
                manager.MouseNone();
                
            }            
        }
       
    }




    IEnumerator ShowBloodScreen()
    {
        //BloodScreen 텍스쳐의 알파값을 불규칙하게 변경
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        //BloodScreen 텍스쳐의 색상을 모두 0
        bloodScreen.color = Color.clear;
    }

    //Player의 사망루틴
    void PlayerDie()
    {
        OnPlayerDie();
        GameManager.instance.isGameOver = true;
    }

    void DisplayHpBar()
    {
        //생명 수치가 50%일 때까지는 녹색에서 노란색으로
        if ((currHp / initHp) > 0.5f)
            currColor.r = (1 - (currHp / initHp)) * 2f;
        //생명 수치가 0%일때까지는 노란색
        else
        currColor.g = (currHp / initHp) * 2f;
        //색상변경
        hpBar.color = currColor;
        //크기 변경
        hpBar.fillAmount = (currHp / initHp);
        
    }
}

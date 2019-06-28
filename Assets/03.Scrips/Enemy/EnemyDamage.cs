using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    const string attackTag = "BULLET";
    //생명게이지
    float hp = 100;
    //초기 생명
    float initHp = 100;
    //피격시 사용할 혈흔 효과
    GameObject bloodEffect;

    //생명게이지 프리팹
    public GameObject hpBarprefab;
    //새명 게이지의 위치를 보저할 오프셋
    public Vector3 hpBarOffset = new Vector3(0, 2.2f, 0);
    //부모가 될 Canvas객체
    Canvas uiCanvas;
    //생명 수치에 따라 fillAmount 속성을 변경
    Image hpBarImage;

    public static EnemyDamage instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        //혈흔 효과 프리팹 로드
        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");
        //생명게이지 생성 및 초기화
        //SetHpBAr();
    }

    void SetHpBAr()
    {
        uiCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        //ui canvas 하위로 생명 게이지
        GameObject hpBar = Instantiate<GameObject>(hpBarprefab, uiCanvas.transform);
        //fillAmount 속성을 변경할 Image를 추출
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];
        
        //생명 게이지가 따라가야 할 대상과 오프셋 값 설정
        var _hpBar = hpBar.GetComponent<EnemyHpBar>();
        _hpBar.targetTr = this.gameObject.transform;
        _hpBar.offset = hpBarOffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == attackTag)
        {
            //혈흔효과 생성하는 함수
            ShowBloodEffect(collision);
            //총알삭제
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);

            //생명게이지 차감
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
            //생명게이지의fillAmount 속성을 변경
           // hpBarImage.fillAmount = hp / initHp;
            GetComponent<EnemyAI>().DamageA();

            if (hp <= 0f)
            {
                //적 캐릭터의 상태를 Die로 만듬
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                //적 캐릭터가 사망한 이후 생명 게이지를 투명 처리
               // hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;
                //적 캐릭터의 사망 횟수룰 누적
                GameManager.instance.IncKillCount();
                //Capsule Collider 컴포넌트
                GetComponent<CapsuleCollider>().enabled = false;
            }
        }
    }

    void ShowBloodEffect(Collision coll)
    {
        //공격 지점
        Vector3 pos = coll.contacts[0].point;
        //공격이 성공 했을때
        Vector3 _normal = coll.contacts[0].normal;
        //공격 성공시 회전값
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

        //혈흔효과 
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    //AudioSource 컴포넌트를 저장
    AudioSource audio;
    //Animation 컴포넌트를 저장
    Animator animator;
    //주인공 캐릭터의 transform 컴포넌트
    Transform playerTr;
    //적 캐릭터의 Transform 컴포넌트
    Transform enemyTr;


    //애니메이터 컨트롤러에 정의한 파라미터의 해시값을 미리 추출
    readonly int hashAttack = Animator.StringToHash("Attack");

    //다음 공격 시간
    float nextAttack = 0f;
    //공격 간격
    readonly float attackRate = 2.5f;
    //주인공을 향해 회전할 속도
    readonly float damping = 10f;

    //공격 여부 판단
    public bool isAttack = false;
    

    void Start()
    {
        //컴포넌트 추출 및 변수
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttack)
        {
            //현재 시간이 다음 공격 시간보다 큰지 확인
            if (Time.time >= nextAttack)
            {
                Attact();
                //다음 공격시간
                nextAttack = Time.time + attackRate + Random.Range(0f, 0f);
            }
            //주인공이 있는 위치까지의 회전 각도
            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            //보간함수를 이용해 점진적으로 회전
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }

    void Attact()
    {
        animator.SetTrigger(hashAttack);
        GetComponent<EnemySound>().AttackMonster();
    }
}

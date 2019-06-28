using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //적 캐릭터의 상태를 표현하기 위한 열거형 변수
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DAMAGE,
        DIE
    }

    //상태를 저장할 변수
    public State state = State.PATROL;
    //주인공의 위치 저장
    Transform playerTr;
    //적 캐릭터의 위치 저장
    Transform enemyTr;
    //Animator 컴포넌트를 저장
    Animator animator;
    //사운드 저장
    EnemySound enemySound;

    //공격 사정거리
    public float attackDist = 2.2f;
    //추적 사정거리
    public float traceDist = 10f;
    //사망 여부
    public bool isDie = false;
    //추적 소리 간격
    public float traceSound;
    //소리 간격
    public float trSound = 3.5f;

    //코루틴에서 사용할 지연시간
    WaitForSeconds ws;
    //이동을 제어하는 MoveAgent 클래스를 저장할 변수
    MoveAgent moveAgent;

    //애니메이터 컨트롤러에 정의한 파라미터의 해시값을 미리 추출
    readonly int hashMove = Animator.StringToHash("IsMove");
    readonly int hashSpeed = Animator.StringToHash("Speed");
    readonly int hashDie = Animator.StringToHash("Die");
    readonly int hashOffset = Animator.StringToHash("Offset");
    readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    readonly int shader = Animator.StringToHash("Dead");
    readonly int damage = Animator.StringToHash("Damage");


    PlayerCtrl player;
    //공격 제어하는 EnemyAttack 클래스
    EnemyAttack enemyAttack;
       

    void Awake()
    {
        //주인공 게임오브젝트 추출
        var player = GameObject.FindGameObjectWithTag("PLAYER");
        //주인공의 Tranform 컴포넌트 추출
        if (player != null)
            playerTr = player.gameObject.GetComponent<Transform>();
        
        //적 캐릭터의 Tranform 컴포넌트 추출
        enemyTr = GetComponent<Transform>();
        //Animator 컴포넌트 추출
        animator = GetComponent<Animator>();
        //이동을 제어하는 MoveAgent 클래스를 추출
        moveAgent = GetComponent<MoveAgent>();
        //공격을 제어하는 EnemyAttack클래스 추출
        enemyAttack = GetComponent<EnemyAttack>();
        enemySound = GetComponent<EnemySound>();
        //코루틴의 지연시간 생성
        ws = new WaitForSeconds(0.3f);

        //Cycle Offset 값을 불규칙하게 변경
        animator.SetFloat(hashOffset, Random.Range(0f, 1f));
        animator.SetFloat(hashWalkSpeed, Random.Range(1f, 1.2f));
   
    }

    //델리게이트
    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        Damage.OnPlayerDie += this.OnPlayerDie;
    }

    private void OnDisable()
    {
        Damage.OnPlayerDie -= this.OnPlayerDie;
    }

    //플레이어가 사망시 몬스터 올 스톱
    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyAttack.isAttack = false;
        //모든 코루틴함수 종료
        StopAllCoroutines();
        animator.SetBool(hashMove, false);
    }



    IEnumerator CheckState()
    {
        while (!isDie)
        {
            //적 캐릭터가 사망하기 전까지 도는 무한루프
            if (state == State.DIE) yield break;
            //주인공과 적 캐릭터 간의 거리를 계산
            float dist = Vector3.Distance(playerTr.position, enemyTr.position);
            //공격 사정거리 이내
            if (dist <= attackDist)
            {
                state = State.ATTACK;

            }
            //추적 사정거리 이내
            else if (dist <= traceDist)
            {
                state = State.TRACE;
                if(Time.deltaTime >= traceSound)
                {
                    enemySound.WalkMonster();
                    traceSound = Time.time + trSound + Random.Range(0, 3f);                    
                }
            }

            else
            {
                state = State.PATROL;                
            }
            //0.3초 동안 대기하는 동안 제어권을 양보
            yield return ws;
        }
    }

    IEnumerator Action()
    {
        //적 캐릭터가 사망할 때까지 무한루프
        while (!isDie)
        {
            yield return ws;
            switch (state)
            {
                case State.PATROL:
                    GetComponentInChildren<SphereCollider>().enabled = false;
                    //공격정지
                    enemyAttack.isAttack = false;
                    //순찰 모드를 활성화
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                        GetComponentInChildren<SphereCollider>().enabled = false;
                        //공격정지
                        enemyAttack.isAttack = false;
                        //주인공의 위치를 넘겨 추적 모드로 변경
                        moveAgent.traceTarget = playerTr.position;
                        animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    GetComponentInChildren<SphereCollider>().enabled = true;
                    animator.SetBool(hashMove, false);
                    //공격시작
                    if (enemyAttack.isAttack == false)
                        enemyAttack.isAttack = true;
                    //순찰 및 추적을 정지
                    moveAgent.Stop();
                    break;
                case State.DAMAGE:
                    animator.SetTrigger(damage);
                    //순찰 및 추적을 정지
                    moveAgent.Stop();
                    break;
                case State.DIE:
                    isDie = true;
                    enemyAttack.isAttack = false;
                    //순찰 및 추적을 정지
                    moveAgent.Stop();
                    //사망 애니메이션 실행
                    animator.SetTrigger(hashDie);
                    animator.SetTrigger(shader);
                    //cllider 비활성화
                    GetComponentInChildren<SphereCollider>().enabled = false;
                    GetComponent<CapsuleCollider>().enabled = false;
                    enemySound.DieMonster();
                    break;
            }
        }
    }

    public void DamageA()
    {
        animator.SetTrigger(damage);
        //순찰 및 추적을 정지
        moveAgent.Stop();
    }


 

    // Update is called once per frame
    void Update()
    {      
        //Speed 파라미터에 이동 속도 전달
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }
}

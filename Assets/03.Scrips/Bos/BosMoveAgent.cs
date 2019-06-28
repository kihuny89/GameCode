using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BosMoveAgent : MonoBehaviour
{    
    //순찰 지점들을 저장하기 위한 List 타입 변수
    public List<Transform> wayPoints;
    //다음 순찰지점의 배열 Index
    public int nextIdx;

    readonly float patrolSpeed = 1.5f;
    readonly float traceSpeed = 15f;
    //회전할 때의 속도를 조절
    float damping = 1f;

    //NavMeshAgent 컴포넌트를 저장할 변수
    NavMeshAgent bosagent;
    //적 캐릭터의 Transfrom 컴포넌트를 저장할 변수
    Transform enemyTr;

    //순찰 여부확인 
    bool _patrolling;
    //patrolling 프로퍼티 정의
    public bool patrolling
    {
        get { return _patrolling; }
        set
        {
            _patrolling = value;
            if (_patrolling)
            {
                bosagent.speed = patrolSpeed;
                //순찰 상태의 회전계수
                damping = 1f;
                MoveWayPoint();
            }
        }
    }

    //추적 대상의 위치를 저장
    Vector3 _traceTarget;
    //_traceTarget 프로퍼티
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            bosagent.speed = traceSpeed;
            //순찰 상태의 회전계수
            damping = 7f;
            TraceTarget(_traceTarget);
        }
    }

    //NavMeshAgent의 이동 속도에 대한 프로퍼티
    public float speed
    {
        get { return bosagent.velocity.magnitude; }
    }

    void Start()
    {
        //적 캐릭터의 Transform 컴포넌트 추출 후 저장
        enemyTr = GetComponent<Transform>();
        //NavMeshAgent 컴포넌트를 추출한 후 변수 저장
        bosagent = GetComponent<NavMeshAgent>();
        //목적지에 가까워 질수록 속도를 줄이는 옵션을 비활성화
        bosagent.autoBraking = false;
        //자동으로 회전하는 기능
        bosagent.updateRotation = false;
        bosagent.speed = patrolSpeed;
        //하이러키 뷰의 WayPointGroup 게임오브젝트를 추출
        var group = GameObject.Find("WayPointBos");
        if (group != null)
        {
            //wayPointGroup 하위에 있는 모든 Transform 컴포넌트를 추출한 후 List 타입의WayPoints 배열에 추가
            group.GetComponentsInChildren<Transform>(wayPoints);
            //배열의 첫 번째 항목 삭제
            wayPoints.RemoveAt(0);

            //첫번째로 이동할 위치를 랜덤으로 변경
            nextIdx = Random.Range(0, wayPoints.Count);
        }
        // MoveWayPoint();
        this.patrolling = true;
    }

    void MoveWayPoint()
    {
        //최단거리 경로 계산이 끝나지 않았으면 다음을 수행하지 않음
        if (bosagent.isPathStale) return;

        //다음 목적지를 WayPoints 배열에서 추출한 위치로 다음 목적지를 지정
        bosagent.destination = wayPoints[nextIdx].position;
        //내비게이션 기능을 활서화해서 이동을 시작함
        bosagent.isStopped = false;

    }

    //주인공을 추적할 때 이동시키는 함수
    void TraceTarget(Vector3 pos)
    {
        if (bosagent.isPathStale) return;
        bosagent.destination = pos;
        bosagent.isStopped = false;
    }

    //순찰 및 추적을 정지시키는 함수
    public void Stop()
    {
        bosagent.isStopped = true;
        //바로 정지하기 위해 속도를 0으로 설정
        bosagent.velocity = Vector3.zero;
        _patrolling = false;
    }

    // Update is called once per frame
    void Update()
    {
        //적 캐릭터가 이동중일 때만 회전
        if (bosagent.isStopped == false)
        {
            //NavMeshAgent 가 가야 할 방향 벡터를 퀘터니언 타입의 각도
            Quaternion rot = Quaternion.LookRotation(bosagent.desiredVelocity);
            //보간 함수를 사용해 점진적으로 회전
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
        //순찰 모드가 아니면 정지
        if (!_patrolling) return;
        //NavMeshAgent가 이동하고 있는 목적지에 도착했는지 여부를 계산
        if (bosagent.velocity.sqrMagnitude >= 0.2f * 0.2f && bosagent.remainingDistance <= 0.5f)
        {
            //다음 목적지의 배열 첨자를 계산
            nextIdx = ++nextIdx % wayPoints.Count;
            //다음 목적지로 이동명령
            MoveWayPoint();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
public enum AnimalType
{
    Small,
    Medium,
    Large
}

public enum AnimalState
{
    Idle,
    FreeWalk,
    FollowPlayer,
    LeashFollow,
    Eat
}

public class AnimalLogic : MonoBehaviour
{
    // NavMeshAgent를 통해 길을 찾아 걸어다닌다.
    private NavMeshAgent nav;
    // 현재 반려동물의 애니메이션 표시
    private Animator anim;
    // 현재 반려동물의 상태 표시
    public AnimalState currentState = AnimalState.Idle;
    public AnimalType type = AnimalType.Small;
    // Player의 위치를 이용하여 반려동물이 돌아오게 하도록 조정
    public Transform player;

    [Header("Free Walk Delay")]
    public float walkRadius = 5f; // 걸어다니는 범위
    public float checkInterval = 5f; // 랜덤 위치 선정 시간
    private float walkTimer = 0f;

    [Header("Follow 설정")]
    public float callDistance = 1.5f; // 반려동물을 불렀을 경우 어느 범위까지만 오게 할 것인지에 대한 범위

    [Header("Leash 설정")]
    [SerializeField] private bool isLeashed;
    public float leashFollowDistance = 2f;

    [Header("Fetch System(공 잡아 오는 시스템)")]
    public Transform mouthPos;
    private GameObject targetBall;
    private bool isFetching = false;
    private bool hasBall = false;

    //Dictionary를 통해 변경 상태를 관리
    private Dictionary<AnimalState, Action> UpdateState;
    private Dictionary<AnimalState, Action> EnterState;

    //행동 시간
    private float behaviourTimer = 0f;
    private float behaviourIntervalTime = 2f;

    //목줄 상태인 경우 시간
    private float leashWalkTimer = 0f;
    private float leashWalkInterval = 3f;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        ChangeState(AnimalState.Idle);
        InitializeState();
        isLeashed = false;
    }
    private void InitializeState()
    {
        UpdateState = new Dictionary<AnimalState, Action>
        {
            {AnimalState.Idle, UpdateIdle},
            {AnimalState.FreeWalk, UpdateWalk },
            {AnimalState.FollowPlayer, UpdateFollow },
            {AnimalState.LeashFollow, UpdateLeashFollow },
            {AnimalState.Eat,UpdateEat }
        };

        EnterState = new Dictionary<AnimalState, Action>
        {
            {AnimalState.Idle,EnterIdle },
            {AnimalState.FreeWalk, EnterWalk},
            {AnimalState.FollowPlayer,EnterFollowPlayer },
            {AnimalState.LeashFollow, EnterLeashFollow },
            {AnimalState.Eat, EnterEat }
        };
    }

    void Update()
    {
        UpdateMovement();

        GetBall();

        if (isFetching && !hasBall && targetBall != null)
        {
            nav.SetDestination(targetBall.transform.position);
        }
    }

    public void OnBallSpawned(GameObject ball)
    {
        targetBall = ball;
        isFetching = true;
        hasBall = false;

        nav.isStopped = false;
        nav.SetDestination(ball.transform.position);
        SetAnimation("Walk");
    }
    void UpdateMovement()
    {
        if (isLeashed && currentState != AnimalState.LeashFollow)
        {
            ChangeState(AnimalState.LeashFollow);
        }
        else if (!isLeashed && currentState == AnimalState.LeashFollow)
        {
            ChangeState(AnimalState.Idle);
        }

        if (!isLeashed)
        {
            behaviourTimer += Time.deltaTime;

            if (behaviourTimer >= behaviourIntervalTime)
            {
                bool isArrived = !nav.pathPending && nav.remainingDistance <= 0.3f;

                if (currentState == AnimalState.FreeWalk && isArrived)
                {
                    ChangeState(AnimalState.Idle);
                    behaviourTimer = 0f;
                    behaviourIntervalTime = UnityEngine.Random.Range(1f, 3f);
                }
                else if (currentState == AnimalState.Idle)
                {
                    ChangeState(AnimalState.FreeWalk);
                    behaviourTimer = 0f;
                    behaviourIntervalTime = UnityEngine.Random.Range(1f, 3f);
                }
            }
        }
        UpdateState[currentState]?.Invoke();
    }

    void GetBall()
    {
        if (isFetching)
        {
            if(!hasBall && targetBall != null)
            {
                float dist = Vector3.Distance(transform.position, targetBall.transform.position);
                if(dist < 1f)
                {
                    targetBall.transform.SetParent(mouthPos);
                    targetBall.transform.localPosition = Vector3.zero;
                    targetBall.transform.localRotation = Quaternion.identity;

                    hasBall = true;
                    nav.SetDestination(player.position);
                    SetAnimation("Walk");
                }
            }
            else if (hasBall)
            {
                float distToPlayer = Vector3.Distance(transform.position, player.position);
                if(distToPlayer < 1f)
                {
                    Destroy(targetBall);
                    targetBall = null;
                    isFetching = false;
                    hasBall = false;

                    ChangeState(AnimalState.Idle);
                    SetAnimation("Idle");
                }
            }
        }
    }
    public void ChangeState(AnimalState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        EnterState[newState]?.Invoke();
    }

    private void UpdateIdle()
    {

    }

    private void UpdateWalk()
    {
        walkTimer += Time.deltaTime;
        if(!nav.pathPending && nav.remainingDistance <= 0.3f && walkTimer >=checkInterval)
        {
            MoveRandomPoint();
        }
    }

    private void UpdateFollow()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > callDistance)
        {
            nav.SetDestination(player.position);
        }
        else
        {
            ChangeState(AnimalState.Idle);
        }
    }

    private void UpdateLeashFollow()
    {
        leashWalkTimer += Time.deltaTime;
        float currentDistance = Vector3.Distance(transform.position, player.position);
        if(currentDistance >= leashFollowDistance)
        {
            nav.SetDestination(player.position);
            return;
        }
        
        if(!nav.pathPending && nav.remainingDistance <=0.3f && leashWalkTimer >= leashWalkInterval)
        {
            MoveRandomPointInLeashArea();
            leashWalkTimer = 0f;
            leashWalkInterval = UnityEngine.Random.Range(1f, 2f);
        }
    }

    private void UpdateEat()
    {

    }

    private void EnterIdle()
    {
        nav.isStopped = true;
        nav.ResetPath();
        SetAnimation("Idle");
    }

    private void EnterWalk()
    {
        nav.isStopped = false;
        walkTimer = 0f;
        MoveRandomPoint();
        SetAnimation("Walk");
    }
    private void EnterFollowPlayer()
    {
        nav.isStopped = false;
        SetAnimation("Walk");
    }
    private void EnterLeashFollow()
    {
        nav.isStopped = false;
        leashWalkTimer = 0f;
        leashWalkInterval = UnityEngine.Random.Range(1f, 2f);
        MoveRandomPointInLeashArea();
        SetAnimation("Walk");
    }
    private void EnterEat()
    {
        nav.isStopped = true;
        nav.ResetPath();
        SetAnimation("Eat");
    }

    private void SetAnimation(string str)
    {
        if (anim == null) return;

        anim.ResetTrigger("Idle");
        anim.ResetTrigger("Walk");
        anim.ResetTrigger("Eat");

        anim.SetTrigger(str);
    }

    private void MoveRandomPoint()
    {
        int angle = UnityEngine.Random.Range(0, 360);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = Mathf.Sin(angle * Mathf.Deg2Rad);
        Vector3 dir = new Vector3(x, 0f, z);
        Vector3 targetPos =  dir * walkRadius + transform.position;

        if (isLeashed)
        {
            Vector3 leashCenter = player.position;
            Vector3 offset = targetPos - leashCenter;

            if (offset.magnitude > leashFollowDistance)
            {
                offset = offset.normalized * leashFollowDistance * 0.9f;
                targetPos = leashCenter + offset;
            }
        }


        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
        {
            if((hit.position - transform.position).magnitude < 1f)
            {
                return;
            }

            Vector3 lookDir = (hit.position - player.position).normalized;
            lookDir.y = 0f;
            if(lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
            nav.SetDestination(hit.position);
        }

        else
        {
            nav.ResetPath();
        }
    }

    private void MoveRandomPointInLeashArea()
    {
        int angle = UnityEngine.Random.Range(0, 360);
        float radius = UnityEngine.Random.Range(leashFollowDistance * 0.5f, leashFollowDistance);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

        Vector3 targetPos = player.position + new Vector3(x, 0f, z);

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, leashFollowDistance, NavMesh.AllAreas))
        {
            float dis = Vector3.Distance(hit.position, player.position);
            if (dis > leashFollowDistance) return;

            Vector3 lookDir = (hit.position - player.position).normalized;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
            nav.SetDestination(hit.position);
        }
    }
}

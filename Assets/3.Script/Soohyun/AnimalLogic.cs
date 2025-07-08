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
} // Animal 타입에 따른 Prefab 설정을 따로 해야함.

public enum AnimalState
{
    Idle,
    FreeWalk,
    FollowPlayer,
    LeashFollow,
    GoToFeed,
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

    //목줄 상태인 경우 시간
    private float leashWalkTimer = 0f;
    private float leashWalkInterval = 3f;

    [Header("Fetch System(공 잡아 오는 시스템)")]
    public Transform mouthPos;

    // 공 Object 체크  
    private GameObject targetBall;
    private bool isFetching = false;
    private bool hasBall = false;

    // 먹이 Object 체크
    private GameObject targetFeed;
    private float eatTimer = 0f;
    private float eatDuration = 2f;

    //Dictionary를 통해 변경 상태를 관리
    private Dictionary<AnimalState, Action> UpdateState;
    private Dictionary<AnimalState, Action> EnterState;

    //행동 시간
    private float behaviourTimer = 0f;
    private float behaviourIntervalTime = 2f;

    private void InitializeState()
    {
        UpdateState = new Dictionary<AnimalState, Action>
        {
            {AnimalState.Idle, UpdateIdle},
            {AnimalState.FreeWalk, UpdateWalk },
            {AnimalState.FollowPlayer, UpdateFollow },
            {AnimalState.LeashFollow, UpdateLeashFollow },
            {AnimalState.GoToFeed, UpdateGoToFeed},
            {AnimalState.Eat,UpdateEat }
        };

        EnterState = new Dictionary<AnimalState, Action>
        {
            {AnimalState.Idle,EnterIdle },
            {AnimalState.FreeWalk, EnterWalk},
            {AnimalState.FollowPlayer,EnterFollowPlayer },
            {AnimalState.LeashFollow, EnterLeashFollow },
            {AnimalState.GoToFeed, EnterGoToFeed},
            {AnimalState.Eat, EnterEat }
        };
    }

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        ChangeState(AnimalState.Idle);
        InitializeState();

        nav.updateRotation = false;
    }

    void Update()
    {
        UpdateMovement();
        UpdateFetch();
        UpdateRotation();
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

    public void OnFeedSpawned(GameObject Feed)
    {
        if (isFetching) return;
        targetFeed = Feed;
        nav.isStopped = false;
        nav.SetDestination(Feed.transform.position);
        ChangeState(AnimalState.GoToFeed);
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
                }
                else if (currentState == AnimalState.Idle)
                {
                    ChangeState(AnimalState.FreeWalk);
                }
                behaviourTimer = 0f;
                behaviourIntervalTime = UnityEngine.Random.Range(1f, 3f);
            }
        }
        UpdateState[currentState]?.Invoke();
    }
    void UpdateFetch()
    {
        if (!isFetching || targetBall == null) return;

        if (!hasBall)
        {
            nav.isStopped = false;
            nav.SetDestination(targetBall.transform.position);

            float dist = Vector3.Distance(transform.position, targetBall.transform.position);
            if (dist < 1f)
            {
                targetBall.transform.SetParent(mouthPos);
                targetBall.transform.localPosition = Vector3.zero;
                targetBall.transform.localRotation = Quaternion.identity;

                var rb = targetBall.GetComponent<Rigidbody>();
                if(rb != null)
                {
                    rb.isKinematic = true;
                }

                hasBall = true;

                Vector3 camForward = player.forward;
                camForward.y = 0;
                Vector3 stopPoint = player.position + camForward.normalized * 1.0f;
                nav.SetDestination(stopPoint);
                SetAnimation("Walk");
            }
        }
        else 
        {
            if(!nav.pathPending && nav.remainingDistance < 0.3f)
            {
                nav.isStopped = true;
                nav.ResetPath();

                Vector3 lookDir = (player.position - transform.position);
                lookDir.y = 0;
                if(lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }
                SetAnimation("Sit");

                Destroy(targetBall);
                targetBall = null;
                isFetching = false;
                hasBall = false;

                ChangeState(AnimalState.Idle);
                SetAnimation("Idle");
            }
        }
    }
    void UpdateRotation()
    {
        if (nav.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 dir = nav.velocity.normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);
            }
        }
    }

    public void ChangeState(AnimalState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;
        EnterState[newState]?.Invoke();
    }

    // State Update
    private void UpdateIdle() { }

    private void UpdateWalk()
    {
        walkTimer += Time.deltaTime;
        if(!nav.pathPending && nav.remainingDistance <= 0.3f && walkTimer >=checkInterval)
        {
            MoveRandomPoint();
            walkTimer = 0f;
        }
    }

    private void UpdateFollow()
    {
        if (Vector3.Distance(transform.position, player.position) > callDistance)
        {
            nav.isStopped = false;
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
        if(Vector3.Distance(transform.position, player.position) >= leashFollowDistance)
        {
            nav.isStopped = false;
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
    
    private void UpdateGoToFeed()
    {
        if (targetFeed == null)
        {
            ChangeState(AnimalState.Idle);
            return;
        }

        if(!nav.pathPending && nav.remainingDistance < 0.3f)
        {
            nav.isStopped = true;
            ChangeState(AnimalState.Eat);
        }
    }
    private void UpdateEat()
    {
        eatTimer -= Time.deltaTime;
        if(eatTimer <= 0f)
        {
            if(targetFeed != null)
            {
                Destroy(targetFeed);
                targetFeed = null;
            }
            ChangeState(AnimalState.Idle);
        }
    }

    //상태 진입 시 동작
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

    private void EnterGoToFeed()
    {
        SetAnimation("Walk");
    }
    private void EnterEat()
    {
        nav.isStopped = true;
        nav.ResetPath();
        SetAnimation("Eat");
        eatTimer = eatDuration;
    }

    private void SetAnimation(string str)
    {
        if (anim == null) return;

        anim.ResetTrigger("Idle");
        anim.ResetTrigger("Walk");
        anim.ResetTrigger("Eat");
        anim.ResetTrigger("Sit");

        anim.SetTrigger(str);
    }

    private void MoveRandomPoint()
    {
        Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized * walkRadius;
        Vector3 targetPos = transform.position + new Vector3(circle.x, 0f, circle.y);

        if (isLeashed)
        {
            Vector3 offset = targetPos - player.position;

            if (offset.magnitude > leashFollowDistance)
            {
                targetPos = player.position + offset.normalized * leashFollowDistance * 0.9f;
            }
        }


        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
        {
            if((hit.position - transform.position).magnitude < 1f)
            {
                return;
            }
            nav.SetDestination(hit.position);
        }
    }

    private void MoveRandomPointInLeashArea()
    {
        float angle = UnityEngine.Random.Range(0, 360);
        float radius = UnityEngine.Random.Range(leashFollowDistance * 0.5f, leashFollowDistance);

        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        Vector3 targetPos = player.position + offset;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, leashFollowDistance, NavMesh.AllAreas))
        {
            if (Vector3.Distance(hit.position, player.position) > leashFollowDistance) return;
            nav.SetDestination(hit.position);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AnimalType { Small, Medium, Large }
public enum AnimalState { Idle, FreeWalk, FollowPlayer, LeashFollow, GoToFeed, Eat, Fetch }
public enum PetAnimation { Idle, Walk, Eat, Sit }

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class AnimalLogic : MonoBehaviour
{
    [Header("Animal Type & CurrentState")] // 현재 반려동물의 타입 및 상태 표시
    public AnimalType type = AnimalType.Small;
    public AnimalState currentState = AnimalState.Idle;

    [Header("Check Player")] // Player의 위치 체크
    public Transform player;

    [Header("Free Walk Setting")]
    public float walkRadius = 5f; // 걸어다니는 범위
    public float checkInterval = 5f; // 랜덤 위치 선정 시간

    [Header("FreeWalk Delay")]
    public float idleToWalkDelay = 3f;
    private float idleTimer = 0f;

    [Header("Follow 설정")]
    public float callDistance = 1.5f; // 반려동물을 불렀을 경우 어느 범위까지만 오게 할 것인지에 대한 범위

    [Header("Leash 설정")]
    [SerializeField] private bool isLeashed;
    public float leashFollowDistance = 2f;

    [Header("Fetch System(공 잡아 오는 시스템)")]
    public Transform mouthPos;

    [Header("Feed Settings")]
    public float eatDuration = 2f;

    // NavMeshAgent를 통해 길을 찾아 걸어다닌다. + 반려동물의 애니메이션
    private NavMeshAgent nav;
    private Animator anim;

    //Dictionary를 통해 변경 상태를 관리
    private Dictionary<AnimalState, Action> UpdateState;
    private Dictionary<AnimalState, Action> EnterState;

    //행동 시간
    private float behaviourTimer;

    private AnimalFetchHandler fetchHandler;
    private AnimalFeedHandler feedHandler;
    private AnimalAnimation animationHandler;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        fetchHandler = new AnimalFetchHandler(this);
        feedHandler = new AnimalFeedHandler(this);
        animationHandler = new AnimalAnimation(anim);

        InitializeState();
        ChangeState(AnimalState.Idle);

        nav.updateRotation = false;
    }

    void Update()
    {
        if (currentState == AnimalState.GoToFeed || currentState == AnimalState.Eat)
        {
            feedHandler.UpdateFeed();
            return;
        }

        UpdateState[currentState]?.Invoke();
        UpdateRotation();
    }

    private void InitializeState()
    {
        UpdateState = new Dictionary<AnimalState, Action>
        {
            {AnimalState.Idle, UpdateIdle},
            {AnimalState.FreeWalk, UpdateWalk },
            {AnimalState.FollowPlayer, UpdateFollow },
            {AnimalState.LeashFollow, UpdateLeashFollow },
            {AnimalState.Fetch, () =>
            {
                fetchHandler.UpdateFetch();
            } }
        };

        EnterState = new Dictionary<AnimalState, Action>
        {
            {AnimalState.Idle,() =>{
                nav.isStopped = true;
                nav.ResetPath();
                idleTimer= 0f;
                animationHandler.SetAnimation(PetAnimation.Idle);
            } },
            {AnimalState.FreeWalk, () =>{
                nav.isStopped = false;
                MoveRandomPoint();
                animationHandler.SetAnimation(PetAnimation.Walk);
            } },
            {AnimalState.FollowPlayer,()=>{
                nav.isStopped = false;
                animationHandler.SetAnimation(PetAnimation.Walk);
            } },
            {AnimalState.LeashFollow, ()=>{
                nav.isStopped = false;
                MoveRandomPointInLeashArea();
                animationHandler.SetAnimation(PetAnimation.Walk);
            } },
            {AnimalState.GoToFeed, () => feedHandler.EnterFeed()},
            {AnimalState.Eat, () => feedHandler.EnterEat() },
            {AnimalState.Fetch, () =>
            {
                nav.isStopped = false;
                animationHandler.SetAnimation(PetAnimation.Walk);
            } }
        };
    }

    public void ChangeState(AnimalState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        EnterState[newState]?.Invoke();
    }

    public void OnBallSpawned(GameObject ball) => fetchHandler.OnBallSpawned(ball);
    public void OnFeedSpawned(GameObject feed) => feedHandler.OnFeedSpawned(feed);

    // State Update
    private void UpdateIdle()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleToWalkDelay)
        {
            idleTimer = 0f;
            ChangeState(AnimalState.FreeWalk);
        }
    }

    private void UpdateWalk()
    {
        behaviourTimer += Time.deltaTime;
        if (!nav.pathPending && nav.remainingDistance <= 0.3f && behaviourTimer >= checkInterval)
        {
            MoveRandomPoint();
            behaviourTimer = 0f;
        }
    }

    private void UpdateFollow()
    {
        if (Vector3.Distance(transform.position, player.position) > callDistance)
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
        if (Vector3.Distance(transform.position, player.position) >= leashFollowDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            Vector3 chk = player.position - dir * (leashFollowDistance * 0.9f);

            if(NavMesh.SamplePosition(chk,out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                nav.isStopped = false;
                nav.SetDestination(hit.position);
            }

            return;
        }

        if (!nav.pathPending && nav.remainingDistance <= 0.3f)
        {
            MoveRandomPointInLeashArea();
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

    private void MoveRandomPoint()
    {
        Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized * walkRadius;
        Vector3 targetPos = transform.position + new Vector3(circle.x, 0f, circle.y);

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
        {
            if ((hit.position - transform.position).magnitude < 1f)
            {
                return;
            }
            nav.SetDestination(hit.position);
        }
    }

    private void MoveRandomPointInLeashArea()
    {
        Vector3 rndDir = UnityEngine.Random.insideUnitSphere;
        rndDir.y = 0;
        rndDir.Normalize();

        float radius = UnityEngine.Random.Range(leashFollowDistance * 0.3f, leashFollowDistance * 0.95f);
        Vector3 targetPos = player.position + rndDir * radius;

        if(NavMesh.SamplePosition(targetPos,out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            float distFromPlayer = Vector3.Distance(player.position, hit.position);

            if (distFromPlayer <= leashFollowDistance)
            {
                nav.SetDestination(hit.position);
            }
            else
            {
                Vector3 clampedPos = player.position + (hit.position - player.position);
                if(NavMesh.SamplePosition(clampedPos, out NavMeshHit clampedHit, 1.0f, NavMesh.AllAreas))
                {
                    nav.SetDestination(clampedHit.position);
                }
            }
        }

    }

    public void SetLeashed(bool on)
    {
        isLeashed = on;
        if (on)
        {
            ChangeState(AnimalState.LeashFollow);
        }
        else
        {
            ChangeState(AnimalState.Idle);
        }
    }

    public NavMeshAgent Agent => nav;
    public Animator animator => anim;
    public Transform Player => player;
    public Transform Mouth => mouthPos;
    public AnimalAnimation AnimationHandler => animationHandler;
    public float EatDuration => eatDuration;
    public AnimalState CurrentState => currentState;
    public void SetState(AnimalState state) => ChangeState(state);
    public bool IsLeashed => isLeashed;
}

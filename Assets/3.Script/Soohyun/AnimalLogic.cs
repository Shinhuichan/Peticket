using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AnimalType { Small, Medium, Large }
public enum AnimalState { Idle, FreeWalk, FollowPlayer, LeashFollow, GoToFeed, Eat }
public enum PetAnimation { Idle, Walk, Eat, Sit }

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class AnimalLogic : MonoBehaviour
{
    [Header("Animal Type & CurrentState")] // ���� �ݷ������� Ÿ�� �� ���� ǥ��
    public AnimalType type = AnimalType.Small;
    public AnimalState currentState = AnimalState.Idle;

    [Header("Check Player")] // Player�� ��ġ üũ
    public Transform player;

    [Header("Free Walk Setting")]
    public float walkRadius = 5f; // �ɾ�ٴϴ� ����
    public float checkInterval = 5f; // ���� ��ġ ���� �ð�

    [Header("FreeWalk Delay")]
    public float idleToWalkDelay = 3f;
    private float idleTimer = 0f;

    [Header("Follow ����")]
    public float callDistance = 1.5f; // �ݷ������� �ҷ��� ��� ��� ���������� ���� �� �������� ���� ����

    [Header("Leash ����")]
    [SerializeField] private bool isLeashed;
    public float leashFollowDistance = 2f;

    [Header("Fetch System(�� ��� ���� �ý���)")]
    public Transform mouthPos;

    [Header("Feed Settings")]
    public float eatDuration = 2f;

    // NavMeshAgent�� ���� ���� ã�� �ɾ�ٴѴ�. + �ݷ������� �ִϸ��̼�
    private NavMeshAgent nav;
    private Animator anim;

    //Dictionary�� ���� ���� ���¸� ����
    private Dictionary<AnimalState, Action> UpdateState;
    private Dictionary<AnimalState, Action> EnterState;

    //�ൿ �ð�
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
        fetchHandler.UpdateFetch();
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
        if(idleTimer >= idleToWalkDelay)
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
            nav.isStopped = false;
            nav.SetDestination(player.position);
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
        float angle = UnityEngine.Random.Range(0, 360);
        float radius = UnityEngine.Random.Range(leashFollowDistance * 0.5f, leashFollowDistance);

        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        Vector3 targetPos = player.position + offset;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, leashFollowDistance, NavMesh.AllAreas))
        {
            nav.SetDestination(hit.position);
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
}

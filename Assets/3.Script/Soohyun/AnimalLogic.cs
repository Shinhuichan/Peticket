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
    // NavMeshAgent�� ���� ���� ã�� �ɾ�ٴѴ�.
    private NavMeshAgent nav;
    // ���� �ݷ������� �ִϸ��̼� ǥ��
    private Animator anim;
    // ���� �ݷ������� ���� ǥ��
    public AnimalState currentState = AnimalState.Idle;
    // Player�� ��ġ�� �̿��Ͽ� �ݷ������� ���ƿ��� �ϵ��� ����
    public Transform player;

    [Header("Free Walk Delay")]
    public float walkRadius = 5f; // �ɾ�ٴϴ� ����
    public float checkInterval = 5f; // ���� ��ġ ���� �ð�
    private float walkTimer = 0f;

    [Header("Follow ����")]
    public float callDistance = 1.5f; // �ݷ������� �ҷ��� ��� ��� ���������� ���� �� �������� ���� ����

    [Header("Leash ����")]
    [SerializeField] private bool isLeashed;
    public float leashFollowDistance = 2f;

    //Dictionary�� ���� ���� ���¸� ����
    private Dictionary<AnimalState, Action> UpdateState;
    private Dictionary<AnimalState, Action> EnterState;
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
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
        if (isLeashed && currentState != AnimalState.LeashFollow)
        {
            ChangeState(AnimalState.LeashFollow);
        }

        UpdateState[currentState]?.Invoke();
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
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > leashFollowDistance)
        {
            nav.SetDestination(player.position);
        }
        else
        {
            nav.ResetPath();
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
            nav.SetDestination(hit.position);
        }

        else
        {
            nav.ResetPath();
        }
    }
}

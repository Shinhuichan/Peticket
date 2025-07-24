using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour
{
    [Header("두 개의 위치 설정")]
    public Transform Pos1;
    public Transform Pos2;

    [Header("Animation Handler")]
    public AISurpriseHandler ai;

    private Animator anim;
    private NavMeshAgent nav;

    private float idleTimer;
    private float idleDuration;

    private float moveDuration;   // 이동한 시간 누적
    private float maxMoveDuration = 5f; // 중간에 쉴 기준 시간

    private bool isIdle = false;
    private Transform targetPos;

    void Start()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        transform.position = Pos1.position;
        nav.Warp(Pos1.position);

        targetPos = Pos2;
        GoToTarget();

        idleDuration = Random.Range(2f, 4f);
        moveDuration = 0f;
    }

    void Update()
    {
        // ❗ 놀란 상태일 땐 아무것도 하지 않음
        if (ai != null && ai.IsSurprised)
        {
            anim.SetBool("IsWalking", false);
            nav.isStopped = true;
            return;
        }

        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                GoToTarget();
                isIdle = false;
                idleTimer = 0f;
                moveDuration = 0f;
                idleDuration = Random.Range(2f, 4f);
            }
        }
        else
        {
            moveDuration += Time.deltaTime;

            if (moveDuration >= maxMoveDuration)
            {
                EnterIdle();
            }

            if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance)
            {
                if (!nav.hasPath || nav.velocity.sqrMagnitude < 0.01f)
                {
                    targetPos = (targetPos == Pos1) ? Pos2 : Pos1;
                    EnterIdle();
                }
            }
        }
    }

    private void GoToTarget()
    {
        nav.isStopped = false;
        nav.SetDestination(targetPos.position);
        anim.SetBool("IsWalking", true);
    }

    private void EnterIdle()
    {
        isIdle = true;
        nav.isStopped = true;
        anim.SetBool("IsWalking", false);
        idleTimer = 0f;
    }
}
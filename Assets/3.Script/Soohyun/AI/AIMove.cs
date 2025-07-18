using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent nav;

    private float randomMoveTimer;
    private float checkTime;

    private Vector3 rndPos;
    private bool waitingForValidPath = false;

    public float moveRadius = 5f;
    public LayerMask obstacleMask; // 벽 감지용 레이어 마스크

    void Start()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        randomMoveTimer = Random.Range(3f, 5f);
        SetPosition(); // 최초 목적지 설정
    }

    void Update()
    {
        checkTime += Time.deltaTime;

        // 목적지 도달 → 애니메이션 멈춤
        if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance)
        {
            if (!nav.hasPath || nav.velocity.sqrMagnitude < 0.01f)
            {
                anim.SetBool("IsWalking", false); // ✅ 여기서 걷기 멈춤

                if (!waitingForValidPath && checkTime >= randomMoveTimer)
                {
                    SetPosition();
                    checkTime = 0f;
                    randomMoveTimer = Random.Range(3f, 5f);
                }
            }
        }

        // 실패 중이면 계속 시도
        if (waitingForValidPath)
        {
            TryFindNewPath();
        }
    }


    private void SetPosition()
    {
        rndPos = GetRandomNavmeshPosition(moveRadius);

        if (IsPathBlocked(rndPos))
        {
            Debug.Log("Raycast: 벽 감지됨 → 다음 프레임에 재시도");
            waitingForValidPath = true;
            return;
        }

        TryFindNewPath();
    }

    private void TryFindNewPath()
    {
        NavMeshPath path = new NavMeshPath();
        nav.CalculatePath(rndPos, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            nav.SetDestination(rndPos);
            anim.SetBool("IsWalking", true);
            waitingForValidPath = false;
        }
        else
        {
            waitingForValidPath = true;
        }
    }

    /// 현재 위치 기준으로 NavMesh 상의 유효한 위치를 반환
    private Vector3 GetRandomNavmeshPosition(float radius)
    {
        Vector3 origin = transform.position;
        Vector3 rndDir = Random.insideUnitSphere * radius;
        rndDir.y = 0f;

        Vector3 candidate = origin + rndDir;

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return origin; // 실패 시 현재 위치 반환
    }

    /// 경로에 벽이 있는지 Raycast로 감지
    private bool IsPathBlocked(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPos);
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, obstacleMask))
        {
            return true;
        }

        return false;
    }
}

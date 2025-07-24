using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AnimalType { Small, Medium, Large }
public enum AnimalState { Idle, FreeWalk, FollowPlayer, LeashFollow, GoToFeed, Eat, Fetch, SitSatisfied, Bark }
public enum PetAnimation { Idle, Walk, EatStart, EatEnd, SitStart, SitEnd, Fetch, Bark }

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class AnimalLogic : MonoBehaviour
{
    [Header("Animal Type & CurrentState")]
    public AnimalType type = AnimalType.Small;
    public AnimalState currentState = AnimalState.Idle;

    [Header("Check Player")]
    public Transform player;

    [Header("Free Walk Setting")]
    public float walkRadius = 5f;
    public float checkInterval = 5f;

    [Header("FreeWalk Delay")]
    public float idleToWalkDelay = 3f;
    private float idleTimer = 0f;

    [Header("Follow 설정")]
    public float callDistance = 1.5f;

    [Header("Leash 설정")]
    [SerializeField] private bool isLeashed;
    public Transform NeckPos;
    public float leashFollowDistance = 2f;

    [Header("Leash 연결 대상")]
    public Transform leashTargetTransform;

    [Header("Fetch System")]
    public Transform mouthPos;

    [Header("Feed Settings")]
    public float eatDuration = 2f;

    [Header("Bark Settings")]
    public float barkDistance = 3f;  // 짖기 감지 거리
    public LayerMask barkTargetLayer;



    private NavMeshAgent nav;
    private Animator anim;
    private AudioClipInventory audios;
    private AudioSource audioSource;

    private float behaviourTimer;
    private float leashWalkTimer;
    private float sitWaitTimer;

    private AnimalFetchHandler fetchHandler;
    private AnimalFeedHandler feedHandler;
    private AnimalAnimation animationHandler;


    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audios = GetComponent<AudioClipInventory>();
        audioSource = GetComponent<AudioSource>();

        fetchHandler = new AnimalFetchHandler(this);
        feedHandler = new AnimalFeedHandler(this);
        animationHandler = new AnimalAnimation(anim);

        if (player == null)
        {
            var xrCam = Camera.main;
            if (xrCam != null)
                player = xrCam.transform;
            else
                Debug.LogWarning("[AnimalLogic] XR Main Camera를 찾을 수 없습니다.");
        }
    }

    private void Start()
    {
        ChangeState(AnimalState.Idle);
        nav.updateRotation = false;
    }
    private void Update()
    {
        if (currentState == AnimalState.GoToFeed || currentState == AnimalState.Eat)
        {
            feedHandler.UpdateFeed();
            return;
        }

        if (currentState == AnimalState.Bark)
        {
            CheckBarkTarget(); // 타겟이 사라지면 여기서만 상태 해제
            return;
        }

        // Bark가 아닐 때만 나머지 동작 실행
        CheckBarkTarget();
        UpdateStateSwitch();
        UpdateRotation();
    }


    private float barkSoundCooldown = 2f;
    private float barkSoundTimer = 0f;
    private Transform currentBarkTarget = null;

    private void CheckBarkTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, barkDistance, barkTargetLayer);

        if (hits.Length > 0)
        {
            Transform newTarget = hits[0].transform;

            if (currentBarkTarget == null || currentBarkTarget != newTarget)
            {
                currentBarkTarget = newTarget;
            }

            if (currentState != AnimalState.Bark)
            {
                ChangeState(AnimalState.Bark);
            }

            barkSoundTimer += Time.deltaTime;
            if (barkSoundTimer >= barkSoundCooldown)
            {
                PlayBarkSound();
                barkSoundTimer = 0f;

                var surprise = currentBarkTarget.GetComponent<AISurpriseHandler>();
                if (surprise != null)
                {
                    surprise.TriggerSurprise(); // 상태 유지
                }
            }
        }
        else
        {
            if (currentBarkTarget != null)
            {
                var surprise = currentBarkTarget.GetComponent<AISurpriseHandler>();
                if (surprise != null && surprise.IsSurprised)
                {
                    surprise.EndSurpriseImmediately(); 
                }

                currentBarkTarget = null;
            }

            if (currentState == AnimalState.Bark)
            {
                ChangeState(AnimalState.FreeWalk);
            }
        }

    }



    private void PlayBarkSound()
    {
        if (audios != null && audios.BarkSound.Length > 0 && audioSource != null)
        {
            int i = Random.Range(0, audios.BarkSound.Length);
            audioSource.PlayOneShot(audios.BarkSound[i]);
            Debug.Log("[BarkSound] 재생됨");
        }
    }
    private void UpdateStateSwitch()
    {
        switch (currentState)
        {
            case AnimalState.Idle:
                UpdateIdle();
                break;
            case AnimalState.FreeWalk:
                UpdateWalk();
                break;
            case AnimalState.FollowPlayer:
                UpdateFollow();
                break;
            case AnimalState.LeashFollow:
                UpdateLeashFollow();
                break;
            case AnimalState.Fetch:
                fetchHandler.UpdateFetch();
                break;
            case AnimalState.SitSatisfied:
                WaitForPatting();
                break;
        }
    }

    public void ChangeState(AnimalState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        EnterStateSwitch(newState);
    }

    private void EnterStateSwitch(AnimalState state)
    {
        switch (state)
        {
            case AnimalState.Idle:
                nav.isStopped = true;
                nav.ResetPath();
                idleTimer = 0f;
                animationHandler.SetAnimation(PetAnimation.Idle);
                break;

            case AnimalState.FreeWalk:
                nav.isStopped = false;
                animationHandler.SetAnimation(PetAnimation.Walk);
                MoveRandomPoint();
                break;

            case AnimalState.FollowPlayer:
                nav.isStopped = false;
                animationHandler.SetAnimation(PetAnimation.Walk);
                break;

            case AnimalState.LeashFollow:
                nav.isStopped = false;
                animationHandler.SetAnimation(PetAnimation.Walk);
                leashWalkTimer = 0f;
                MoveRandomPointInLeashArea();
                break;

            case AnimalState.GoToFeed:
                feedHandler.EnterFeed();
                break;

            case AnimalState.Eat:
                feedHandler.EnterEat();
                if (audios != null && audios.EatSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(audios.EatSound);
                }
                break;

            case AnimalState.Fetch:
                nav.isStopped = false;
                animationHandler.SetAnimation(PetAnimation.Fetch);
                break;

            case AnimalState.SitSatisfied:
                nav.isStopped = true;
                nav.ResetPath();
                sitWaitTimer = 0f;
                animationHandler.SetSitPhase(1); // SitStart
                break;

            case AnimalState.Bark:
                nav.isStopped = true;
                nav.ResetPath();
                animationHandler.SetAnimation(PetAnimation.Bark);

                // ★ 진입 즉시 BarkSound 재생
                PlayBarkSound();
                barkSoundTimer = 0f; // 타이머도 초기화해서 반복 주기 유지

                // ★ 사람 반응도 즉시
                if (currentBarkTarget != null)
                {
                    AISurpriseHandler surprise = currentBarkTarget.GetComponent<AISurpriseHandler>();
                    if (surprise != null)
                    {
                        surprise.TriggerSurprise();
                    }
                }
                break;
        }
    }

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

        if (!nav.pathPending && nav.remainingDistance <= 0.3f)
        {
            ChangeState(AnimalState.Idle);
        }

        if (behaviourTimer >= checkInterval)
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
        // 손 위치 기준으로 이동하도록 수정
        Transform leashTarget = leashTargetTransform != null ? leashTargetTransform : player;

        float distance = Vector3.Distance(transform.position, leashTarget.position);

        if (distance > leashFollowDistance)
        {
            Vector3 dir = (transform.position - leashTarget.position).normalized;
            Vector3 clamped = leashTarget.position + dir * leashFollowDistance * 0.9f;

            if (NavMesh.SamplePosition(clamped, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                nav.isStopped = false;
                nav.SetDestination(hit.position);
            }
        }
        else
        {
            leashWalkTimer += Time.deltaTime;

            if (!nav.pathPending && nav.remainingDistance <= 0.3f)
            {
                if (leashWalkTimer >= checkInterval)
                {
                    MoveRandomPointInLeashArea(); // hand 기준
                    leashWalkTimer = 0f;
                }
            }
        }
    }



    private void WaitForPatting()
    {
        sitWaitTimer += Time.deltaTime;

        if (sitWaitTimer > 2f && sitWaitTimer < 2.1f)
        {
            animationHandler.SetSitPhase(2); // SitLoop
        }

        if (sitWaitTimer > 10f)
        {
            nav.isStopped = true;
            nav.ResetPath();
            animationHandler.SetSitPhase(3); // SitEnd

            StartCoroutine(WaitAndFreeWalk(1.2f));
        }
    }

    private IEnumerator WaitAndFreeWalk(float delay)
    {
        yield return new WaitForSeconds(delay);

        ChangeState(AnimalState.FreeWalk);
    }

    private void UpdateRotation()
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
        for (int i = 0; i < 10; i++) // 최대 10회 시도
        {
            Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized * walkRadius;
            Vector3 targetPos = transform.position + new Vector3(circle.x, 0f, circle.y);

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, walkRadius, NavMesh.AllAreas))
            {
                if ((hit.position - transform.position).magnitude >= 1f)
                {
                    nav.SetDestination(hit.position);
                    return;
                }
            }
        }
    }

    private void MoveRandomPointInLeashArea()
    {
        Transform leashCenter = leashTargetTransform != null ? leashTargetTransform : player;

        Vector3 rndDir = UnityEngine.Random.insideUnitSphere;
        rndDir.y = 0;
        rndDir.Normalize();

        float radius = UnityEngine.Random.Range(leashFollowDistance * 0.3f, leashFollowDistance * 0.95f);
        Vector3 targetPos = leashCenter.position + rndDir * radius;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            if (Vector3.Distance(leashCenter.position, hit.position) <= leashFollowDistance)
            {
                nav.SetDestination(hit.position);
            }
        }
    }



    public void SetLeashed(bool on)
    {
        isLeashed = on;
        Debug.Log($"[SetLeashed] 줄 상태 변경됨: {on}");
        if (on)
        {
            if (Vector3.Distance(transform.position, player.position) > leashFollowDistance)
            {
                Vector3 dir = (transform.position - player.position).normalized;
                Vector3 clamped = player.position + dir * leashFollowDistance * 0.9f;

                if (NavMesh.SamplePosition(clamped, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    nav.SetDestination(hit.position);
                }
            }

            ChangeState(AnimalState.LeashFollow);
        }
        else
        {
            ChangeState(AnimalState.Idle);
        }
    }

    public void OnBallSoundDetected(GameObject ball)
    {
        Debug.Log($"[AnimalLogic] OnBallSoundDetected 실행됨: {ball.name}");
        var ballObj = ball.GetComponent<BallObject>();
        if (ballObj != null && ballObj.isFromInventory)
        {
            fetchHandler.OnBallSpawned(ball);
        }
    }

    public void OnBallSpawned(GameObject ball) => fetchHandler.OnBallSpawned(ball);
    public void OnFeedSpawned(GameObject feed) => feedHandler.OnFeedSpawned(feed);

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
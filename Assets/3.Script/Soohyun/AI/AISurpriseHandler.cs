using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AISurpriseHandler : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent nav;

    private float timer = 0f;
    private float surpriseDuration = 2f;
    private bool isSurprised = false;
    private bool surpriseEnding = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
    }

    private bool isForcedSurprised = false;

    void Update()
    {
        if (isForcedSurprised)
        {
            timer = 0f; // 계속 초기화
            return;
        }

        if (isSurprised)
        {
            timer += Time.deltaTime;
            if (timer >= surpriseDuration && !surpriseEnding)
            {
                surpriseEnding = true;
                StartCoroutine(EndSurpriseAfterDelay(1f));
            }
        }
    }
    private IEnumerator EndSurpriseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isSurprised = false;
        surpriseEnding = false;
        isForcedSurprised = false;

        nav.isStopped = false;
        animator.SetBool("IsSurprised", false);
        Debug.Log("[AISurprise] 놀람 종료 → 이동 재개");
    }

    public void TriggerSurprise()
    {
        timer = 0f;

        // isSurprised가 이미 true여도 타이머는 초기화되도록 함
        if (!isSurprised)
        {
            isSurprised = true;
            surpriseEnding = false;

            nav.isStopped = true;
            animator.SetBool("IsSurprised", true);
            Debug.Log("[AISurprise] 놀람 시작");
        }

        // isForcedSurprised는 일정 시간동안만 true로 유지
        if (!isForcedSurprised)
        {
            StartCoroutine(ForceSurpriseDuration(3f)); // 예: 3초 강제유지
        }
    }

    private IEnumerator ForceSurpriseDuration(float duration)
    {
        isForcedSurprised = true;
        yield return new WaitForSeconds(duration);
        isForcedSurprised = false;
    }


    // 강아지가 떠났을 때 호출
    public void EndSurpriseImmediately()
    {
        if (!isSurprised) return;

        isForcedSurprised = false;
        isSurprised = false;
        surpriseEnding = false;

        // 놀람 애니메이션 정리
        animator.SetBool("IsSurprised", false);

        // 이동 재개는 애니메이션 끝난 후로 미룸
        StartCoroutine(WaitUntilAnimationFinishThenMove("SurprisedFinish", 0.1f));
    }

    private IEnumerator WaitUntilAnimationFinishThenMove(string animStateName, float bufferTime)
    {
        nav.isStopped = true;
        yield return null; // 1프레임 대기

        // 기다리기: 현재 SurprisedFinish 상태가 끝날 때까지
        while (!IsInAnimationState(animStateName))
            yield return null;

        // 애니메이션 재생 시간 기다림
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float waitTime = stateInfo.length - stateInfo.normalizedTime * stateInfo.length + bufferTime;

        yield return new WaitForSeconds(waitTime);

        // 모든 애니메이션 종료 후 → 이동 재개
        animator.SetBool("IsWalking", true);
        nav.isStopped = false;
    }

    private bool IsInAnimationState(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    public bool IsSurprised => isSurprised;
}
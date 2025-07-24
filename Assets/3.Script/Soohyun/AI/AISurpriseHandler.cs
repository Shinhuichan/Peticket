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

    void Update()
    {
        if (isSurprised)
        {
            timer += Time.deltaTime;
            if (timer >= surpriseDuration && !surpriseEnding)
            {
                surpriseEnding = true;
                StartCoroutine(EndSurpriseAfterDelay(1f)); // 놀람 애니메이션 시간만큼 대기
            }
        }
    }

    public void TriggerSurprise()
    {
        timer = 0f;

        if (!isSurprised)
        {
            isSurprised = true;
            surpriseEnding = false;

            nav.isStopped = true;
            animator.SetBool("IsSurprised", true);
            Debug.Log("[AISurprise] 놀람 시작");
        }
    }

    private IEnumerator EndSurpriseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isSurprised = false;
        surpriseEnding = false;

        nav.isStopped = false;
        animator.SetBool("IsSurprised", false);
        Debug.Log("[AISurprise] 놀람 종료 → 이동 재개");
    }

    public bool IsSurprised => isSurprised;
}
using UnityEngine;
using UnityEngine.AI;

public class AnimalFetchHandler
{
    private AnimalLogic animal;
    private GameObject targetBall;

    private bool isGoingToBall = false;
    private bool isReturning = false;
    private bool hasBall = false;
    private bool hasTriggeredFetchAnim = false;

    public AnimalFetchHandler(AnimalLogic logic)
    {
        animal = logic;
    }

    public void OnBallSpawned(GameObject ball)
    {
        if (targetBall != null || isGoingToBall || isReturning)
        {
            Debug.Log("[FetchHandler] 이미 처리 중인 공 있음 → 무시");
            return;
        }

        Debug.Log($"[FetchHandler] 공 감지됨 → Fetch 상태 진입: {ball.name}");

        targetBall = ball;
        isGoingToBall = true;
        isReturning = false;
        hasBall = false;

        animal.SetState(AnimalState.Fetch);
    }

    public void UpdateFetch()
    {
        if (targetBall == null) return;

        if (isGoingToBall)
        {
            if (NavMesh.SamplePosition(targetBall.transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                animal.Agent.isStopped = false;
                animal.Agent.SetDestination(hit.position);

                if (!hasTriggeredFetchAnim)
                {
                    animal.AnimationHandler.SetAnimation(PetAnimation.Fetch);
                    hasTriggeredFetchAnim = true;
                }
            }

            if (CloseEnoughToGrab() && !hasBall)
            {
                GrabBall();
                hasTriggeredFetchAnim = false;
            }
        }

        if (isReturning)
        {
            if (!animal.Agent.pathPending && animal.Agent.remainingDistance <= 0.5f)
            {
                DropBall();
            }
        }
    }

    private void GrabBall()
    {
        targetBall.transform.SetParent(animal.Mouth);
        targetBall.transform.localPosition = Vector3.zero;
        targetBall.transform.localRotation = Quaternion.identity;

        // 위치 안정화
        Vector3 fixedPos = targetBall.transform.position;
        fixedPos.y = Mathf.Max(fixedPos.y, animal.transform.position.y + 0.2f);
        targetBall.transform.position = fixedPos;

        if (targetBall.TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
        if (targetBall.TryGetComponent(out Collider col)) col.enabled = false;

        hasBall = true;
        isGoingToBall = false;
        isReturning = true;

        // ✅ 카메라(시야) 기준 복귀 위치 계산
        Transform cam = animal.Player;
        Vector3 returnPoint = cam.position + cam.forward * 1.2f;
        returnPoint.y = animal.transform.position.y; // y는 강아지와 같은 높이

        if (NavMesh.SamplePosition(returnPoint, out NavMeshHit resultHit, 1.0f, NavMesh.AllAreas))
        {
            animal.Agent.SetDestination(resultHit.position);
        }

        animal.AnimationHandler.SetAnimation(PetAnimation.Walk);
    }

    private void DropBall()
    {
        if (targetBall == null) return;

        targetBall.transform.SetParent(null);

        // ✅ 카메라 시야 앞에 공 내려놓기
        Transform cam = animal.Player;
        Vector3 drop = cam.position + cam.forward * 0.9f;
        drop.y = animal.transform.position.y + 0.05f;

        if (targetBall.TryGetComponent(out Rigidbody rb)) rb.isKinematic = false;
        if (targetBall.TryGetComponent(out Collider col)) col.enabled = true;

        animal.Agent.isStopped = true;
        animal.Agent.ResetPath();

        animal.AnimationHandler.ResetFetchAnimation();
        animal.AnimationHandler.SetSitPhase(1); // SitStart

        targetBall.transform.position = drop;
        targetBall.transform.rotation = Quaternion.identity;

        isReturning = false;
        hasBall = false;
        targetBall = null;

        Debug.Log("[DropBall] SitSatisfied 상태로 전환합니다");
        animal.ChangeState(AnimalState.SitSatisfied);
    }

    private bool CloseEnoughToGrab()
    {
        return Vector3.Distance(animal.transform.position, targetBall.transform.position) <= 1.0f;
    }
}

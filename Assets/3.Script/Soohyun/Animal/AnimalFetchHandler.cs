// ✅ AnimalFetchHandler.cs 수정본
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

        if (targetBall.TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
        if (targetBall.TryGetComponent(out Collider col)) col.enabled = false;

        hasBall = true;
        isGoingToBall = false;
        isReturning = true;

        Vector3 returnPoint = animal.Player.position + animal.Player.forward.normalized * 1.5f;
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

        Vector3 drop = animal.Player.position + animal.Player.forward * 0.9f;
        drop.y = animal.transform.position.y;

        if (targetBall.TryGetComponent(out Rigidbody rb)) rb.isKinematic = false;
        if (targetBall.TryGetComponent(out Collider col)) col.enabled = true;

        animal.Agent.isStopped = true;
        animal.Agent.ResetPath();

        // ✅ Fetch 트리거 초기화 (중복 애니메이션 방지)
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

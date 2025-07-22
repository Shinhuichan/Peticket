using UnityEngine;
using UnityEngine.AI;

public class AnimalFetchHandler
{
    private AnimalLogic animal;
    private GameObject targetBall;

    private bool isGoingToBall = false;
    private bool isReturning = false;
    private bool hasBall = false;

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

    private bool hasTriggeredFetchAnim = false;
    public void UpdateFetch()
    {
        if (targetBall == null) return;

        if (isGoingToBall)
        {
            if (NavMesh.SamplePosition(targetBall.transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                animal.Agent.isStopped = false;
                animal.Agent.SetDestination(hit.position);

                // ✅ 단 한 번만 Trigger
                if (!hasTriggeredFetchAnim)
                {
                    animal.AnimationHandler.SetAnimation(PetAnimation.Fetch);
                    hasTriggeredFetchAnim = true;
                }
            }

            if (CloseEnoughToGrab() && !hasBall)
            {
                GrabBall();
                hasTriggeredFetchAnim = false; // Reset for next use
            }
        }

        if (isReturning)
        {
            if (!animal.Agent.pathPending && animal.Agent.remainingDistance <= 0.5f)
            {
                DropBall();
                animal.ChangeState(AnimalState.Idle);
            }
        }
    }

    private void GrabBall()
    {
        targetBall.transform.SetParent(animal.Mouth);
        targetBall.transform.localPosition = Vector3.zero;
        targetBall.transform.localRotation = Quaternion.identity;

        if(targetBall.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        if(targetBall.TryGetComponent(out Collider col))
        {
            col.enabled = false;
        }
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

        if (targetBall.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
        }

        if (targetBall.TryGetComponent(out Collider col))
        {
            col.enabled = true;
        }

        animal.Agent.isStopped = true;
        animal.Agent.ResetPath();
        animal.AnimationHandler.SetAnimation(PetAnimation.SitStart);

        targetBall.transform.position = drop;
        targetBall.transform.rotation = Quaternion.identity;

        isReturning = false;
        hasBall = false;
        targetBall = null;
    }

    private bool CloseEnoughToGrab()
    {
        float dis = Vector3.Distance(animal.transform.position, targetBall.transform.position);
        return dis <= 1.0f;
    }

}
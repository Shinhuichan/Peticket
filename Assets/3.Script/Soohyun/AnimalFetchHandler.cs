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
        targetBall = ball;
        isGoingToBall = true;
        isReturning = false;
        hasBall = false;

        animal.SetState(AnimalState.Fetch);
    }

    public void UpdateFetch()
    {
        if (isGoingToBall && targetBall != null)
        {
            if (NavMesh.SamplePosition(targetBall.transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                animal.Agent.isStopped = false;
                animal.Agent.SetDestination(hit.position);
                animal.AnimationHandler.SetAnimation(PetAnimation.Walk);
            }

            if(CloseEnoughToGrab() && !hasBall)
            {
                targetBall.transform.SetParent(animal.Mouth);
                targetBall.transform.localPosition = Vector3.zero;
                targetBall.transform.localRotation = Quaternion.identity;

                var rb = targetBall.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                hasBall = true;
                isGoingToBall = false;
                isReturning = true;

                var col = targetBall.GetComponent<SphereCollider>();
                if(col != null){
                    col.enabled = false;
                }

                Vector3 returnPoint = animal.Player.position + animal.Player.forward.normalized * 1.0f;

                if(NavMesh.SamplePosition(returnPoint, out NavMeshHit resultHit, 1.0f, NavMesh.AllAreas))
                {
                    Debug.Log("SetDestination to: " + resultHit.position);
                    animal.Agent.SetDestination(resultHit.position);
                }

                animal.AnimationHandler.SetAnimation(PetAnimation.Walk);
            }
        }

        if (isReturning && targetBall != null)
        {
            if (!animal.Agent.pathPending && animal.Agent.remainingDistance <= 0.5f)
            {
                var col = targetBall.GetComponent<SphereCollider>();
                if (col != null)
                {
                    col.enabled = true;
                }

                targetBall.transform.SetParent(null);
                Object.Destroy(targetBall);
                targetBall = null;

                isReturning = false;
                hasBall = false;

                animal.Agent.isStopped = true;
                animal.Agent.ResetPath();
                animal.AnimationHandler.SetAnimation(PetAnimation.Sit);
            }
        }
    }

    private bool CloseEnoughToGrab()
    {
        float dis = Vector3.Distance(animal.transform.position, targetBall.transform.position);
        return dis <= 1.0f;
    }
}
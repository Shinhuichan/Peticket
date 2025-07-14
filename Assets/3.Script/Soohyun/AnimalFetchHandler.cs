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
        if (targetBall != null || isGoingToBall || isReturning) return;
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
                animal.AnimationHandler.SetAnimation(PetAnimation.Walk);
            }

            if(CloseEnoughToGrab() && !hasBall)
            {
                GrabBall();
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

        targetBall.transform.position = drop;
        targetBall.transform.rotation = Quaternion.identity;

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
        animal.AnimationHandler.SetAnimation(PetAnimation.Sit);

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
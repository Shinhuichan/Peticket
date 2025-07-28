using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFeedHandler : MonoBehaviour
{
    private AnimalLogic animal;
    private GameObject targetFeed;
    private float eatTimer;
    private bool isEating;
    public AnimalFeedHandler(AnimalLogic logic)
    {
        animal = logic;
    }

    public void OnFeedSpawned(GameObject Feed)
    {
        if (animal.CurrentState == AnimalState.GoToFeed || animal.CurrentState == AnimalState.Eat) return;

        targetFeed = Feed;

        Vector3 lookDir = (Feed.transform.position - animal.transform.position);
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            animal.transform.rotation = Quaternion.LookRotation(lookDir);
        }

        animal.Agent.isStopped = false;
        animal.Agent.SetDestination(Feed.transform.position);
        animal.SetState(AnimalState.GoToFeed);
    }


    public void EnterFeed()
    {
        if(targetFeed == null)
        {
            animal.SetState(AnimalState.Idle);
            return;
        }
    }
    public void EnterEat()
    {
        animal.Agent.isStopped = true;
        animal.Agent.ResetPath();

        eatTimer = animal.EatDuration;
        isEating = true;

        animal.StartCoroutine(EatAnimationSequence());
    }
    private IEnumerator EatAnimationSequence()
    {
        animal.AnimationHandler.SetAnimation(PetAnimation.EatStart);
        yield return new WaitForSeconds(1f);

        animal.AnimationHandler.SetAnimation(PetAnimation.EatStart + 1);
        float loopDuration = eatTimer - 2f;
        if (loopDuration < 0) loopDuration = 1f;
        yield return new WaitForSeconds(loopDuration);

        animal.AnimationHandler.SetAnimation(PetAnimation.EatEnd);
        yield return new WaitForSeconds(1f);

        if (targetFeed != null)
        {
            targetFeed.SetActive(false);
            targetFeed = null;
        }

        if (!string.IsNullOrEmpty(animal.petId))
            PetAffinityManager.Instance?.ChangeAffinityAndSave(animal.petId, 1f);

        isEating = false;
        animal.SetState(AnimalState.Idle);
    }

    public void UpdateFeed()
    {
        if(animal.CurrentState == AnimalState.GoToFeed && targetFeed != null)
        {
            if(!animal.Agent.pathPending && animal.Agent.remainingDistance < 0.3f)
            {
                animal.SetState(AnimalState.Eat);
            }
        }

        if(animal.CurrentState == AnimalState.Eat && isEating)
        {
            eatTimer -= Time.deltaTime;
            if(eatTimer <= 0f)
            {
                if (targetFeed != null)
                {
                    targetFeed.SetActive(false); // Destroy ¡æ SetActive
                    targetFeed = null;
                }

                isEating = false;
                animal.SetState(AnimalState.Idle);
            }
        }
    }
}

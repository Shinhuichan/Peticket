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
        animal.AnimationHandler.SetAnimation(PetAnimation.EatStart);

        eatTimer = animal.EatDuration;
        isEating = true;
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
                    Destroy(targetFeed);
                    targetFeed = null;
                }

                isEating = false;
                animal.SetState(AnimalState.Idle);
            }
        }
    }
}

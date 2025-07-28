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
        // 1. EatStart
        animal.AnimationHandler.SetAnimation(PetAnimation.EatStart);
        yield return new WaitForSeconds(1f); // EatStart 길이 (애니메이션 클립 길이에 따라 조절)

        // 2. EatLoop
        animal.AnimationHandler.SetAnimation(PetAnimation.Idle); // 일단 Loop 전용 트리거 없으면 Idle 대체
        float loopDuration = eatTimer - 2f; // 총 식사 시간에서 Start/End 빼고 Loop에 할당
        if (loopDuration < 0) loopDuration = 1f;

        animal.AnimationHandler.SetAnimation(PetAnimation.EatStart + 1); // 임시로 EatLoop 분기 처리 시
        yield return new WaitForSeconds(loopDuration);

        // 3. EatEnd
        animal.AnimationHandler.SetAnimation(PetAnimation.EatEnd);
        yield return new WaitForSeconds(1f); // EatEnd 애니메이션 시간

        // 음식 처리 및 상태 복귀
        if (targetFeed != null)
        {
            targetFeed.SetActive(false); // Destroy 대신 비활성화
            targetFeed = null;
        }

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
                    targetFeed.SetActive(false); // Destroy → SetActive
                    targetFeed = null;
                }

                isEating = false;
                animal.SetState(AnimalState.Idle);
            }
        }
    }
}

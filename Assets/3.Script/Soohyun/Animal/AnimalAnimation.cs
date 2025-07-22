using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimation
{
    private Animator animator;
    private PetAnimation currentAnimation;

    public AnimalAnimation(Animator anim)
    {
        animator = anim;
        currentAnimation = PetAnimation.Idle; // �⺻��
    }

    public void SetAnimation(PetAnimation animType)
    {
        if (currentAnimation == animType)
            return;

        currentAnimation = animType;

        // Ʈ���� �ʱ�ȭ �� �� Ʈ���� ����
        ResetAllTriggers();

        switch (animType)
        {
            case PetAnimation.Idle:
                animator.SetTrigger("Idle");
                break;
            case PetAnimation.Walk:
                animator.SetTrigger("Walk");
                break;
            case PetAnimation.Fetch:
                animator.SetTrigger("Fetch");
                break;
            case PetAnimation.EatStart:
                animator.SetTrigger("EatStart");
                break;
            case PetAnimation.EatEnd:
                animator.SetTrigger("EatEnd");
                break;
        }
    }

    private void ResetAllTriggers()
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("Fetch");
        animator.ResetTrigger("EatStart");
        animator.ResetTrigger("EatEnd");
    }

    public void SetSitPhase(int phase)
    {
        animator.SetInteger("SitPhase", phase);
    }

    public void ResetFetchAnimation()
    {
        animator.ResetTrigger("Fetch"); // Trigger ����� ��� �ʿ�
    }
}

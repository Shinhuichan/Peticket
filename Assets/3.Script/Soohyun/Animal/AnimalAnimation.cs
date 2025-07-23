using UnityEngine;

public class AnimalAnimation
{
    private Animator animator;
    private PetAnimation currentAnim;

    public AnimalAnimation(Animator anim)
    {
        animator = anim;
        currentAnim = PetAnimation.Idle;
    }

    public void SetAnimation(PetAnimation animType)
    {
        if (currentAnim == animType) return;
        currentAnim = animType;

        ResetAllTriggers();

        animator.SetTrigger(animType.ToString());
    }

    private void ResetAllTriggers()
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("Fetch");
        animator.ResetTrigger("SitStart");
        animator.ResetTrigger("SitEnd");
        animator.ResetTrigger("EatStart");
        animator.ResetTrigger("EatEnd");
    }


    public void SetSitPhase(int phase)
    {
        animator.SetInteger("SitPhase", phase);
    }

    public void ResetFetchAnimation()
    {
        animator.ResetTrigger("Fetch"); // Trigger 기반일 경우 필요
    }
}

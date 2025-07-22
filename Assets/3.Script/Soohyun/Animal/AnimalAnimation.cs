using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimation
{
    private Animator animator;
    public AnimalAnimation (Animator anim)
    {
        animator = anim;
    }

    public void SetAnimation(PetAnimation animType)
    {
        if (animator == null) return;

        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("EatStart");
        animator.ResetTrigger("EatEnd");
        animator.ResetTrigger("SitStart");
        animator.ResetTrigger("SitEnd");

        animator.SetTrigger(animType.ToString());
    }
}

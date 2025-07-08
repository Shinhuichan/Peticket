using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimation : MonoBehaviour
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
        animator.ResetTrigger("Eat");
        animator.ResetTrigger("Sit");

        animator.SetTrigger(animType.ToString());
    }
}

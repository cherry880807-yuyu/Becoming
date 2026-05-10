using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimation : IAnimation
{
    private Animator animator;

    public BasicAnimation(Animator animator)
    {
        this.animator = animator;
    }

    public void SetState(CharacterState state)
    {
        animator.SetFloat("Speed", state.velocity.magnitude);
        animator.SetBool("Sprint", state.isSprinting);
        animator.SetBool("Grounded", state.isGrounded);
        animator.SetBool("Dashing", state.isDashing);
    }
}
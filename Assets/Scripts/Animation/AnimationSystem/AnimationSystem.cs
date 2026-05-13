using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem 
{
    private Animator animator;

    public AnimationSystem(Animator animator)
    {
        this.animator = animator;
    }

    public void SetMovementState(MovementState state)
    {
        animator.SetFloat("Speed", state.velocity.magnitude);
        animator.SetBool("Sprint", state.isSprinting);
        animator.SetBool("Grounded", state.isGrounded);
        animator.SetBool("Dashing", state.isDashing);
        animator.SetFloat("VerticalVelocity", state.velocity.y);
    }
    public void PlayAttack(int index)
    {
        animator.SetTrigger($"Attack{index}");
    }
     public void PlayHit()
    {

    }
     public void PlayDeath()
    {

    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem
{
    private Animator animator;
    private static readonly int _attackLayerIndex = 1;
    public AnimationSystem(Animator animator)
    {
        this.animator = animator;
    }

    public void SetMovementState(MovementState state)
    {
        animator.SetFloat("Speed", Mathf.Abs(state.velocity.x));
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
        animator.SetTrigger("GetHit");
    }
    public void PlayDeath()
    {

    }


    public float GetAttackNormalizedTime()
    {
        var info = animator.GetCurrentAnimatorStateInfo(_attackLayerIndex);
        if (!info.IsTag("Attack")) return -1f;
        return info.normalizedTime;
    }

    public bool IsInAttackState()
    {
        return animator.GetCurrentAnimatorStateInfo(_attackLayerIndex).IsTag("Attack");
    }

}
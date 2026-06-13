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
    public void PlayAttack(int index, ComboType comboType)
    {
        string attackType = comboType switch
        {
            ComboType.Ground => "NormalAttack",
            ComboType.Dash => "DashAttack",
            ComboType.Air => "AirAttack",
            _ => "NormalAttack"
        };

        animator.SetTrigger($"{attackType}{index + 1}");
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
        if (animator.IsInTransition(_attackLayerIndex))
        {
            // 取 next state 的資訊
            var nextInfo = animator.GetNextAnimatorStateInfo(_attackLayerIndex);
            if (nextInfo.IsTag("Attack")) return 0f; // 剛進入攻擊，給 0
            return -1f;
        }

        var info = animator.GetCurrentAnimatorStateInfo(_attackLayerIndex);
        if (!info.IsTag("Attack")) return -1f;
        return info.normalizedTime;
    }

    public bool IsInAttackState()
    {
        return animator.GetCurrentAnimatorStateInfo(_attackLayerIndex).IsTag("Attack");
    }

}
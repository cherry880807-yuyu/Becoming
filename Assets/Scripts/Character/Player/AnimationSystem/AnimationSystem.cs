using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
AttackType = 0 普通地面攻擊
AttackType = 1 衝刺攻擊
AttackType = 2 空中攻擊
AttackIndex = 0 / 1 / 2*/
public class AnimationSystem
{
    private Animator animator;
    private AnimatorOverrideController overrideController;
    private static readonly int _attackLayerIndex = 1;
    private static readonly int AttackTypeHash = Animator.StringToHash("AttackType");
    private static readonly int AttackIndexHash = Animator.StringToHash("AttackIndex");
    private static readonly int PlayAttackHash = Animator.StringToHash("PlayAttack");
    private static readonly int SkillChargeTrigger = Animator.StringToHash("SkillCharge");
    private static readonly int SkillCastTrigger = Animator.StringToHash("SkillCast");
    private static readonly int SkillCancelTrigger = Animator.StringToHash("SkillCancel");
    private const string ChargeSkillBaseClipName = "ChargeSkill_Charge";
    private const string ReleaseSkillBaseClipName = "ChargeSkill_Release";
    private const string CancelSkillBaseClipName = "ChargeSkill_Cancel";

    public AnimationSystem(Animator animator)
    {
        this.animator = animator;
        InitializeOverrideController();
    }

    public void SetMovementState(MovementState state)
    {
        animator.SetFloat("Speed", Mathf.Abs(state.velocity.x));
        animator.SetBool("Sprint", state.isSprinting);
        animator.SetBool("Grounded", state.isGrounded);
        animator.SetBool("Dashing", state.isDodging);
        animator.SetFloat("VerticalVelocity", state.velocity.y);
    }
    public void PlayAttack(int index, ComboType comboType)
    {
        animator.SetInteger(AttackTypeHash, (int)comboType);
        animator.SetInteger(AttackIndexHash, index);
        animator.SetTrigger(PlayAttackHash);
    }
    public void PlayHit()
    {
        animator.SetTrigger("GetHit");
    }
    public void PlaySkillTrigger(string triggerName)
    {
        if (string.IsNullOrWhiteSpace(triggerName)) return;
        animator.SetTrigger(triggerName);
    }

    public void ApplyActiveSkillAnimation(ActiveSkillAnimationProfile profile)
    {
        if (profile == null || overrideController == null) return;

        OverrideClip(ChargeSkillBaseClipName, profile.chargeClip);
        OverrideClip(ReleaseSkillBaseClipName, profile.releaseClip);
        OverrideClip(CancelSkillBaseClipName, profile.cancelClip);
    }

    public void ApplyWeaponAnimation(WeaponDataSO weapon)
    {
        if (weapon == null || overrideController == null) return;

        OverrideComboClips(ComboType.Ground, weapon.groundSteps);
        OverrideComboClips(ComboType.Air, weapon.airSteps);
        OverrideComboClips(ComboType.Dash, weapon.dashSteps);
    }

    public void PlaySkillCharge()
    {
        animator.SetTrigger(SkillChargeTrigger);
    }

    public void PlaySkillCast()
    {
        animator.SetTrigger(SkillCastTrigger);
    }

    public void PlaySkillCancel()
    {
        animator.SetTrigger(SkillCancelTrigger);
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

    private void InitializeOverrideController()
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;

        overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (overrideController == null)
            overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

        animator.runtimeAnimatorController = overrideController;
    }

    private void OverrideClip(string baseClipName, AnimationClip replacement)
    {
        if (replacement == null) return;

        AnimationClip baseClip = FindBaseClip(baseClipName);
        if (baseClip == null)
        {
                Debug.LogWarning($"AnimatorOverrideController missing base clip '{baseClipName}'.", animator);
            return;
        }

        overrideController[baseClip] = replacement;
    }

    private void OverrideComboClips(ComboType comboType, ComboStep[] steps)
    {
        if (steps == null) return;

        for (int i = 0; i < steps.Length; i++)
        {
            ComboStep step = steps[i];
            if (step == null) continue;
            OverrideClip(GetAttackBaseClipName(comboType, i), step.anim);
        }
    }

    private string GetAttackBaseClipName(ComboType comboType, int index)
    {
        return comboType switch
        {
            ComboType.Ground => $"Attack_Ground_{index}_Base",
            ComboType.Air => $"Attack_Air_{index}_Base",
            ComboType.Dash => $"Attack_Dash_{index}_Base",
            _ => $"Attack_Ground_{index}_Base"
        };
    }

    private AnimationClip FindBaseClip(string clipName)
    {
        AnimationClip[] clips = overrideController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            AnimationClip clip = clips[i];
            if (clip != null && clip.name == clipName) return clip;
        }

        return null;
    }

}

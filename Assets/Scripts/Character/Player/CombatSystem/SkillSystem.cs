using System.Collections.Generic;
using UnityEngine;
public enum ActiveSkillTriggerType //輸入觸發方式
{
    ChargedAttack,
    RapidCombo,
    AirDownAttack,
    Ultimate
}
public sealed class SkillSystem
{
    private readonly Dictionary<SubCategory, ActiveSkillDataSO> equippedBySubCategory = new();
    private readonly Dictionary<string, float> nextReadyTimeBySkillId = new();
    private WeaponDataSO currentWeapon;
    private ActiveSkillDataSO chargingSkill;
    private ActiveSkillContext chargingContext;
    private ActiveSkillCastRequest pendingCastRequest;
    private bool hasPendingCast;

    public bool IsCharging => chargingSkill != null;
    public bool HasPendingCast => hasPendingCast;

    public void Register(SubCategory subCategory, ActiveSkillDataSO skill)
    {
        if (skill == null) return;
        if (!IsSkillAvailableForCurrentWeapon(subCategory, skill))
        {
            Debug.LogWarning($"{skill.name} is not available for current weapon.", skill);
            return;
        }

        equippedBySubCategory[subCategory] = skill;
    }

    public void Unregister(SubCategory subCategory, ActiveSkillDataSO skill)
    {
        if (skill == null) return;
        if (equippedBySubCategory.TryGetValue(subCategory, out ActiveSkillDataSO equipped) && equipped == skill)
            equippedBySubCategory.Remove(subCategory);
    }

    public void SetWeapon(WeaponDataSO weapon)
    {
        if (currentWeapon == weapon) return;

        currentWeapon = weapon;
        equippedBySubCategory.Clear();
        chargingSkill = null;
        chargingContext = default;
        pendingCastRequest = default;
        hasPendingCast = false;
    }

    public bool TryExecute(ActiveSkillTriggerType triggerType, ActiveSkillContext context)
    {
        SubCategory subCategory = GetSubCategory(triggerType);
        if (!equippedBySubCategory.TryGetValue(subCategory, out ActiveSkillDataSO skill)) return false;
        if (skill == null || !skill.CanTrigger(context)) return false;
        if (!IsCooldownReady(skill)) return false;
        if (!CanPayCost(skill, context)) return false;

        ActiveSkillCastRequest request = new ActiveSkillCastRequest(skill, context);
        if (skill.castEffect == null || !skill.castEffect.CanExecute(request)) return false;

        PayCost(skill, context);
        skill.castEffect.Execute(request);
        StartCooldown(skill);
        return true;
    }

    public bool CanBeginCharge(ActiveSkillTriggerType triggerType, ActiveSkillContext context)
    {
        SubCategory subCategory = GetSubCategory(triggerType);
        if (!equippedBySubCategory.TryGetValue(subCategory, out ActiveSkillDataSO skill)) return false;
        return CanPrepareSkill(skill, context);
    }

    public bool TryBeginCharge(ActiveSkillTriggerType triggerType, ActiveSkillContext context)
    {
        if (chargingSkill != null || hasPendingCast) return false;

        SubCategory subCategory = GetSubCategory(triggerType);
        if (!equippedBySubCategory.TryGetValue(subCategory, out ActiveSkillDataSO skill)) return false;
        if (!CanPrepareSkill(skill, context)) return false;

        chargingSkill = skill;
        chargingContext = context;
        ApplySkillAnimation(context, skill);
        PlaySkillCharge(context);
        return true;
    }

    public bool TryCommitCharge(ActiveSkillContext context)
    {
        if (chargingSkill == null) return false;

        ActiveSkillDataSO skill = chargingSkill;
        ActiveSkillCastRequest request = new ActiveSkillCastRequest(skill, context);

        if (!skill.CanTrigger(context) ||
            !CanPayCost(skill, context) ||
            skill.castEffect == null ||
            !skill.castEffect.CanExecute(request))
        {
            CancelCharge();
            return false;
        }

        chargingSkill = null;
        chargingContext = default;
        pendingCastRequest = request;
        hasPendingCast = true;
        ApplySkillAnimation(context, skill);
        PlaySkillCast(context);

        return true;
    }

    public bool CancelCharge()
    {
        if (chargingSkill == null) return false;

        ActiveSkillDataSO skill = chargingSkill;
        ActiveSkillContext context = chargingContext;
        chargingSkill = null;
        chargingContext = default;
        ApplySkillAnimation(context, skill);
        PlaySkillCancel(context);
        return true;
    }

    public bool ExecutePendingCast()
    {
        if (!hasPendingCast) return false;

        ActiveSkillCastRequest request = pendingCastRequest;
        ActiveSkillDataSO skill = request.Skill;
        hasPendingCast = false;
        pendingCastRequest = default;

        if (skill == null || skill.castEffect == null) return false;
        if (!CanPayCost(skill, request.Context)) return false;
        if (!skill.castEffect.CanExecute(request)) return false;

        PayCost(skill, request.Context);
        skill.castEffect.Execute(request);
        StartCooldown(skill);
        return true;
    }

    private SubCategory GetSubCategory(ActiveSkillTriggerType triggerType)
    {
        return triggerType switch
        {
            ActiveSkillTriggerType.ChargedAttack => SubCategory.Charge,
            ActiveSkillTriggerType.RapidCombo => SubCategory.Lunge,
            ActiveSkillTriggerType.AirDownAttack => SubCategory.Aerial,
            ActiveSkillTriggerType.Ultimate => SubCategory.Special,
            _ => SubCategory.Combat
        };
    }

    private bool IsCooldownReady(ActiveSkillDataSO skill)
    {
        return !nextReadyTimeBySkillId.TryGetValue(skill.SkillKey, out float nextReadyTime) || Time.time >= nextReadyTime;
    }

    private bool CanPrepareSkill(ActiveSkillDataSO skill, ActiveSkillContext context)
    {
        if (skill == null) return false;
        if (!IsCooldownReady(skill)) return false;
        if (skill.castEffect == null) return false;
        return context.ActorData != null;
    }

    private bool CanPayCost(ActiveSkillDataSO skill, ActiveSkillContext context)
    {
        if (skill.staminaCost <= 0f) return true;
        return context.ActorData?.StaminaSystem != null && context.ActorData.StaminaSystem.CanUse(skill.staminaCost);
    }

    private void PayCost(ActiveSkillDataSO skill, ActiveSkillContext context)
    {
        if (skill.staminaCost <= 0f) return;
        context.ActorData?.StaminaSystem?.Consume(skill.staminaCost);
    }

    private void StartCooldown(ActiveSkillDataSO skill)
    {
        if (skill.cooldown <= 0f) return;
        nextReadyTimeBySkillId[skill.SkillKey] = Time.time + skill.cooldown;
    }

    private void ApplySkillAnimation(ActiveSkillContext context, ActiveSkillDataSO skill)
    {
        context.ActorData?.AnimationSystem?.ApplyActiveSkillAnimation(skill.animationProfile);
    }

    private void PlaySkillCharge(ActiveSkillContext context)
    {
        context.ActorData?.AnimationSystem?.PlaySkillCharge();
    }

    private void PlaySkillCast(ActiveSkillContext context)
    {
        context.ActorData?.AnimationSystem?.PlaySkillCast();
    }

    private void PlaySkillCancel(ActiveSkillContext context)
    {
        context.ActorData?.AnimationSystem?.PlaySkillCancel();
    }

    private bool IsSkillAvailableForCurrentWeapon(SubCategory subCategory, ActiveSkillDataSO skill)
    {
        if (currentWeapon == null || currentWeapon.skillSlots == null) return false;

        for (int i = 0; i < currentWeapon.skillSlots.Count; i++)
        {
            WeaponSkillSlot slot = currentWeapon.skillSlots[i];
            if (slot == null || slot.subCategory != subCategory || slot.skills == null) continue;
            if (slot.skills.Contains(skill)) return true;
        }

        return false;
    }
}

public sealed class PlayerCombatSystem
{
    private readonly PlayerNormalAttackInputSystem normalAttackInputSystem;
    private readonly PlayerSkillInputSystem skillInputSystem;
    private readonly SkillSystem skillSystem;

    public bool IsAttacking => normalAttackInputSystem != null && normalAttackInputSystem.IsAttacking;

    public PlayerCombatSystem(
        PlayerNormalAttackInputSystem normalAttackInputSystem,
        PlayerSkillInputSystem skillInputSystem,
        SkillSystem skillSystem)
    {
        this.normalAttackInputSystem = normalAttackInputSystem;
        this.skillInputSystem = skillInputSystem;
        this.skillSystem = skillSystem;
    }

    public void ApplyWeapon(WeaponDataSO weapon)
    {
        normalAttackInputSystem?.SetWeapon(weapon);
        skillSystem?.SetWeapon(weapon);
    }

    public void Evaluate()
    {
        skillInputSystem?.Evaluate();
        normalAttackInputSystem?.EvaluateCombo();
    }

    public void HandleAttackPressed(bool isGrounded)
    {
        if (skillInputSystem != null && skillInputSystem.TryHandleAttackPressed(isGrounded))
            return;

        if (normalAttackInputSystem != null && normalAttackInputSystem.CanQueueNextCombo)
        {
            if (skillInputSystem != null && skillInputSystem.TryHandleQueuedComboInput())
                return;
        }

        normalAttackInputSystem?.OnAttackInput();
    }

    public void HandleAttackReleased(float holdDuration)
    {
        if (skillInputSystem != null && skillInputSystem.TryHandleAttackReleased(holdDuration))
            return;

        if (skillInputSystem != null && skillInputSystem.ConsumeNormalAttackFallbackRequest())
        {
            normalAttackInputSystem?.OnAttackInput();
        }
    }

    public bool TryCancelSkillCast()
    {
        return skillInputSystem != null && skillInputSystem.TryCancelCharge();
    }

    public bool TryExecutePendingSkillCast()
    {
        return skillInputSystem != null && skillInputSystem.ExecutePendingCast();
    }

    public void Dispose()
    {
        normalAttackInputSystem?.Dispose();
    }
}

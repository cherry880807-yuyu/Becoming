using UnityEngine;

public sealed class PlayerSkillInputSystem
{
    private const float DownInputThreshold = -0.5f;
    private const float CommandSequenceWindow = 0.35f;
    private const float ChargeStartThreshold = 0.2f;

    private readonly PlayerBrain player;
    private readonly PlayerActorData actorData;
    private readonly IPlayerSkillInputSource inputSource;
    private bool attackHeld;
    private bool chargeCandidateActive;
    private bool chargeStarted;
    private bool normalAttackFallbackRequested;
    private float attackStartedAt;

    public PlayerSkillInputSystem(
        PlayerBrain player,
        PlayerActorData actorData,
        IPlayerSkillInputSource inputSource)
    {
        this.player = player;
        this.actorData = actorData;
        this.inputSource = inputSource;
    }

    public bool TryHandleAttackPressed(bool isGrounded)
    {
        normalAttackFallbackRequested = false;

        bool hasDownAttackCommand =
            inputSource.MoveInput.y < DownInputThreshold ||
            inputSource.MatchPressedCommandSequence(
                CommandSequenceWindow,
                PlayerCommand.DirectionDown,
                PlayerCommand.Attack);

        if (!isGrounded && hasDownAttackCommand)
            return TryExecute(ActiveSkillTriggerType.AirDownAttack);

        ActiveSkillContext context = CreateContext(ActiveSkillTriggerType.ChargedAttack);
        if (actorData.SkillSystem != null &&
            actorData.SkillSystem.CanBeginCharge(ActiveSkillTriggerType.ChargedAttack, context))
        {
            attackHeld = true;
            chargeCandidateActive = true;
            chargeStarted = false;
            attackStartedAt = Time.time;
            return true;
        }

        return false;
    }

    public void Evaluate()
    {
        if (!attackHeld || !chargeCandidateActive || chargeStarted) return;
        if (Time.time - attackStartedAt < ChargeStartThreshold) return;

        ActiveSkillContext context = CreateContext(
            ActiveSkillTriggerType.ChargedAttack,
            Time.time - attackStartedAt);

        chargeStarted = actorData.SkillSystem != null &&
                        actorData.SkillSystem.TryBeginCharge(ActiveSkillTriggerType.ChargedAttack, context);

        if (!chargeStarted)
            ClearChargeState();
    }

    public bool TryHandleAttackReleased(float holdDuration)
    {
        if (!chargeCandidateActive) return false;

        attackHeld = false;

        if (!chargeStarted && holdDuration >= ChargeStartThreshold)
        {
            ActiveSkillContext beginContext = CreateContext(ActiveSkillTriggerType.ChargedAttack, holdDuration);
            chargeStarted = actorData.SkillSystem != null &&
                            actorData.SkillSystem.TryBeginCharge(ActiveSkillTriggerType.ChargedAttack, beginContext);
        }

        if (chargeStarted)
        {
            ActiveSkillContext commitContext = CreateContext(ActiveSkillTriggerType.ChargedAttack, holdDuration);
            bool committed = actorData.SkillSystem != null && actorData.SkillSystem.TryCommitCharge(commitContext);
            ClearChargeState();
            return true;
        }

        normalAttackFallbackRequested = true;
        ClearChargeState();
        return false;
    }

    public bool TryHandleQueuedComboInput()
    {
        if (!inputSource.WasCommandPressed(PlayerCommand.Attack, CommandSequenceWindow)) return false;
        return TryExecute(ActiveSkillTriggerType.RapidCombo);
    }

    private bool TryExecute(ActiveSkillTriggerType triggerType, float holdDuration = 0f)
    {
        if (actorData.SkillSystem == null) return false;

        ActiveSkillContext context = CreateContext(triggerType, holdDuration);

        return actorData.SkillSystem.TryExecute(triggerType, context);
    }

    public bool TryCancelCharge()
    {
        bool hadChargeCandidate = chargeCandidateActive;
        if (!hadChargeCandidate && actorData.SkillSystem?.IsCharging != true) return false;

        bool canceled = actorData.SkillSystem != null && actorData.SkillSystem.CancelCharge();
        ClearChargeState();
        return canceled || hadChargeCandidate;
    }

    public bool ConsumeNormalAttackFallbackRequest()
    {
        if (!normalAttackFallbackRequested) return false;
        normalAttackFallbackRequested = false;
        return true;
    }

    public bool ExecutePendingCast()
    {
        return actorData.SkillSystem != null && actorData.SkillSystem.ExecutePendingCast();
    }

    private ActiveSkillContext CreateContext(ActiveSkillTriggerType triggerType, float holdDuration = 0f)
    {
        return new ActiveSkillContext(
            player,
            triggerType,
            inputSource.MoveInput,
            holdDuration);
    }

    private void ClearChargeState()
    {
        attackHeld = false;
        chargeCandidateActive = false;
        chargeStarted = false;
        attackStartedAt = 0f;
    }
}

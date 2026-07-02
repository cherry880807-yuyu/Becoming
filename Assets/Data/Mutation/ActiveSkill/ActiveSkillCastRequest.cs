using UnityEngine;

public readonly struct ActiveSkillCastRequest //施放技能請求，把「技能資料」和「當下施放環境」包在一起
{
    public readonly ActiveSkillDataSO Skill;
    public readonly ActiveSkillContext Context;

    public ActiveSkillCastRequest(ActiveSkillDataSO skill, ActiveSkillContext context)
    {
        Skill = skill;
        Context = context;
    }
}


public readonly struct ActiveSkillContext //施放當下環境資料
{
    public readonly PlayerBrain Player;
    public readonly PlayerActorData ActorData;
    public readonly ActiveSkillTriggerType TriggerType;
    public readonly Vector2 MoveInput;
    public readonly Vector2 Facing;
    public readonly Vector3 Origin;
    public readonly float HoldDuration;

    public ActiveSkillContext(
        PlayerBrain player,
        ActiveSkillTriggerType triggerType,
        Vector2 moveInput,
        float holdDuration = 0f)
    {
        Player = player;
        ActorData = player != null ? player.PlayerActorData : null;
        TriggerType = triggerType;
        MoveInput = moveInput;
        Facing = ActorData != null ? ActorData.Facing : Vector2.right;
        Origin = player != null ? player.transform.position : Vector3.zero;
        HoldDuration = holdDuration;
    }
}

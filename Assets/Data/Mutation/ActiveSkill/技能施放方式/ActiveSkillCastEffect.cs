using UnityEngine;

public abstract class ActiveSkillCastEffect : ScriptableObject //技能施放方式
{
    public virtual bool CanExecute(ActiveSkillCastRequest request)
    {
        ActiveSkillContext context = request.Context;
        return context.Player != null && context.ActorData != null;
    }

    public abstract void Execute(ActiveSkillCastRequest request);
}

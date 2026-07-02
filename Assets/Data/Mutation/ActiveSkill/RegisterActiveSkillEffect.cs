using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Effect/Register Active Skill")]
public class ActiveSkillEffect : MutationEffect
{
    public ActiveSkillDataSO skill;

    public override void Apply(GameObject target)
    {
        Apply(target, null);
    }

    public override void Remove(GameObject target)
    {
        Remove(target, null);
    }

    public override void Apply(GameObject target, MutationDataSO sourceMutation)
    {
        PlayerBrain player = target != null ? target.GetComponent<PlayerBrain>() : null;
        if (sourceMutation == null) return;
        player?.PlayerActorData?.SkillSystem?.Register(sourceMutation.subCategory, skill);
    }

    public override void Remove(GameObject target, MutationDataSO sourceMutation)
    {
        PlayerBrain player = target != null ? target.GetComponent<PlayerBrain>() : null;
        if (sourceMutation == null) return;
        player?.PlayerActorData?.SkillSystem?.Unregister(sourceMutation.subCategory, skill);
    }
}

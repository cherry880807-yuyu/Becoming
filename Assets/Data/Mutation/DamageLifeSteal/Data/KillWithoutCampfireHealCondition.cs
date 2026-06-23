using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Condition/Kill Without Campfire Heal Condition")]
public sealed class KillWithoutCampfireHealCondition : MutationCondition
{
    [SerializeField, Min(1)] private int requiredKillCount = 10;

    public override bool Evaluate(MutationContext context)
    {
        return context.enemyKillsSinceCampfireHeal >= requiredKillCount;
    }
}

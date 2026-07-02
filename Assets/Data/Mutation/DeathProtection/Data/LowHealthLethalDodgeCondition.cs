using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Condition/Low Health Lethal Dodge Condition")]
public sealed class LowHealthLethalDodgeCondition : MutationCondition
{
    [SerializeField, Range(0f, 1f)] private float healthThreshold = 0.1f;
    [SerializeField, Min(1)] private int requiredLethalDodgeCount = 5;

    public override bool Evaluate(MutationContext context)
    {
        return context.playerHealthPercent <= healthThreshold &&
               context.totalLethalDodgeCount >= requiredLethalDodgeCount;
    }
}

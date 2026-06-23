using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Condition/Dash Count Condition")]
public sealed class DashCountCondition : MutationCondition
{
    [SerializeField, Min(1)] private int requiredDashCount = 100;

    public override bool Evaluate(MutationContext context)
    {
        return context.totalDashCount >= requiredDashCount;
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Condition/Sprint Distance Condition")]
public sealed class SprintDistanceCondition : MutationCondition
{
    [SerializeField, Min(0f)] private float requiredDistance = 100f;

    public override bool Evaluate(MutationContext context)
    {
        return context.totalSprintDistance >= requiredDistance;
    }
}

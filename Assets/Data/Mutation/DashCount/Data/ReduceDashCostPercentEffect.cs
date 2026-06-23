using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Effect/Reduce Dash Cost Percent Effect")]
public sealed class ReduceDashCostPercentEffect : MutationEffect
{
    [SerializeField, Range(0f, 100f)] private float percent = 20f;

    public override void Apply(GameObject target)
    {
        PlayerBrain player = target.GetComponent<PlayerBrain>();
        if (player == null) return;
        player.PlayerActorData.DashSystem.AddDashCostReductionPercent(percent);
    }

    public override void Remove(GameObject target)
    {
        PlayerBrain player = target.GetComponent<PlayerBrain>();
        if (player == null) return;
        player.PlayerActorData.DashSystem.RemoveDashCostReductionPercent(percent);
    }
}

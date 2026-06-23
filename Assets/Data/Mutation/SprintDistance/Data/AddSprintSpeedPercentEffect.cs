using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Effect/Add Sprint Speed Percent Effect")]
public sealed class AddSprintSpeedPercentEffect : MutationEffect
{
    [SerializeField] private float percent = 10f;

    public override void Apply(GameObject target)
    {
        PlayerBrain player = target.GetComponent<PlayerBrain>();
        if (player == null) return;

        player.PlayerActorData.MovementSystem.AddSprintSpeedMultiplier(percent / 100f);
    }

    public override void Remove(GameObject target)
    {
        PlayerBrain player = target.GetComponent<PlayerBrain>();
        if (player == null)  return;

        player.PlayerActorData.MovementSystem.RemoveSprintSpeedMultiplier(percent / 100f);
    }
}

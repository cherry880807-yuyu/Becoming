using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Effect/Death Protection Effect")]
public sealed class DeathProtectionEffect : MutationEffect
{
    [SerializeField, Range(0f, 1f)] private float healPercent = 0.1f;

    public override void Apply(GameObject target)
    {
        PlayerBrain player = target.GetComponent<PlayerBrain>();
        if (player == null)
            return;

        player.EnableDeathProtection(healPercent);
    }

    public override void Remove(GameObject target)
    {
        PlayerBrain player = target.GetComponent<PlayerBrain>();
        if (player == null)
            return;

        player.DisableDeathProtection();
    }
}

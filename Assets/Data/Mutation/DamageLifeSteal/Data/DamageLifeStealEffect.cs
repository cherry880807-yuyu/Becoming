using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Effect/Damage Life Steal Effect")]
public sealed class DamageLifeStealEffect : MutationEffect
{
    [SerializeField, Range(0f, 100f)] private float lifeStealPercent = 20f;
    [SerializeField, Min(0)] private int minimumHeal = 1;

    private PlayerBrain player;

    public override void Apply(GameObject target)
    {
        player = target.GetComponent<PlayerBrain>();
        if (player == null) return;

        EventBus.Subscribe<PlayerDamageDealtEvent>(OnPlayerDamageDealt);
    }

    public override void Remove(GameObject target)
    {
        EventBus.Unsubscribe<PlayerDamageDealtEvent>(OnPlayerDamageDealt);

        if (player != null && player.gameObject == target)  player = null;
    }

    private void OnPlayerDamageDealt(PlayerDamageDealtEvent eventData)
    {
        if (player == null || eventData.Damage <= 0) return;

        int healAmount = Mathf.FloorToInt(eventData.Damage * lifeStealPercent / 100f);
        if (minimumHeal > 0) healAmount = Mathf.Max(minimumHeal, healAmount);

        if (healAmount > 0) player.Heal(healAmount);
    }
}

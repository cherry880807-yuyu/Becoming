using System.Collections.Generic;
using UnityEngine;

public static class ActiveSkillDamageUtility
{
    public static bool DamageBox(
        ActiveSkillContext context,
        Vector2 center,
        Vector2 size,
        HitConfig hitConfig)
    {
        if (context.Player == null || hitConfig.Damage <= 0 || size.x <= 0f || size.y <= 0f) return false;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, size, 0f);
        HashSet<IDamageable> damaged = new();
        bool hasHit = false;

        foreach (Collider2D collider in colliders)
        {
            if (collider == null || !collider.TryGetComponent(out HurtBox2D hurtBox)) continue;
            if (hurtBox.Team == Team.Player) continue;

            IDamageable damageable = hurtBox.Owner;
            if (damageable == null || !damaged.Add(damageable)) continue;

            InvincibleType invincibleType = hurtBox.GetCurrentInvincibleType();
            if (invincibleType != InvincibleType.None)
            {
                EventBus.Publish(new DodgeSucceededEvent
                {
                    text = InvincibleTypeDB.Text.TryGetValue(invincibleType, out string text) ? text : "Dodge",
                    WorldPosition = hurtBox.transform.position + Vector3.up * 1.5f,
                    incomingDamage = hitConfig.Damage,
                    wouldBeLethal = hurtBox.Owner is BaseBrain brain &&
                                    Mathf.Max(0, hitConfig.Damage - brain.CurrentShield) >= brain.CurrentHP
                });
                continue;
            }

            EventBus.Publish(new PlayerDamageDealtEvent
            {
                Damage = hitConfig.Damage,
                WorldPosition = hurtBox.transform.position
            });

            damageable.TakeDamage(hitConfig);

            EventBus.Publish(new AttackEnemyEvent { hitTime = hitConfig.HitStopTime });
            hasHit = true;
        }

        return hasHit;
    }
}

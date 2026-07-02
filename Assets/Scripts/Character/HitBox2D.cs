using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HitBox2D : MonoBehaviour
{
    protected Collider2D col;
    public Team ownerTeam;
    private readonly HashSet<IDamageable> hitSet = new();
    public Action<IDamageable, Vector2> onHit;
    public Func<IDamageable, int> damagePreviewProvider;

    protected virtual void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;
    }

    public virtual void Activate()
    {
        hitSet.Clear();
        col.enabled = true;
    }

    public virtual void Deactivate()
    {
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HurtBox2D hurtBox = other.GetComponent<HurtBox2D>();
        if (hurtBox == null || hurtBox.Team == ownerTeam)
            return;

        InvincibleType invincibleType = hurtBox.GetCurrentInvincibleType();
        if (invincibleType != InvincibleType.None)
        {
            int incomingDamage = damagePreviewProvider?.Invoke(hurtBox.Owner) ?? 0;
            bool wouldBeLethal = hurtBox.Owner is BaseBrain brain &&
                                 Mathf.Max(0, incomingDamage - brain.CurrentShield) >= brain.CurrentHP;

            EventBus.Publish(new DodgeSucceededEvent
            {
                text = InvincibleTypeDB.Text.TryGetValue(invincibleType, out string t) ? t : "Dodge",
                WorldPosition = hurtBox.transform.position + Vector3.up * 1.5f,
                incomingDamage = incomingDamage,
                wouldBeLethal = wouldBeLethal
            });
            return;
        }

        IDamageable damageable = hurtBox.Owner;
        if (damageable == null || hitSet.Contains(damageable))
            return;

        hitSet.Add(damageable);
        Vector2 dir = (other.transform.position - transform.position).normalized;
        onHit?.Invoke(damageable, dir);
    }
}

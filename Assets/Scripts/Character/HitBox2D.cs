using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HitBox2D : MonoBehaviour
{
    protected Collider2D col;
    public Team ownerTeam;
    private HashSet<IDamageable> _hitSet = new();//防重複命中
    public Action<IDamageable, Vector2> onHit;

    protected virtual void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;
    }

    public virtual void Activate()
    {
        _hitSet.Clear();
        col.enabled = true;
    }

    public virtual void Deactivate()
    {
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var hurtBox = other.GetComponent<HurtBox2D>();
        if (hurtBox == null) return;
        if (hurtBox.Team == ownerTeam) return;
        
        var invincibleType = hurtBox.GetCurrentInvincibleType();
        if (invincibleType != InvincibleType.None)
        {
            EventBus.Publish(new DodgeSucceededEvent
            {
                text = InvincibleTypeDB.Text.TryGetValue(invincibleType, out var t) ? t : "尚未登陸文字",
                WorldPosition = hurtBox.transform.position + Vector3.up * 1.5f
            });
            return;
        }
        var damageable = hurtBox.Owner;
        if (damageable == null || _hitSet.Contains(damageable)) return;

        _hitSet.Add(damageable);
        Vector2 dir = (other.transform.position - transform.position).normalized;
        onHit?.Invoke(damageable, dir);


    }

}
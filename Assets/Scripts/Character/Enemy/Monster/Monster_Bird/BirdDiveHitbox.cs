using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BirdDiveHitbox : MonoBehaviour
{
    private BirdAttackDataSO _data;
    private EnemyActorData _actorData;

    private IDamageable _lastHitTarget;
    private Collider2D col;

    public void Init(BirdAttackDataSO data, EnemyActorData actorData)
    {
        _data = data;
        _actorData = actorData;
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        Deactivate();
    }


    public void Activate()
    {
        _lastHitTarget = null;
        col.enabled = true;
    }

    public void Deactivate()
    {
       col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null || damageable == _lastHitTarget) return;

        _lastHitTarget = damageable;

        Vector2 knockDir = (other.transform.position - _actorData.Transform.position).normalized;

        damageable.TakeDamage(new DamageInfo
        {
            damage = _data.diveDamage,
            knockbackForce = 0,
            hitDirection = knockDir
        });
    }
}
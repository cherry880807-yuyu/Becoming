using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : BaseBrain, IDamageable
{
    [SerializeField] private int maxHP = 9999;
    Rigidbody2D rb;
    protected override void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
        SetMaxHP(maxHP);
    }

    public void TakeDamage(DamageInfo info)
    {
        ApplyDamage(
            info.damage,
            info.hitDirection,
            info.knockbackForce
        );
    }

    protected override void OnApplyKnockback(Vector2 dir, float force)
    {
       //木樁不擊退就註解
       rb.velocity=Vector2.zero;
       rb.AddForce(dir * force, ForceMode2D.Impulse);
    }

    protected override void HandleDeath()
    {
        Destroy(gameObject);
    }
}
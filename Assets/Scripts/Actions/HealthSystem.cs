using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [Header("HP")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Shield")]
    public int shield = 0;

    [Header("State")]
    public bool isInvincible;

    public event Action<int> OnHPChanged;
    public event Action<int> OnDamage;
    public event Action OnDeath;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {

        if (isInvincible) return;

        int finalDamage = damage;

        if (shield > 0)
        {
            int absorbed = Mathf.Min(shield, finalDamage);
            shield -= absorbed;
            finalDamage -= absorbed;
        }

        currentHP -= finalDamage;
        currentHP = Mathf.Max(currentHP, 0);

        OnDamage?.Invoke(damage);
        OnHPChanged?.Invoke(currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }


    public void Heal(int value)
    {
        currentHP += value;
        currentHP = Mathf.Min(currentHP, maxHP);
         OnHPChanged?.Invoke(currentHP);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

}

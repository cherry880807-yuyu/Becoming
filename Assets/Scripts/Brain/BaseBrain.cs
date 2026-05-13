using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseBrain : MonoBehaviour, IDamageable
{
     [SerializeField] CharacterStatsSO characterData;
    protected int currentHP;
    protected int shield;

    protected IState currentState;

    public Action<int> OnHPChanged;
    public Action<int> OnDamage;
    public Action OnDeath;


    public int MaxHP=>characterData.maxHP;
    public int CurrentHP => currentHP;
    public int Shield => shield;





    public virtual void ChangeState(IState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState?.Enter();
    }

    protected virtual void Start()
    {
        currentHP = characterData.maxHP;
        shield = characterData.shield;
        OnHPChanged?.Invoke(currentHP);
    }
    protected virtual void Update()
    {
        currentState?.Update();
    }
    public virtual void TakeDamage(int damage)
    {
        int finalDamage = damage;

        if (shield > 0)
        {
            int absorbed = Mathf.Min(shield, finalDamage);
            shield -= absorbed;
            finalDamage -= absorbed;
        }

        currentHP -= finalDamage;
        currentHP = Mathf.Max(currentHP, 0);

        OnDamage?.Invoke(finalDamage);
        OnHPChanged?.Invoke(currentHP);

        if(currentHP<=0) Destroy(gameObject);
    }
}
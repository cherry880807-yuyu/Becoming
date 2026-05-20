using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem
{
    private PlayerBasicAttackBehavior behavior;
    private PlayerActorData actorData;
    public ComboStep CurrentStep{ get; private set; }

    public int BaseDamage { get; private set; }
    public int FinalDamage => BaseDamage + bonusDamage;
    private int bonusDamage;

    public AttackSystem(PlayerBasicAttackBehavior behavior, PlayerActorData actorData)
    {
        this.behavior = behavior;
        this.actorData = actorData;
    }

    public void AddBonusDamage(int amount)
    {
        bonusDamage += amount;
    }

    public void RemoveBonusDamage(int amount)
    {
        bonusDamage -= amount;
    }

    public void Attack()
    {
        CurrentStep = behavior.Execute(actorData);
        BaseDamage = CurrentStep.damage;
    }


}
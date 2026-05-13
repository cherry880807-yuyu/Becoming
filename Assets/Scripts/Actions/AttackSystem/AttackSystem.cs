using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem
{
    private IAttackBehavior behavior;
    private ActorData actorData;

     public int CurrentDamage { get; private set; }

    public AttackSystem(IAttackBehavior behavior,ActorData actorData)
    {
        this.behavior = behavior;
        this.actorData=actorData;
    }


    public void Attack()
    {
        CurrentDamage=behavior.Execute(actorData);
    }
}
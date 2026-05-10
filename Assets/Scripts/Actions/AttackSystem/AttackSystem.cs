using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem
{
    private IAttack attack;

    public AttackSystem(IAttack attack)
    {
        this.attack = attack;
    }

    public void Attack()
    {
        attack.Attack();
    }
}
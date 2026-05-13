using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBehavior : IAttackBehavior
{
    private ComboData comboData;
    private int index;
    private float lastTime;

    public PlayerAttackBehavior(ComboData comboData)
    {
        this.comboData = comboData;
    }

    public int Execute(ActorData actorData)
    {
        if (comboData.steps.Length == 0) return 0;

        if (Time.time - lastTime > comboData.resetTime) index = 0;
        index %= comboData.steps.Length;
        var step = comboData.steps[index];
        comboData.weaponPrefab.GetComponent<Weapon>().SetStep(step);

        actorData.AnimationSystem.PlayAttack(index);

        index++;
        lastTime = Time.time;

        return step.damage;
    }
}

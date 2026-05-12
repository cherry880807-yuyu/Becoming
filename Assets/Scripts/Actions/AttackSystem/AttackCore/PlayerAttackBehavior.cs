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

    public void Execute(ActorData actorData)
    {
        if (comboData.steps.Length == 0) return;

        if (Time.time - lastTime > comboData.resetTime) index = 0;
        index %= comboData.steps.Length;
        var step = comboData.steps[index];
        Weapon wp = comboData.weaponPrefab.GetComponent<Weapon>();
        wp.SetStep(step);

        actorData.AnimationSystem.PlayAttack(index);

        index++;
        lastTime = Time.time;
    }
}

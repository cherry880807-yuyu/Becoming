using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBasicAttackBehavior 
{
    private ComboDataSO comboData;
    private int index;
    private float lastTime;

    public PlayerBasicAttackBehavior(ComboDataSO comboData)
    {
        this.comboData = comboData;
    }

    public ComboStep Execute(PlayerActorData actorData)
    {
        if (comboData.steps.Length == 0) return null;

        if (Time.time - lastTime > comboData.resetTime) index = 0;
        index %= comboData.steps.Length;
        var step = comboData.steps[index];
        actorData.AnimationSystem.PlayAttack(index);

        index++;
        lastTime = Time.time;

        return step;
    }
}

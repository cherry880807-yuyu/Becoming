using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Condition/AttackCount Condition")]
public class AttackCountCondition : MutationCondition
{
    [SerializeField]private int requiredExposure;

    public override bool Evaluate(MutationContext context)
    {
        return context.totalAttackCount>= requiredExposure;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Condition/Test Condition")]
public class TestCondition : MutationCondition
{
    [SerializeField]private int requiredExposure;

    public override bool Evaluate(MutationContext context)
    {
        return context.test_TotalAttackCount>= requiredExposure;
    }
}


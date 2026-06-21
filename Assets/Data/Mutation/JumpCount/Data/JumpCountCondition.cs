using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Condition/JumpCountCondition Condition")]
public sealed class JumpCountCondition : MutationCondition
{
    [SerializeField, Min(1)] private int requiredJumpCount = 5;

    public override bool Evaluate(MutationContext context)
    {
        return context.totalJumpCount>= requiredJumpCount;
    }
}


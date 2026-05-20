using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//成就條件
public abstract class MutationCondition : ScriptableObject
{
    //檢查條件是否完成
    public abstract bool Evaluate(MutationContext context);
}
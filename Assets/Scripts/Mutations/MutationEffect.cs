using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MutationEffect : ScriptableObject
{
    public abstract void Apply(GameObject target);

    public abstract void Remove(GameObject target);
}

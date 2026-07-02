using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MutationEffect : ScriptableObject
{
    public abstract void Apply(GameObject target);

    public abstract void Remove(GameObject target);

    public virtual void Apply(GameObject target, MutationDataSO sourceMutation)
    {
        Apply(target);
    }

    public virtual void Remove(GameObject target, MutationDataSO sourceMutation)
    {
        Remove(target);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseBrain : MonoBehaviour
{
    protected IState currentState;

    public virtual void ChangeState(IState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState?.Enter();
    }

    protected virtual void Update()
    {
        currentState?.Update();
    }
}
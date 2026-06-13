using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public IState CurrentState { get; private set; }
    public IState PreviousState { get; private set; }

    public void Initialize(IState initialState)
    {
        CurrentState = initialState;
        initialState.Enter();
        Debug.Log("初始化狀態:"+initialState);
    }

    public void ChangeState(IState newState)
    {
        if (newState == null || newState == CurrentState) return;
         Debug.Log(CurrentState+" -> "+newState);
        CurrentState?.Exit();
        PreviousState = CurrentState;
        CurrentState = newState;
        CurrentState.Enter();

    }

    public void Update(float deltaTime) => CurrentState?.Update(deltaTime);
    public void FixedUpdate(float fixedDeltaTime) => CurrentState?.FixedUpdate(fixedDeltaTime);

    public void RevertToPreviousState()
    {
        if (PreviousState != null)
            ChangeState(PreviousState);
    }

}

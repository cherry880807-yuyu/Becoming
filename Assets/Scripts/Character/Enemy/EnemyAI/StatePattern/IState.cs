using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter();
    void Update(float deltaTime);
    void FixedUpdate(float fixedDeltaTime);
    void Exit();
}
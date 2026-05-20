using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChasePattern
{
    void Enter(EnemyActorData actor);
    void Update(float deltaTime, EnemyActorData actor);
    void FixedUpdate(float fixedDeltaTime, EnemyActorData actor);
    void Exit(EnemyActorData actor);
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEnemyAttackPattern
{
    bool CanActivate(EnemyActorData actor);
    void Enter(EnemyActorData actor);
    void Update(float dt);
    void FixedUpdate(float fixedDeltaTime);
    bool IsFinished { get; }
    void Exit();
}
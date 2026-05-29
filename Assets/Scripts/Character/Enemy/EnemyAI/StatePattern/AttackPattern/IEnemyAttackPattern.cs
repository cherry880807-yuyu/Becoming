using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEnemyAttackPattern
{
    void Enter(EnemyActorData actor);
    void Update(float dt);
    void FixedUpdate(float fixedDeltaTime);
    void Exit();
    
    bool IsFinished { get; }
    bool CanBeInterrupted { get; }
    bool CanActivate(EnemyActorData actor);

}
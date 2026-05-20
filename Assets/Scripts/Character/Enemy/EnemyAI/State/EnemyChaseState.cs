using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : IState
{
    private readonly EnemyActorData _actorData;
    private readonly IChasePattern _pattern;
    public EnemyChaseState(EnemyActorData actorData, IChasePattern pattern)
    {
        _actorData = actorData;
        _pattern = pattern;
    }

    public void Enter()
    {
        _pattern.Enter(_actorData);
        _actorData.AnimationSystem.PlayMove(_actorData.EnemyAIDataSO.chaseSpeed);
    }

    public void Update(float deltaTime)
    {
        
        _pattern.Update(deltaTime, _actorData);
    }

    public void FixedUpdate(float fixedDeltaTime)
    {
        _pattern.FixedUpdate(fixedDeltaTime, _actorData);
    }

    public void Exit()
    {
        _actorData.Rigidbody.velocity = Vector2.zero;
        _pattern.Exit(_actorData);
    }
}
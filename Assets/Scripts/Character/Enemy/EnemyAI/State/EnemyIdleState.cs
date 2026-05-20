using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IState
{
    private readonly EnemyActorData _actorData;
    private float _idleTimer;
    private const float IdleDuration = 1.5f;

    public EnemyIdleState(EnemyActorData ctx) => _actorData = ctx;

    public void Enter()
    {
        _idleTimer = 0f;
        _actorData.Rigidbody.velocity = Vector2.zero;
        _actorData.AnimationSystem.PlayIdle();
    }

    public void Update(float deltaTime)
    {

        _idleTimer += deltaTime;
    }

    public void FixedUpdate(float fixedDeltaTime) { }

    public void Exit() { }
}
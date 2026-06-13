using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : IState
{
    private readonly EnemyActorData _actorData;


    private bool _deathTriggered;

    public EnemyDeadState(EnemyActorData ctx) => _actorData = ctx;

    public void Enter()
    {
        _deathTriggered = false;
        _actorData.Rigidbody.velocity = Vector2.zero;
         _actorData.Rigidbody.gravityScale=2f;
        _actorData.Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        _actorData.AnimationSystem.PlayDead();
    }

    public void Update(float deltaTime) { }

    public void FixedUpdate(float fixedDeltaTime) { }

    public void Exit()
    {
        // Dead 是終態，正常不會 Exit，
        // 但如果有復活機制可以在這裡 reset RigidbodyType
    }

}

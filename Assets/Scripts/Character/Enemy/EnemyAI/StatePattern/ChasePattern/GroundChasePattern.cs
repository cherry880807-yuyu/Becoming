using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChasePattern : IChasePattern
{
    private readonly float _chaseSpeed;

    public GroundChasePattern(float chaseSpeed) => _chaseSpeed = chaseSpeed;

    public void Enter(EnemyActorData actorData) { }

    public void Update(float deltaTime, EnemyActorData actorData) { }


    public void FixedUpdate(float fixedDeltaTime, EnemyActorData actorData)
    {
        if (actorData.PlayerTransform == null) return;
        float dir = actorData.PlayerTransform.position.x - actorData.Transform.position.x;
        actorData.Rigidbody.velocity = new Vector2(Mathf.Sign(dir) * _chaseSpeed, actorData.Rigidbody.velocity.y);
        actorData.FlipToward(actorData.PlayerTransform.position.x);
    }

    public void Exit(EnemyActorData actor)
    {
        actor.Rigidbody.velocity = Vector2.zero;
    }
}
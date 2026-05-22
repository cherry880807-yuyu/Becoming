using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHurtState : IState
{
    private readonly EnemyActorData _actorData;
    public bool IsFinished { get; private set; }

    private float _hurtTimer;
    private Vector2 _knockbackDir;

    public EnemyHurtState(EnemyActorData ctx) => _actorData = ctx;

    public void SetKnockback(Vector2 direction) => _knockbackDir = direction;

    public void Enter()
    {
        IsFinished = false;
        _hurtTimer = 0f;
        if (_actorData.IsAlive) _actorData.AnimationSystem.PlayHurt();
        _actorData.Rigidbody.velocity = Vector2.zero;
        _actorData.Rigidbody.AddForce(_knockbackDir * _actorData.EnemyAIDataSO.knockbackForce, ForceMode2D.Impulse);
    }

    public void Update(float deltaTime)
    {
        _hurtTimer += deltaTime;
        if (_hurtTimer >= _actorData.EnemyAIDataSO.hurtDuration)
            IsFinished = true;
    }

    public void FixedUpdate(float fixedDeltaTime) { }

    public void Exit()
    {
        IsFinished = false;
    }
}
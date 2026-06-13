using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : IState
{
    private readonly EnemyActorData _actorData;
    private readonly Transform[] _patrolPoints;
    private int _currentPointIndex;
    private float _waitTimer;
    private bool _isWaiting;

    private const float ReachThreshold = 0.2f;

    public EnemyPatrolState(EnemyActorData ctx, Transform[] patrolPoints)
    {
        _actorData = ctx;
        _patrolPoints = patrolPoints;
    }

    public void Enter()
    {
        _isWaiting = false;
        _waitTimer = 0f;
        _actorData.AnimationSystem.PlayMove(_actorData.EnemyAIDataSO.chaseSpeed);
    }

    public void Update(float deltaTime)
    {

        if (_patrolPoints == null || _patrolPoints.Length == 0) return;

        if (_isWaiting)
        {
            _waitTimer += deltaTime;
            if (_waitTimer >= _actorData.EnemyAIDataSO.patrolWaitTime)
            {
                _isWaiting = false;
                AdvanceToNextPoint();
            }
            return;
        }

        MoveTowardCurrentPoint();
    }

    public void FixedUpdate(float fixedDeltaTime) { }

    public void Exit()
    {
        _actorData.Rigidbody.velocity = Vector2.zero;
    }

    private void MoveTowardCurrentPoint()
    {
        var target = _patrolPoints[_currentPointIndex];

        Vector2 currentPos = _actorData.Transform.position;
        Vector2 targetPos = target.position;
        Vector2 dir = (targetPos - currentPos).normalized;
        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance <= ReachThreshold)
        {
            _actorData.Rigidbody.velocity = Vector2.zero;
            _isWaiting = true;
            _waitTimer = 0f;
            return;
        }

        _actorData.FlipToward(target.position.x);
        _actorData.Rigidbody.velocity =  dir * _actorData.EnemyAIDataSO.patrolSpeed;
    }

    private void AdvanceToNextPoint()
    {
        _currentPointIndex = (_currentPointIndex + 1) % _patrolPoints.Length;
    }
}
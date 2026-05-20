using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMeleeAttack : IEnemyAttackPattern
{
    private readonly GroundMeleeAttackDataSO _data;
    private EnemyActorData _actor;
    private float _lastExitTime = -999f;
    private float _attackTimer;

    public bool IsFinished { get; private set; }

    // Animator hash 靜態快取
    private static readonly int AnimAttack = Animator.StringToHash("Attack");

    public GroundMeleeAttack(GroundMeleeAttackDataSO data) => _data = data;

    // ── IEnemyAttackPattern ───────────────────────────────
    public bool CanActivate(EnemyActorData actorData)
    {
        if (actorData.PlayerTransform == null) return false;

        // 冷卻
        if (Time.time < _lastExitTime + _data.cooldown) return false;

        // 距離
        float dist = Vector2.Distance(
            actorData.Transform.position,
            actorData.PlayerTransform.position
        );
        return dist <= _data.attackRange;
    }

    public void Enter(EnemyActorData actorData)
    {
        _actor      = actorData;
        IsFinished  = false;
        _attackTimer = _data.attackDuration;

        _actor.Rigidbody.velocity = Vector2.zero;
        _actor.Animator.CrossFade(AnimAttack, 0.05f);

        // Hitbox 由 Animation Event 觸發
        // → animator 呼叫 Brain.OnAttackHit()
        // → Brain 呼叫 weapon.DoHitCheck()
    }

    public void Update(float dt)
    {
        _attackTimer -= dt;
        if (_attackTimer <= 0f) IsFinished = true;
    }

    public void FixedUpdate(float fixedDt) { }

    public void Exit()
    {
        _lastExitTime = Time.time;
        _actor.Rigidbody.velocity = Vector2.zero;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Monster_Bird_Enemy : EnemyBrain
{
    [Header("Attack Data")]
    [SerializeField] private BirdAttackDataSO _birdAttackData; // 鳥專屬設定 SO
    [Header("Hitbox")]
    [SerializeField] private BirdDiveHitbox _diveHitbox;
    protected override void BuildStates()
    {
        base.BuildStates();
        _diveHitbox.Init(_birdAttackData, ActorData);
        List<IEnemyAttackPattern> attackPatterns = new List<IEnemyAttackPattern> { new BirdDiveAttack(_birdAttackData) };
        var circleChase = new BirdCircleChasePattern(_birdAttackData);

        // 覆蓋掉 base 建的 _chaseState
        _chaseState = new EnemyChaseState(ActorData, circleChase);
        _attackState = new EnemyAttackState(ActorData, attackPatterns);
        ActorData.Rigidbody.gravityScale = 0f;
    }

    // ── Animation Events ──────────────────────────────────
    public void OnDiveHitboxOpen() => _diveHitbox.Activate();

    public void OnDiveHitboxClose() => _diveHitbox.Deactivate();


}



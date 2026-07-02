using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class Monster_Bird_Enemy : EnemyBrain
{
    [Header("Attack Data")]
    [SerializeField] private BirdAttackDataSO _birdAttackData; // 鳥專屬設定 SO

    protected override void BuildStates()
    {
        base.BuildStates();

        List<IEnemyAttackPattern> attackPatterns = new List<IEnemyAttackPattern> { new BirdDiveAttack(_birdAttackData) };
        var circleChase = new BirdCircleChasePattern(_birdAttackData);

        // 覆蓋掉 base 建的 _chaseState
        _chaseState = new EnemyChaseState(ActorData, circleChase);
        _attackState = new EnemyAttackState(ActorData, attackPatterns);
        ActorData.Rigidbody.gravityScale = 0f;
    }
    protected override void OnHit(IDamageable damageable, Vector2 knockDir)
    {
        damageable.TakeDamage(new HitConfig(
            _birdAttackData.diveDamage,
            knockDir,
            0f,
            transform.position,
            0f));
    }

    protected override int GetHitDamagePreview(IDamageable target)
    {
        return _birdAttackData != null ? _birdAttackData.diveDamage : 0;
    }
}



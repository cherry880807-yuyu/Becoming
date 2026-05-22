using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActorData
{
    //Data
    public EnemyBrain Brain;
    public EnemyAIDataSO EnemyAIDataSO { get; }

    //Component
    public Transform Transform;
    public Rigidbody2D Rigidbody;
    public Animator Animator;
    public Collider2D Collider;
    public SpriteRenderer SpriteRenderer;

    //System
    public EnemyAnimationSystem AnimationSystem { get; set; }

    //State Info
    public Transform PlayerTransform { get; set; }
    public Vector2 PatrolTargetPosition { get; set; }
    public bool IsFacingRight { get; set; } = true;
    public float LastAttackTime { get; set; } = -999f;

    // 血量由外部 HealthComponent 管理，這裡只存快取
    public bool IsAlive => Brain.IsAlive;

    private Transform root;

    public EnemyActorData(EnemyBrain brain, Transform transform, Rigidbody2D rigidbody, Animator animator, Collider2D collider, SpriteRenderer spriteRenderer, EnemyAIDataSO enemyAIData)
    {
        Brain = brain;
        Transform = transform;
        Rigidbody = rigidbody;
        Animator = animator;
        Collider = collider;
        SpriteRenderer = spriteRenderer;
        EnemyAIDataSO = enemyAIData;
        
        root = Transform.GetChild(0);
        AnimationSystem = new EnemyAnimationSystem(animator);
    }

    // ─── 常用工具方法放這裡，讓 State 保持乾淨 ───

    public float DistanceToPlayer()
    {
        if (PlayerTransform == null) return float.MaxValue;
        return Mathf.Abs(Transform.position.x - PlayerTransform.position.x);
    }

    public bool CanDetectPlayer() => PlayerTransform != null && DistanceToPlayer() <= EnemyAIDataSO.detectionRange;

    public void FlipToward(float targetX)
    {
        bool shouldFaceRight = targetX > Transform.position.x;
        if (shouldFaceRight == IsFacingRight) return;

        IsFacingRight = shouldFaceRight;
        //SpriteRenderer.flipX = !IsFacingRight;
        root.localScale = new Vector3(IsFacingRight ? 1 : -1, 1, 1);

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BirdCircleChasePattern : IChasePattern
{
    private readonly BirdAttackDataSO _data;

    private float _orbitAngle;
    private float _currentRadiusX; // 橢圓水平半徑（動態收斂）
    private float _currentRadiusY; // 橢圓垂直半徑（動態收斂）

    public BirdCircleChasePattern(BirdAttackDataSO data) => _data = data;

    public void Enter(EnemyActorData actor)
    {
        Vector2 toActor = (Vector2)actor.Transform.position- (Vector2)actor.PlayerTransform.position;

        // 從目前位置推算初始角度，避免瞬移
        _orbitAngle    = Mathf.Atan2(toActor.y, toActor.x) * Mathf.Rad2Deg;

        // 初始半徑從目前距離開始收斂，X/Y 各自獨立
        _currentRadiusX = Mathf.Abs(toActor.x);
        _currentRadiusY = Mathf.Abs(toActor.y);

        actor.AnimationSystem.PlayMove(1f);
    }

    public void Update(float dt, EnemyActorData actor)
    {
        //FacePlayer(actor);
        actor.FlipToward(actor.PlayerTransform.position.x);
    }

    public void FixedUpdate(float fixedDt, EnemyActorData actor)
    {
        if (actor.PlayerTransform == null) return;

        // 角度旋轉
        _orbitAngle += _data.orbitSpeed * fixedDt;

        // X / Y 半徑各自收斂到目標
        _currentRadiusX = Mathf.Lerp(
            _currentRadiusX,
            _data.orbitRadiusX,
            _data.orbitRadiusLerpSpeed * fixedDt
        );
        _currentRadiusY = Mathf.Lerp(
            _currentRadiusY,
            _data.orbitRadiusY,
            _data.orbitRadiusLerpSpeed * fixedDt
        );

        // 橢圓公式：x = cos(θ) * rx, y = sin(θ) * ry
        float rad = _orbitAngle * Mathf.Deg2Rad;
        Vector2 center = (Vector2)actor.PlayerTransform.position
                       + Vector2.up * _data.orbitHeightOffset;

        Vector2 targetPos = center + new Vector2(
            Mathf.Cos(rad) * _currentRadiusX,
            Mathf.Sin(rad) * _currentRadiusY
        );

        Vector2 moveDir = targetPos - (Vector2)actor.Transform.position;
        actor.Rigidbody.velocity = moveDir * _data.orbitFollowSpeed;
    }

    public void Exit(EnemyActorData actor)
    {
        actor.Rigidbody.velocity = Vector2.zero;
    }

    private void FacePlayer(EnemyActorData actor)
    {
        if (actor.PlayerTransform == null) return;
        float dir = actor.PlayerTransform.position.x - actor.Transform.position.x;
        if (Mathf.Approximately(dir, 0f)) return;

        Vector3 scale = actor.Transform.localScale;
        float targetX = Mathf.Abs(scale.x) * Mathf.Sign(dir);
        if (Mathf.Approximately(scale.x, targetX)) return;
        scale.x = targetX;
        actor.Transform.localScale = scale;
    }
}
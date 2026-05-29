using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DivePhase
{
    None = 0,
    Aim = 1,
    Windup = 2,
    Dive = 3,
    Recover = 4
}
public class BirdDiveAttack : IEnemyAttackPattern
{
    private EnemyActorData _actor;
    private readonly BirdAttackDataSO _data;

    private DivePhase _phase;
    public bool IsFinished { get; private set; }
    public bool CanBeInterrupted => _phase == DivePhase.Aim || _phase == DivePhase.Windup ||_phase == DivePhase.Recover ;

    private Vector2 _diveDir;
    private float _timer;
    private float _lastActivateTime = -999f;

    private static readonly int AnimAim = Animator.StringToHash("Bird_Aim");
    private static readonly int AnimWindup = Animator.StringToHash("Bird_Windup");
    private static readonly int AnimDive = Animator.StringToHash("Bird_Dive");
    private static readonly int AnimRecover = Animator.StringToHash("Bird_Recover");
    public BirdDiveAttack(BirdAttackDataSO data)
    {
        _data = data;
    }

    public bool CanActivate(EnemyActorData actor)
    {
        if (actor.PlayerTransform == null) return false;

        // 冷卻
        if (Time.time < _lastActivateTime + _data.diveCooldown) return false;

        // 距離
        Vector2 toPlayer = actor.PlayerTransform.position - actor.Transform.position;
        if (toPlayer.magnitude > _data.diveRange) return false;

        // 鳥需要在玩家上方
        if (toPlayer.y > -_data.minHeightAbovePlayer) return false;

        return true;
    }
    public void Enter(EnemyActorData actor)
    {
        _actor = actor;
        IsFinished = false;
        _actor.Rigidbody.velocity = Vector2.zero;
        ChangePhase(DivePhase.Aim);
    }

    public void Update(float dt)
    {
        _timer -= dt;
        switch (_phase)
        {
            case DivePhase.Aim:
                UpdateAim();
                break;

            case DivePhase.Windup:
                UpdateWindup();
                break;

            case DivePhase.Dive:
                UpdateDive();
                break;

            case DivePhase.Recover:
                UpdateRecover();
                break;
        }
    }


    private void ChangePhase(DivePhase phase)
    {
        Debug.Log($"{_phase} -> {phase}");
        _phase = phase;

        switch (_phase)
        {
            case DivePhase.Aim:
                _timer = _data.aimDuration;
                _actor.Animator.CrossFade(AnimAim, 0.1f);
                break;

            case DivePhase.Windup:
                _diveDir = (_actor.PlayerTransform.position - _actor.Transform.position).normalized;
                _timer = _data.windupDuration;
                _actor.Animator.CrossFade(AnimWindup, 0.05f);
                break;

            case DivePhase.Dive:
                _timer = _data.diveDuration;
                _actor.Animator.CrossFade(AnimDive, 0.02f);
                break;

            case DivePhase.Recover:
                _timer = _data.recoverDuration;
                _actor.Animator.CrossFade(AnimRecover, 0.05f);

                _actor.Rigidbody.velocity = Vector2.zero;
                break;
        }
    }

    private void UpdateAim()
    {
        // 看向玩家（可選）
        _actor.FlipToward(_actor.PlayerTransform.position.x);
        if (_timer <= 0f)
        {
            ChangePhase(DivePhase.Windup);
        }
    }

    private void UpdateWindup()
    {
        // 前搖動畫
        _actor.FlipToward(_actor.PlayerTransform.position.x);
        if (_timer <= 0f)
        {
            ChangePhase(DivePhase.Dive);
        }
    }
    private void UpdateDive()
    {
        _actor.Rigidbody.velocity = _diveDir * _data.diveSpeed;

        if (_timer <= 0f)
        {
            ChangePhase(DivePhase.Recover);
        }
    }

    private void UpdateRecover()
    {
        Vector2 target = new Vector2(_actor.Transform.position.x, _actor.PlayerTransform.position.y + _data.diveRange);

        Vector2 dir = (target - (Vector2)_actor.Transform.position).normalized;

        _actor.Rigidbody.velocity = dir * 5f;

        if (_timer <= 0f)
        {
            IsFinished = true;
        }
    }


    public void FixedUpdate(float fixedDeltaTime)
    {

    }

    public void Exit()
    {
         _lastActivateTime = Time.time;
        _actor.Rigidbody.velocity = Vector2.zero;
    }
}
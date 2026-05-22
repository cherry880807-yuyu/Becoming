using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : IState
{
    private readonly EnemyActorData _actorData;
    private readonly List<IEnemyAttackPattern> _patterns;

    private IEnemyAttackPattern _currentPattern;
    private int _LastPatternIndex;

    public bool IsFinished { get; private set; }
    public bool CurrentPatternCanBeInterrupted => _currentPattern.CanBeInterrupted;






    /// <summary>多策略版本（Boss 用）</summary>
    public EnemyAttackState(EnemyActorData actorData, List<IEnemyAttackPattern> patterns)
    {
        _actorData = actorData;
        _patterns = patterns;
    }

    public void Enter()
    {
        IsFinished = false;
        _actorData.Rigidbody.velocity = Vector2.zero;
        _actorData.LastAttackTime = Time.time;
        _currentPattern = PickPattern();
        _currentPattern.Enter(_actorData);

        // 動畫長度決定攻擊持續時間，結束時透過 Animation Event 呼叫 OnAttackAnimationEnd()
    }

    public void Update(float deltaTime)
    {
        _currentPattern?.Update(deltaTime);
        if (_currentPattern != null && _currentPattern.IsFinished) IsFinished = true;
    }
    public void FixedUpdate(float fixedDeltaTime)
    {
        _currentPattern?.FixedUpdate(fixedDeltaTime);
    }

    public void Exit()
    {
        _currentPattern?.Exit();
    }
    public void ForceExit()
    {
        _currentPattern?.Exit(); // 冷卻在 Exit 裡記錄
        IsFinished = true;
    }

    protected virtual IEnemyAttackPattern PickPattern()
    {
        // 優先挑現在 CanActivate 的
        for (int i = 0; i < _patterns.Count; i++)
        {
            int idx = (_LastPatternIndex + i) % _patterns.Count;
            if (_patterns[idx].CanActivate(_actorData))
            {
                _LastPatternIndex = idx + 1;
                return _patterns[idx];
            }
        }
        // 全部冷卻中也不該進入 AttackState，但防呆
        return _patterns[_LastPatternIndex++ % _patterns.Count];
    }
    public bool HasReadyPattern()
    {
        for (int i = 0; i < _patterns.Count; i++)
            if (_patterns[i].CanActivate(_actorData)) return true;
        return false;
    }

}
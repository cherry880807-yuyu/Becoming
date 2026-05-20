using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationSystem
{
    private readonly Animator _animator;

    // ── Hash 全部靜態快取，打錯字編譯期不會報錯但至少集中管理 ──
    private static readonly int HashIdle    = Animator.StringToHash("Idle");
    private static readonly int HashMove    = Animator.StringToHash("Move");

    // Trigger
    private static readonly int TriggerHurt = Animator.StringToHash("Hurt");
    private static readonly int TriggerDead = Animator.StringToHash("Dead");

    // float
    private static readonly int ParamSpeed  = Animator.StringToHash("Speed");

    public EnemyAnimationSystem(Animator animator) => _animator = animator;

    // ── 基礎狀態 ─────────────────────────────────────────
    public void PlayIdle()  => _animator.CrossFade(HashIdle, 0.1f);
    public void PlayMove(float speed)
    {
        _animator.SetFloat(ParamSpeed, speed);
        _animator.CrossFade(HashMove, 0.1f);
    }
    public void PlayHurt()  => _animator.SetTrigger(TriggerHurt);
    public void PlayDead()  => _animator.SetTrigger(TriggerDead);


}
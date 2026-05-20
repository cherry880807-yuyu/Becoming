using System;
using UnityEngine;

/// <summary>
/// 所有 Brain（Player/Enemy）的基底
/// 單一狀態來源：HP 只在這裡管
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public abstract class BaseBrain : MonoBehaviour
{
    // ── 對外唯讀，避免外部亂改 ──────────────────────────
    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }
    public int CurrentShield { get; private set; }
    public bool IsAlive => CurrentHP > 0;

    // ── Events（用 event 關鍵字，防止外部 = 覆蓋） ──────
    public event Action<int> OnHPChanged;   // 傳當前 HP
    public event Action<int> OnDamageTaken; // 傳最終傷害值
    public event Action OnDeath;

    // ── 快取 ────────────────────────────────────────────
    private static readonly WaitForSeconds _hitFlashWait = new WaitForSeconds(0.1f);

    // ────────────────────────────────────────────────────
    protected virtual void Awake() => Init();

    protected abstract void Init();

    // ────────────────────────────────
    protected void SetMaxHP(int max)
    {
        MaxHP = max;
        CurrentHP = max;
    }

    protected void SetShield(int shield) => CurrentShield = shield;

    public void ApplyDamage(int rawDamage, Vector2 knockbackDir, float knockbackForce)
    {
        if (!IsAlive) return;

        int finalDamage = rawDamage;

        // Shield 吸收
        if (CurrentShield > 0)
        {
            int absorbed = Mathf.Min(CurrentShield, finalDamage);
            CurrentShield -= absorbed;
            finalDamage -= absorbed;
        }

        CurrentHP = Mathf.Max(CurrentHP - finalDamage, 0);

        OnDamageTaken?.Invoke(finalDamage);
        OnHPChanged?.Invoke(CurrentHP);

        OnApplyKnockback(knockbackDir, knockbackForce);

        if (CurrentHP <= 0)
            HandleDeath();
    }

    public void ApplyHeal(int amount)
    {
        if (!IsAlive) return;
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
        OnHPChanged?.Invoke(CurrentHP);
    }

    protected virtual void OnApplyKnockback(Vector2 dir, float force) { }

    protected virtual void HandleDeath()
    {
        OnDeath?.Invoke();
    }

    protected bool CheckGrounded(Collider2D col)
    {
        Bounds b = col.bounds;
        return Physics2D.BoxCast(
            b.center,
            new Vector2(b.size.x * 0.9f, b.size.y),
            0f,
            Vector2.down,
            0.1f,
            LayerMask.GetMask("Ground")
        ).collider != null;
    }
}
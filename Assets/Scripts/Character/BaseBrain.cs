using System;
using UnityEngine;

/// <summary>
/// 所有 Brain（Player/Enemy）的基底
/// 單一狀態來源：HP 只在這裡管
/// </summary>
public abstract class BaseBrain : MonoBehaviour
{
    [Header("Hitbox")]
    [SerializeField] protected HitBox2D _Hitbox;
    [Header("Hurtbox")]
    [SerializeField] protected HurtBox2D _Hurtbox;
    // ── 對外唯讀，避免外部亂改 ──────────────────────────
    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }
    public int CurrentShield { get; private set; }

    public bool IsAlive => CurrentHP > 0;


    // ── Events（用 event 關鍵字，防止外部 = 覆蓋） ──────
    public event Action<int> OnHPChanged;   // 傳當前 HP


    // ── 快取 ────────────────────────────────────────────
    private static readonly WaitForSeconds _hitFlashWait = new WaitForSeconds(0.1f);

    // ────────────────────────────────────────────────────
    protected virtual void Awake()
    {

        _Hitbox = GetComponentInChildren<HitBox2D>();
        _Hurtbox = GetComponentInChildren<HurtBox2D>();
    }
    protected virtual void Start() { }
    protected virtual void OnEnable()
    {
        _Hitbox.onHit += OnHit;
    }
    protected virtual void OnDisable()
    {
        _Hitbox.onHit -= OnHit;
    }


    // ────────────────────────────────
    protected void SetMaxHP(int max)
    {
        MaxHP = max;
        CurrentHP = max;
    }

    protected void SetShield(int shield)
    {
        CurrentShield = shield;
    }
    // ────────────────────────────────

    public void OnHitboxOpen() => _Hitbox.Activate();
    public void OnHitboxClose() => _Hitbox.Deactivate();

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
        Debug.Log($"{name} Get {finalDamage} damage!");
        CurrentHP = Mathf.Max(CurrentHP - finalDamage, 0);

        OnHPChanged?.Invoke(CurrentHP);
        OnApplyKnockback(knockbackDir, knockbackForce);

        EventBus.Publish(new DamageDealtEvent
        {
            Damage = finalDamage,
            WorldPosition = GetDamageTextPosition()
        });

        if (!IsAlive) HandleDeath();
    }

    public void ApplyHeal(int amount)
    {

        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
        OnHPChanged?.Invoke(CurrentHP);
        EventBus.Publish(new HealEvent
        {
            HealAmount = amount,
            WorldPosition = GetDamageTextPosition()
        });

    }
    public void Respawn(int restoreHP)
    {
        CurrentHP = Mathf.Clamp(restoreHP, 1, MaxHP);
        OnHPChanged?.Invoke(CurrentHP);
    }
    // ────────────────────────────────
    protected virtual void OnHit(IDamageable damageable, Vector2 knockDir)
    {

    }
    protected abstract void OnApplyKnockback(Vector2 dir, float force);

    protected virtual void HandleDeath()
    {

    }
    //--------------------------------------
    protected virtual Vector3 GetDamageTextPosition() => transform.position + Vector3.up * 1f;


}
using System;
using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(PlayerInputHandler))]
public class PlayerBrain : BaseBrain, IDamageable
{
    [Header("Data")]
    [SerializeField] private CharacterDataSO characterData;
    [SerializeField] private MovementDataSO movementData;
    [SerializeField] private DodgeDataSO dashData;
    [SerializeField] private StaminaDataSO staminaData;

    [Header("WeaponData")]
    [SerializeField] private WeaponDataSO weapon;
    [SerializeField] private WeaponDataSO testweapon1;
    [SerializeField] private WeaponDataSO testweapon2;

    // ────────────────────────────────────────────────────
    public PlayerActorData PlayerActorData { get; private set; }
    private PlayerInputHandler _input;

    // ── IsGrounded 快取（每幀只算一次）─────────────────────
    private bool _isGrounded;
    private bool _isDropping;
    private int _lastGroundedFrame = -1;
    private bool hasDeathProtection;
    private bool deathProtectionAvailable;
    private float deathProtectionHealPercent;

    // ────────────────────────────────────────────────────

    protected override void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
        SetMaxHP(characterData.maxHP);
        SetShield(characterData.maxShield);
        BuildActorData();
        if (PlayerLocator.IsInitialized)
            PlayerLocator.Instance.Register(transform);
        PublishHealthChanged();
        PublishDeathProtectionState();
    }
    protected override void Start()
    {
        if (PlayerLocator.IsInitialized && PlayerLocator.Instance.PlayerBrain != this)
            PlayerLocator.Instance.Register(transform);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _input.OnDashPressed += HandleDash;
        _input.OnAttackPressed += HandleAttack;
        _input.OnAttackReleased += HandleAttackReleased;
        _input.OnJumpPressed += HandleJump;
        _input.OnDownPressed += HandleDown;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _input.OnDashPressed -= HandleDash;
        _input.OnAttackPressed -= HandleAttack;
        _input.OnAttackReleased -= HandleAttackReleased;
        _input.OnJumpPressed -= HandleJump;
        _input.OnDownPressed -= HandleDown;
        PlayerActorData?.CombatSystem?.Dispose();

    }
    public void Oestroy()
    {
        PlayerLocator.Instance.Unregister(transform);
    }

    // ── Build ─────────────────────────────────────────────
    private void BuildActorData()
    {
        PlayerActorData = new PlayerActorData
        (
            GetComponent<Rigidbody2D>(),
            GetComponent<Animator>(),
            transform.GetChild(0).GetComponent<Collider2D>()
        );
        PlayerActorData.MovementSystem = new MovementSystem(PlayerActorData, new PlayerMovement_TypeA(movementData.moveSpeed, movementData.sprintSpeed));
        PlayerActorData.JumpSystem = new JumpSystem(PlayerActorData, movementData.baseAirJumps, movementData.jumpForce);
        PlayerActorData.AnimationSystem = new AnimationSystem(PlayerActorData.Animator);
        PlayerActorData.DodgeSystem = new DodgeSystem(new PlayerDodge_TypeA(dashData), this);
        PlayerActorData.PlayerNormalAttackInputSystem = new PlayerNormalAttackInputSystem(PlayerActorData, null);
        PlayerActorData.StaminaSystem = new StaminaSystem(staminaData);
        PlayerActorData.SkillSystem = new SkillSystem();
        PlayerActorData.SkillInputSystem = new PlayerSkillInputSystem(this, PlayerActorData, _input);
        PlayerActorData.CombatSystem = new PlayerCombatSystem(
            PlayerActorData.PlayerNormalAttackInputSystem,
            PlayerActorData.SkillInputSystem,
            PlayerActorData.SkillSystem);
        PlayerActorData.WeaponInventorySystem = new PlayerWeaponInventorySystem(this, PlayerActorData.CombatSystem, weapon);

        _Hurtbox.Register(PlayerActorData.DodgeSystem);
    }

    // ── Unity Loop ────────────────────────────────────────
    private void Update()
    {
        // IsGrounded 快取：同一幀只做一次 BoxCast
        _isGrounded = GetGroundedCached();
        if (_isGrounded) PlayerActorData.JumpSystem.ResetAirJumps();
        PlayerActorData.StaminaSystem.Regen(Time.deltaTime);
        PlayerActorData.CombatSystem.Evaluate();
        HandleFacing();
        UpdateAnimationState();
    }
    public void TestUnLockWeapon()
    {
        PlayerActorData.WeaponInventorySystem.UnlockWeapon(weapon);
        PlayerActorData.WeaponInventorySystem.UnlockWeapon(testweapon1);
        PlayerActorData.WeaponInventorySystem.UnlockWeapon(testweapon2);
    }

    private void FixedUpdate()
    {
        PlayerActorData.MovementSystem.CheckGrounded_ByBoxCast();
        PlayerActorData.MovementSystem.SetSprint(_input.IsSprinting);
        PlayerActorData.MovementSystem.Move(PlayerActorData.Rigidbody, _input.MoveInput);
        PublishSprintDistance();

        HandleAirPhysics();
    }

    // ── Input Handlers ────────────────────────────────────
    private void PublishSprintDistance()
    {
        if (!_input.IsSprinting || Mathf.Abs(_input.MoveInput.x) <= 0.01f) return;

        float distance = Mathf.Abs(PlayerActorData.Rigidbody.velocity.x) * Time.fixedDeltaTime;
        if (distance <= 0f) return;

        EventBus.Publish(new PlayerSprintDistanceEvent { distance = distance });
    }

    private void HandleDash()
    {
        if (PlayerActorData.CombatSystem.TryCancelSkillCast())
            return;

        float dodgeCost = PlayerActorData.DodgeSystem.GetModifiedDodgeCost(dashData.dodgeCost);
        if (!PlayerActorData.StaminaSystem.CanUse(dodgeCost)) return;
        PlayerActorData.StaminaSystem.Consume(dodgeCost);
        PlayerActorData.DodgeSystem.Execute(PlayerActorData.Rigidbody, _input.LastMoveDir);

        EventBus.Publish(new DashEvent
        {
            WorldPosition = new Vector3(transform.position.x, PlayerActorData.Collider.bounds.min.y, transform.position.z),
            FacingRight = PlayerActorData.Facing
        });
    }

    private void HandleAttack()
    {
        PlayerActorData.CombatSystem.HandleAttackPressed(_isGrounded);
    }

    private void HandleAttackReleased(float holdDuration)
    {
        PlayerActorData.CombatSystem.HandleAttackReleased(holdDuration);
    }

    public void OnSkillCastFrame()
    {
        PlayerActorData.CombatSystem.TryExecutePendingSkillCast();
    }

    private void HandleJump()
    {
        if (!PlayerActorData.JumpSystem.OnJumpInput()) return;
        EventBus.Publish(new JumpEvent());
        EventBus.Publish(new ResetAttackComboEvent());
    }
    private void HandleDown()
    {
        if (!_isGrounded || _isDropping || PlayerActorData.CombatSystem.IsAttacking) return;

        StartCoroutine(DropPlatform_TimerBased());
    }

    private IEnumerator DropPlatform_PhysicsBased()
    {
        if (_isDropping) yield break;
        _isDropping = true;

        Collider2D currentPlatform = PlayerActorData.MovementSystem.GetCurrentPlatform(PlayerActorData.Collider);

        if (currentPlatform == null)
        {
            _isDropping = false;
            yield break;
        }

        Physics2D.IgnoreCollision(
            PlayerActorData.Collider,
            currentPlatform,
            true
        );

        PlayerActorData.Rigidbody.velocity = new Vector2(
            PlayerActorData.Rigidbody.velocity.x,
            -movementData.downPlatformForce
        );

        yield return new WaitUntil(() =>
            PlayerActorData.Collider.bounds.max.y <
            currentPlatform.bounds.min.y
        );

        Physics2D.IgnoreCollision(
            PlayerActorData.Collider,
            currentPlatform,
            false
        );

        _isDropping = false;
    }

    private IEnumerator DropPlatform_TimerBased()
    {
        _isDropping = true;
        Collider2D currentPlatform = PlayerActorData.MovementSystem.GetCurrentPlatform(PlayerActorData.Collider);

        if (currentPlatform == null)
        {
            _isDropping = false;
            yield break;
        }

        Physics2D.IgnoreCollision(
            PlayerActorData.Collider,
            currentPlatform,
            true
        );

        PlayerActorData.Rigidbody.velocity = new Vector2(
            PlayerActorData.Rigidbody.velocity.x,
            -movementData.downPlatformForce
        );

        yield return new WaitForSecondsRealtime(0.1f);

        Physics2D.IgnoreCollision(
            PlayerActorData.Collider,
            currentPlatform,
            false
        );

        _isDropping = false;
    }

    // ── IDamageable ───────────────────────────────────────
    protected override void OnHit(IDamageable damageable, Vector2 knockDir)
    {
        if (damageable == null) return;

        bool IsEnemyAlive = false;
        if (damageable is BaseBrain baseBrain) IsEnemyAlive = baseBrain.IsAlive;
        float angleRad = PlayerActorData.PlayerNormalAttackInputSystem.CurrentStep.KnockbackAngle * Mathf.Deg2Rad;

        EventBus.Publish(new PlayerDamageDealtEvent
        {
            Damage = PlayerActorData.PlayerNormalAttackInputSystem.FinalDamage,
            WorldPosition = transform.position
        });

        damageable.TakeDamage(new HitConfig(
            PlayerActorData.PlayerNormalAttackInputSystem.FinalDamage,
            new Vector2(Mathf.Cos(angleRad) * PlayerActorData.Facing.x, Mathf.Sin(angleRad)),
            PlayerActorData.PlayerNormalAttackInputSystem.CurrentStep.knockbackForce,
            transform.position,
            PlayerActorData.PlayerNormalAttackInputSystem.CurrentStep.hitStopTime));

        if (IsEnemyAlive)
        {
            EventBus.Publish(new AttackEnemyEvent
            {
                hitTime = PlayerActorData.PlayerNormalAttackInputSystem.CurrentStep.hitStopTime
            });
        }
        else
        {
            Debug.Log("鞭打屍體!");
        }
    }

    protected override int GetHitDamagePreview(IDamageable target)
    {
        return PlayerActorData?.PlayerNormalAttackInputSystem?.FinalDamage ?? 0;
    }

    public void TakeDamage(HitConfig hitConfig)
    {
        if (!IsAlive) return;
        if (TryConsumeDeathProtection(hitConfig)) return;

        FaceTowards(hitConfig.AttackerPosition);
        PlayerActorData.AnimationSystem.PlayHit();
        ApplyDamage(hitConfig.Damage, hitConfig.HitDirection, hitConfig.KnockbackForce);
        PublishHealthChanged();
    }

    protected override void OnApplyKnockback(Vector2 dir, float force)
    {
        // Player 是否需要擊退由這裡控制
        // 例如：無敵幀期間不受擊退
        if (PlayerActorData.DodgeSystem.IsDodging) return;
        PlayerActorData.Rigidbody.AddForce(dir * force, ForceMode2D.Impulse);
    }

    public void Heal(int amount)
    {
        ApplyHeal(amount);
        PublishHealthChanged();
    }

    // ── Death ─────────────────────────────────────────────
    public new void Respawn(int restoreHP)
    {
        base.Respawn(restoreHP);
        ResetDeathProtectionForNewLife();
        PublishHealthChanged();
    }

    public void EnableDeathProtection(float healPercent)//解鎖
    {
        hasDeathProtection = true;
        deathProtectionHealPercent = healPercent;
        ResetDeathProtectionForNewLife();
        PublishDeathProtectionState();
    }

    public void DisableDeathProtection()//尚未解鎖
    {
        hasDeathProtection = false;
        deathProtectionAvailable = false;
        deathProtectionHealPercent = 0f;
        PublishDeathProtectionState();
    }

    private bool TryConsumeDeathProtection(HitConfig hitConfig)
    {
        if (!hasDeathProtection || !deathProtectionAvailable)
            return false;

        int finalDamage = Mathf.Max(0, hitConfig.Damage - CurrentShield);
        if (CurrentHP > finalDamage)
            return false;

        FaceTowards(hitConfig.AttackerPosition);
        PlayerActorData.AnimationSystem.PlayHit();
        deathProtectionAvailable = false;
        ApplyDamage(hitConfig.Damage, hitConfig.HitDirection, hitConfig.KnockbackForce, false);

        int healAmount = Mathf.CeilToInt(MaxHP * deathProtectionHealPercent);
        if (healAmount > 0)
            ApplyHeal(healAmount);

        PublishHealthChanged();
        PublishDeathProtectionState();
        return true;
    }

    private void ResetDeathProtectionForNewLife()
    {
        if (!hasDeathProtection) return;
        deathProtectionAvailable = true;
        PublishDeathProtectionState();
    }

    private void PublishDeathProtectionState()
    {
        EventBus.Publish(new DeathProtectionStateChangedEvent
        {
            isUnlocked = hasDeathProtection,
            isAvailable = hasDeathProtection && deathProtectionAvailable
        });
    }

    private void PublishHealthChanged()
    {
        EventBus.Publish(new PlayerHealthChangedEvent
        {
            currentHP = CurrentHP,
            maxHP = MaxHP,
            healthPercent = MaxHP > 0 ? (float)CurrentHP / MaxHP : 0f
        });
    }

    protected override void HandleDeath()
    {
        base.HandleDeath(); // 觸發 OnDeath event
        enabled = false;
        EventBus.Publish(new PlayerDiedEvent
        {
            WorldPosition = transform.position
        });
        // TODO: 接 Event Bus → GameOverSystem / MutationSystem（能力清除）
        // GameEventBus.Emit(new PlayerDiedEvent());
    }

    // ── Player內部工具 ──────────────────────────────────────────
    private bool GetGroundedCached()
    {
        if (_lastGroundedFrame == Time.frameCount) return _isGrounded;
        _lastGroundedFrame = Time.frameCount;
        return PlayerActorData.MovementSystem.IsGrounded; // BaseBrain 的方法
    }

    private void HandleFacing()
    {
        if (Mathf.Approximately(_input.MoveInput.x, 0f)) return;

        PlayerActorData.Facing = _input.MoveInput.normalized;
        // 避免每幀 new Vector3：只在方向改變時設
        float scaleX = PlayerActorData.Facing.x < 0 ? -1f : 1f;
        if (!Mathf.Approximately(transform.GetChild(0).localScale.x, scaleX)) transform.GetChild(0).localScale = new Vector3(scaleX, 1f, 1f);
    }
    private void FaceTowards(Vector2 targetPosition)
    {
        float dirX = targetPosition.x - transform.position.x;
        if (Mathf.Approximately(dirX, 0f)) return;

        float scaleX = dirX > 0f ? 1f : -1f;
        PlayerActorData.Facing = new Vector2(scaleX, 0f);
        if (!Mathf.Approximately(transform.GetChild(0).localScale.x, scaleX)) transform.GetChild(0).localScale = new Vector3(scaleX, 1f, 1f);
    }

    private void UpdateAnimationState()
    {
        PlayerActorData.AnimationSystem?.SetMovementState(new MovementState
        {
            velocity = PlayerActorData.Rigidbody.velocity,
            isGrounded = _isGrounded,
            isDodging = PlayerActorData.DodgeSystem.IsDodging,
            isSprinting = _input.IsSprinting
        });
    }

    private void HandleAirPhysics()
    {
        if (PlayerActorData.MovementSystem.IsGrounded) return;

        var rb = PlayerActorData.Rigidbody;
        float extraGravity = rb.velocity.y < 0
            ? movementData.gravity * movementData.fallMultiplier
            : movementData.gravity;

        rb.velocity += Vector2.up * (extraGravity * Time.fixedDeltaTime);
    }
}

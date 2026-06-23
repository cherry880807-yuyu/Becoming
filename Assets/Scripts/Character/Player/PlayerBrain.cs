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
    [SerializeField] private DashDataSO dashData;
    [SerializeField] private StaminaDataSO staminaData;

    [Header("WeaponData")]
    [SerializeField] private WeaponDataSO weapon;

    // ────────────────────────────────────────────────────
    public PlayerActorData PlayerActorData { get; private set; }
    private PlayerInputHandler _input;

    // ── IsGrounded 快取（每幀只算一次）─────────────────────
    private bool _isGrounded;
    private bool _isDropping;
    private int _lastGroundedFrame = -1;

    // ────────────────────────────────────────────────────

    protected override void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
        SetMaxHP(characterData.maxHP);
        SetShield(characterData.maxShield);
        BuildActorData();
    }
    protected override void Start()
    {
        PlayerLocator.Instance.Register(transform);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _input.OnDashPressed += HandleDash;
        _input.OnAttackPressed += HandleAttack;
        _input.OnJumpPressed += HandleJump;
        _input.OnDownPressed += HandleDown;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _input.OnDashPressed -= HandleDash;
        _input.OnAttackPressed -= HandleAttack;
        _input.OnJumpPressed -= HandleJump;
        _input.OnDownPressed -= HandleDown;
        PlayerActorData?.AttackSystem?.Dispose();

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
        PlayerActorData.DashSystem = new DashSystem(new PlayerDash_TypeA(dashData), this);
        PlayerActorData.AttackSystem = new AttackSystem(PlayerActorData, weapon);
        PlayerActorData.AnimationSystem = new AnimationSystem(PlayerActorData.Animator);
        PlayerActorData.StaminaSystem = new StaminaSystem(staminaData);

        _Hurtbox.Register(PlayerActorData.DashSystem);
    }

    // ── Unity Loop ────────────────────────────────────────
    private void Update()
    {
        // IsGrounded 快取：同一幀只做一次 BoxCast
        _isGrounded = GetGroundedCached();
        if (_isGrounded) PlayerActorData.JumpSystem.ResetAirJumps();
        PlayerActorData.StaminaSystem.Regen(Time.deltaTime);
        PlayerActorData.AttackSystem.EvaluateCombo();
        HandleFacing();
        UpdateAnimationState();
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

        EventBus.Publish(new PlayerSprintDistanceEvent {distance = distance});
    }

    private void HandleDash()
    {
        float dashCost = PlayerActorData.DashSystem.GetModifiedDashCost(dashData.dashCost);
        if (!PlayerActorData.StaminaSystem.CanUse(dashCost)) return;
        PlayerActorData.StaminaSystem.Consume(dashCost);
        PlayerActorData.DashSystem.Execute(PlayerActorData.Rigidbody, _input.LastMoveDir);

        EventBus.Publish(new DashEvent
        {
            WorldPosition = new Vector3( transform.position.x,PlayerActorData.Collider.bounds.min.y,transform.position.z),
            FacingRight = PlayerActorData.Facing
        });
    }

    private void HandleAttack() => PlayerActorData.AttackSystem.OnAttackInput();

    private void HandleJump()
    {
        if (!PlayerActorData.JumpSystem.OnJumpInput()) return;
        EventBus.Publish(new JumpEvent());
        EventBus.Publish(new ResetAttackComboEvent());
    }
    private void HandleDown()
    {
        if (!_isGrounded || _isDropping || PlayerActorData.AttackSystem.IsAttacking) return;

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
        float angleRad = PlayerActorData.AttackSystem.CurrentStep.KnockbackAngle * Mathf.Deg2Rad;

        EventBus.Publish(new PlayerDamageDealtEvent
        {
            Damage = PlayerActorData.AttackSystem.FinalDamage,
            WorldPosition = transform.position
        });

        damageable.TakeDamage(new DamageInfo
        {
            damage = PlayerActorData.AttackSystem.FinalDamage,
            knockbackForce = PlayerActorData.AttackSystem.CurrentStep.knockbackForce,
            hitDirection = new Vector2(Mathf.Cos(angleRad) * PlayerActorData.Facing.x, Mathf.Sin(angleRad)),
            attackerPosition = (Vector2)transform.position
        });

        if (IsEnemyAlive)
        {
            EventBus.Publish(new AttackEnemyEvent
            {
                hitTime = PlayerActorData.AttackSystem.CurrentStep.hitStopTime
            });
        }
        else
        {
            Debug.Log("鞭打屍體!");
        }
    }

    public void TakeDamage(DamageInfo info)
    {
        if (!IsAlive) return;
        FaceTowards(info.attackerPosition);
        PlayerActorData.AnimationSystem.PlayHit();
        ApplyDamage(info.damage, info.hitDirection, info.knockbackForce);
    }

    protected override void OnApplyKnockback(Vector2 dir, float force)
    {
        // Player 是否需要擊退由這裡控制
        // 例如：無敵幀期間不受擊退
        if (PlayerActorData.DashSystem.IsDashing) return;
        PlayerActorData.Rigidbody.AddForce(dir * force, ForceMode2D.Impulse);
    }

    public void Heal(int amount) => ApplyHeal(amount);

    // ── Death ─────────────────────────────────────────────
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
            isDashing = PlayerActorData.DashSystem.IsDashing,
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

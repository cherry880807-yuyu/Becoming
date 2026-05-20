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

    [Header("Weapon")]
    [SerializeField] private Weapon wp;

    public PlayerActorData PlayerActorData { get; private set; }

    private PlayerInputHandler _input;

    // ── IsGrounded 快取（每幀只算一次）─────────────────────
    private bool _isGrounded;
    private int _lastGroundedFrame = -1;

    // ────────────────────────────────────────────────────
    protected override void Init()
    {
        // 走 BaseBrain 統一入口，不直接改 CurrentHP
        SetMaxHP(characterData.maxHP);
        SetShield(characterData.maxShield);
    }

    protected override void Awake()
    {
        base.Awake();
        _input = GetComponent<PlayerInputHandler>();
        PlayerLocator.Instance.Register(transform);
    }

    private void Start()
    {
        
        BuildActorData();
    }

    private void OnEnable()
    {
        _input.OnDashPressed += HandleDash;
        _input.OnAttackPressed += HandleAttack;
        _input.OnJumpPressed += HandleJump;
        OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        _input.OnDashPressed -= HandleDash;
        _input.OnAttackPressed -= HandleAttack;
        _input.OnJumpPressed -= HandleJump;
        OnDeath -= HandleDeath;
    }

    // ── Build ─────────────────────────────────────────────
    private void BuildActorData()
    {
        PlayerActorData = new PlayerActorData
        (
            GetComponent<Rigidbody2D>(),
            GetComponent<Animator>(),
            GetComponent<Collider2D>(),
            GetComponent<SpriteRenderer>()
        );

        PlayerActorData.MovementSystem = new MovementSystem(new PlayerMovement_TypeA(movementData.moveSpeed, movementData.sprintSpeed));
        PlayerActorData.DashSystem = new DashSystem(new PlayerDash_TypeA(dashData), this);
        PlayerActorData.AttackSystem = new AttackSystem(new PlayerBasicAttackBehavior(wp.ComboDataSO), PlayerActorData);
        PlayerActorData.AnimationSystem = new AnimationSystem(PlayerActorData.Animator);
        PlayerActorData.StaminaSystem = new StaminaSystem(staminaData);
    }

    // ── Unity Loop ────────────────────────────────────────
    private void Update()
    {
        // IsGrounded 快取：同一幀只做一次 BoxCast
        _isGrounded = GetGroundedCached();

        PlayerActorData.StaminaSystem.Regen(Time.deltaTime);
        HandleFacing();
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        PlayerActorData.MovementSystem.Move(PlayerActorData.Rigidbody, _input.MoveInput);
        HandleAirPhysics();
    }

    // ── Input Handlers ────────────────────────────────────
    private void HandleDash()
    {
        if (!PlayerActorData.StaminaSystem.CanUse(dashData.dashCost)) return;
        PlayerActorData.StaminaSystem.Consume(dashData.dashCost);
        PlayerActorData.DashSystem.Execute(PlayerActorData.Rigidbody, _input.LastMoveDir);
    }

    private void HandleAttack() => PlayerActorData.AttackSystem.Attack();

    private void HandleJump()
    {
        if (!_isGrounded) return;
        var rb = PlayerActorData.Rigidbody;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * movementData.jumpForce, ForceMode2D.Impulse);
    }

    // ── IDamageable ───────────────────────────────────────
    public void TakeDamage(DamageInfo info)
    {
        if (!IsAlive) return;

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

    public void Attack()
    {
        wp.SetStep(PlayerActorData.AttackSystem.CurrentStep);
        wp.DoHitCheck(PlayerActorData.AttackSystem.FinalDamage);
    }

    // ── Death ─────────────────────────────────────────────
    protected override void HandleDeath()
    {
        base.HandleDeath(); // 觸發 OnDeath event
        enabled = false;
        // TODO: 接 Event Bus → GameOverSystem / MutationSystem（能力清除）
        // GameEventBus.Emit(new PlayerDiedEvent());
    }

    // ── Player內部工具 ──────────────────────────────────────────
    private bool GetGroundedCached()
    {
        if (_lastGroundedFrame == Time.frameCount) return _isGrounded;
        _lastGroundedFrame = Time.frameCount;
        return CheckGrounded(PlayerActorData.Collider); // BaseBrain 的方法
    }

    private void HandleFacing()
    {
        if (_input.MoveInput == Vector2.zero) return;

        PlayerActorData.Facing = _input.MoveInput.normalized;
        // 避免每幀 new Vector3：只在方向改變時設
        float scaleX = PlayerActorData.Facing.x < 0 ? -1f : 1f;
        if (!Mathf.Approximately(transform.localScale.x, scaleX)) transform.localScale = new Vector3(scaleX, 1f, 1f);
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
        // FixedUpdate 裡也用 CheckGrounded，但這是物理幀，跟 Update 分開是正確的
        if (CheckGrounded(PlayerActorData.Collider)) return;

        var rb = PlayerActorData.Rigidbody;
        float extraGravity = rb.velocity.y < 0
            ? movementData.gravity * movementData.fallMultiplier
            : movementData.gravity;

        rb.velocity += Vector2.up * (extraGravity * Time.fixedDeltaTime);
    }
}
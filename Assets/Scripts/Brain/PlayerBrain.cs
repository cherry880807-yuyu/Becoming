using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBrain : BaseBrain
{
    [Header("Data")]
    [SerializeField] private MovementData movementData;
    [SerializeField] private DashData dashData;
    [SerializeField] private StaminaData staminaData;

    public ActorData ActorData { get; private set; }

    private PlayerInputActions input;

    private Vector2 moveInput;
    private Vector2 lastMoveDir;

    private bool isSprinting;
    // =========================
    // LIFE
    // =========================
    private void Awake()
    {
        input = new PlayerInputActions();
        ActorData = new ActorData();
        ActorData.Rigidbody = GetComponent<Rigidbody2D>();
        ActorData.Animator = GetComponent<Animator>();
        ActorData.Collider = GetComponent<Collider2D>();
        ActorData.MovementSystem = new MovementSystem(new PlayerMovement_TypeA(movementData.moveSpeed, movementData.sprintSpeed));
        ActorData.DashSystem = new DashSystem(new PlayerDash_TypeA(dashData), this);
        ActorData.AttackSystem = new AttackSystem(new PlayerAttack_TypeA());
        ActorData.AnimationSystem = new AnimationSystem(new BasicAnimation(ActorData.Animator));
        ActorData.StaminaSystem = new StaminaSystem(staminaData);

    }
    private void OnEnable()
    {
        input.Enable();

        input.Player.Dash.performed += OnDash;
        input.Player.Attack.performed += OnAttack;
        input.Player.Jump.performed += OnJump;
    }
    private void OnDisable()
    {
        input.Player.Dash.performed -= OnDash;
        input.Player.Attack.performed -= OnAttack;
        input.Player.Jump.performed -= OnJump;

        input.Disable();
    }
    protected override void Update()
    {
        ReadInput();
        HandleMovementState();
        HandleStamina();

        //ActorData.AnimationSystem?.SetState(BuildState());
    }
    private void FixedUpdate()
    {
        ActorData.MovementSystem.Move(ActorData.Rigidbody, moveInput);
    }
    // =========================
    // INPUT
    // =========================
    private void ReadInput()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>();
        if (moveInput != Vector2.zero) lastMoveDir = moveInput.normalized;
        isSprinting = input.Player.Sprint.IsPressed();
    }
    // =========================
    // ACTION
    // =========================

    private void HandleMovementState()
    {
        ActorData.MovementSystem.SetSprint(isSprinting);
    }
    private void HandleStamina()
    {
        ActorData.StaminaSystem?.Regen(Time.deltaTime);
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (!ActorData.StaminaSystem.CanUse(dashData.dashCost)) return;
        ActorData.StaminaSystem.Consume(dashData.dashCost);
        ActorData.DashSystem.Execute(ActorData.Rigidbody, lastMoveDir);
    }
    private void OnAttack(InputAction.CallbackContext ctx)
    {
        ActorData.AttackSystem.Attack();
    }
    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!IsGrounded()) return;
        ActorData.Rigidbody.velocity = new Vector2(ActorData.Rigidbody.velocity.x, 0f);
        ActorData.Rigidbody.AddForce(Vector2.up * movementData.jumpForce, ForceMode2D.Impulse);
    }


    // =========================
    // UTIL
    // =========================
    private bool IsGrounded()
    {
        Bounds bounds = ActorData.Collider.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(
            bounds.center,
            new Vector2(bounds.size.x * 0.9f, bounds.size.y),
            0f,
            Vector2.down,
            0.1f,
            LayerMask.GetMask("Ground")
        );
        return hit.collider != null;
    }

    private CharacterState BuildState()
    {
        return new CharacterState
        {
            velocity = ActorData.Rigidbody.velocity,
            isGrounded = IsGrounded(),
            isDashing = ActorData.DashSystem.IsDashing,
            isSprinting = isSprinting
        };
    }


}

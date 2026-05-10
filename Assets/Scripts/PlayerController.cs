using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private MovementData movementData;
    [SerializeField] private DashData dashData;
    [SerializeField] private StaminaData staminaData;

    [SerializeField] private LayerMask groundLayer;


    private MovementSystem movementSystem;
    private DashSystem dashSystem;
    private AttackSystem attackSystem;
    private AnimationSystem animationSystem;
    private CharacterState state;

    //Observer
    [SerializeField] private float stamina;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isInvincible;
    [SerializeField] private bool isSprinting;
    private Vector2 lastMoveDir = Vector2.right;
    //Input
    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    //Component
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D playerCollider;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();

        movementSystem = new MovementSystem(new PlayerMovement_TypeA(movementData.moveSpeed, movementData.sprintSpeed));
        dashSystem = new DashSystem(new PlayerDash_TypeA(dashData), this);
        attackSystem = new AttackSystem(new PlayerAttack_TypeA());
        //animationSystem = new AnimationSystem(new BasicAnimation(animator)

        stamina = staminaData.maxStamina;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += Jump;
        inputActions.Player.Attack.performed += Attack;
        inputActions.Player.Dash.performed += Dash;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= Jump;
        inputActions.Player.Attack.performed -= Attack;
        inputActions.Player.Dash.performed -= Dash;
        inputActions.Player.Disable();
    }

    private void Update()
    {
        RegenerateStamina();

        isSprinting = inputActions.Player.Sprint.IsPressed();
        movementSystem.SetSprint(isSprinting);
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        if (moveInput != Vector2.zero)lastMoveDir = moveInput.normalized;
        UpdateState();
    }

    private void FixedUpdate()
    {
        movementSystem.Move(rb, moveInput);
    }
    private void LateUpdate()
    {
        // animationSystem.Update(state);
    }


    //Action
    private void Jump(InputAction.CallbackContext context)
    {
        if (!IsGrounded()) return;
        Debug.Log("Jump!");
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * movementData.jumpForce, ForceMode2D.Impulse);
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (!canDash || isDashing) return;
        if (stamina < dashData.dashCost) return;

        canDash = false;
        isDashing = true;
        isInvincible = true;
        stamina -= dashData.dashCost;

        Vector2 dir = lastMoveDir;
        dashSystem.Execute(rb, dir);

        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashData.dashCooldown);

        isDashing = false;
        isInvincible = false;
        canDash = true;
    }

    private void RegenerateStamina()
    {
        if (isDashing) return;

        stamina += staminaData.regenRate * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0, staminaData.maxStamina);
    }
    private void Attack(InputAction.CallbackContext context)
    {
        attackSystem.Attack();
        //animator.SetTrigger("ATTACK1");
    }

    private bool IsGrounded()
    {
        Bounds bounds = playerCollider.bounds;

        RaycastHit2D hit = Physics2D.BoxCast(
            bounds.center,
            new Vector2(bounds.size.x * 0.9f, bounds.size.y),
            0f,
            Vector2.down,
            0.1f,
            groundLayer
        );
        return hit.collider != null;
    }
    private void UpdateState()
    {
        state.velocity = rb.velocity;
        state.isGrounded = IsGrounded();
        state.isDashing = isDashing;
        state.isSprinting = isSprinting;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, IPlayerSkillInputSource
{
    private const float DirectionCommandThreshold = 0.5f;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LastMoveDir { get; private set; } = Vector2.right;
    public bool IsSprinting { get; private set; }

    // 事件型 Input（按下瞬間，不適合 polling）
    public event Action OnDashPressed;
    public event Action OnAttackPressed;
    public event Action<float> OnAttackReleased;
    public event Action OnJumpPressed;
    public event Action OnDownPressed;

    private PlayerInputActions.PlayerActions _playerInput;
    private readonly PlayerCommandBuffer commandBuffer = new();
    private float _attackStartedTime = -1f;
    private bool _wasSprinting;
    private bool _wasHoldingUp;
    private bool _wasHoldingDown;
    private bool _wasHoldingLeft;
    private bool _wasHoldingRight;



    private void OnEnable()
    {
        _playerInput = InputManager.Instance.InputAction.Player;

        _playerInput.Dash.performed += HandleDashPerformed;
        _playerInput.Attack.started += HandleAttackStarted;
        _playerInput.Attack.performed += HandleAttackPerformed;
        _playerInput.Attack.canceled += HandleAttackCanceled;
        _playerInput.Jump.performed += HandleJumpPerformed;
        _playerInput.DownPlatform.performed += HandleDownPlatformPerformed;
    }

    private void OnDisable()
    {
        _playerInput.Dash.performed -= HandleDashPerformed;
        _playerInput.Attack.started -= HandleAttackStarted;
        _playerInput.Attack.performed -= HandleAttackPerformed;
        _playerInput.Attack.canceled -= HandleAttackCanceled;
        _playerInput.Jump.performed -= HandleJumpPerformed;
        _playerInput.DownPlatform.performed -= HandleDownPlatformPerformed;
    }

    private void Update()
    {
        MoveInput = _playerInput.Move.ReadValue<Vector2>();
        IsSprinting = _playerInput.Sprint.IsPressed();

        if (MoveInput != Vector2.zero) LastMoveDir = MoveInput.normalized;

        TrackDirectionCommands();
        TrackSprintCommand();
    }

    public bool WasCommandPressed(PlayerCommand command, float withinSeconds)
    {
        return commandBuffer.WasPressedRecently(command, withinSeconds);
    }

    public bool WasCommandReleased(PlayerCommand command, float withinSeconds)
    {
        return commandBuffer.WasReleasedRecently(command, withinSeconds);
    }

    public float GetCommandHoldDuration(PlayerCommand command)
    {
        return commandBuffer.GetHoldDuration(command);
    }

    public bool MatchPressedCommandSequence(float withinSeconds, params PlayerCommand[] sequence)
    {
        return commandBuffer.MatchPressedSequence(withinSeconds, sequence);
    }

    private void HandleDashPerformed(InputAction.CallbackContext _)
    {
        commandBuffer.PushPressed(PlayerCommand.Dash);
        OnDashPressed?.Invoke();
    }

    private void HandleAttackStarted(InputAction.CallbackContext _)
    {
        _attackStartedTime = Time.time;
        commandBuffer.PushPressed(PlayerCommand.Attack);
        OnAttackPressed?.Invoke();
    }

    private void HandleAttackPerformed(InputAction.CallbackContext _)
    {
    }

    private void HandleAttackCanceled(InputAction.CallbackContext _)
    {
        float holdDuration = _attackStartedTime >= 0f ? Time.time - _attackStartedTime : 0f;
        _attackStartedTime = -1f;
        commandBuffer.PushReleased(PlayerCommand.Attack);
        OnAttackReleased?.Invoke(holdDuration);
    }

    private void HandleJumpPerformed(InputAction.CallbackContext _)
    {
        commandBuffer.PushPressed(PlayerCommand.Jump);
        OnJumpPressed?.Invoke();
    }

    private void HandleDownPlatformPerformed(InputAction.CallbackContext _)
    {
        commandBuffer.PushPressed(PlayerCommand.Down);
        OnDownPressed?.Invoke();
    }

    private void TrackDirectionCommands()
    {
        TrackDirectionCommand(PlayerCommand.DirectionUp, MoveInput.y > DirectionCommandThreshold, ref _wasHoldingUp);
        TrackDirectionCommand(PlayerCommand.DirectionDown, MoveInput.y < -DirectionCommandThreshold, ref _wasHoldingDown);
        TrackDirectionCommand(PlayerCommand.DirectionLeft, MoveInput.x < -DirectionCommandThreshold, ref _wasHoldingLeft);
        TrackDirectionCommand(PlayerCommand.DirectionRight, MoveInput.x > DirectionCommandThreshold, ref _wasHoldingRight);
    }

    private void TrackDirectionCommand(PlayerCommand command, bool isHolding, ref bool wasHolding)
    {
        if (isHolding && !wasHolding)
            commandBuffer.PushPressed(command);
        else if (!isHolding && wasHolding)
            commandBuffer.PushReleased(command);

        wasHolding = isHolding;
    }

    private void TrackSprintCommand()
    {
        if (IsSprinting && !_wasSprinting)
            commandBuffer.PushPressed(PlayerCommand.Sprint);
        else if (!IsSprinting && _wasSprinting)
            commandBuffer.PushReleased(PlayerCommand.Sprint);

        _wasSprinting = IsSprinting;
    }
}

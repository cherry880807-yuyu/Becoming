using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LastMoveDir { get; private set; } = Vector2.right;
    public bool IsSprinting { get; private set; }

    // 事件型 Input（按下瞬間，不適合 polling）
    public event Action OnDashPressed;
    public event Action OnAttackPressed;
    public event Action OnJumpPressed;
    public event Action OnDownPressed;

    private PlayerInputActions.PlayerActions _playerInput;



    private void OnEnable()
    {
        _playerInput = InputManager.Instance.InputAction.Player;

        _playerInput.Dash.performed += HandleDashPerformed;
        _playerInput.Attack.performed += HandleAttackPerformed;
        _playerInput.Jump.performed += HandleJumpPerformed;
        _playerInput.DownPlatform.performed += HandleDownPlatformPerformed;
    }

    private void OnDisable()
    {
        _playerInput.Dash.performed -= HandleDashPerformed;
        _playerInput.Attack.performed -= HandleAttackPerformed;
        _playerInput.Jump.performed -= HandleJumpPerformed;
        _playerInput.DownPlatform.performed -= HandleDownPlatformPerformed;
    }

    private void Update()
    {
        MoveInput = _playerInput.Move.ReadValue<Vector2>();
        IsSprinting = _playerInput.Sprint.IsPressed();

        if (MoveInput != Vector2.zero) LastMoveDir = MoveInput.normalized;
    }
    private void HandleDashPerformed(InputAction.CallbackContext _) => OnDashPressed?.Invoke();
    private void HandleAttackPerformed(InputAction.CallbackContext _) => OnAttackPressed?.Invoke();
    private void HandleJumpPerformed(InputAction.CallbackContext _) => OnJumpPressed?.Invoke();
    private void HandleDownPlatformPerformed(InputAction.CallbackContext _) => OnDownPressed?.Invoke();

}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private PlayerInputActions _input;

    private void Awake() => _input = new PlayerInputActions();

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.Dash.performed += _ => OnDashPressed?.Invoke();
        _input.Player.Attack.performed += _ => OnAttackPressed?.Invoke();
        _input.Player.Jump.performed += _ => OnJumpPressed?.Invoke();
        _input.Player.DownPlatform.performed += _ => OnDownPressed?.Invoke();
    }

    private void OnDisable()
    {
        _input.Player.Dash.performed -= _ => OnDashPressed?.Invoke();
        _input.Player.Attack.performed -= _ => OnAttackPressed?.Invoke();
        _input.Player.Jump.performed -= _ => OnJumpPressed?.Invoke();
        _input.Player.DownPlatform.performed -= _ => OnDownPressed?.Invoke();
        _input.Disable();
    }

    private void Update()
    {
        MoveInput = _input.Player.Move.ReadValue<Vector2>();
        IsSprinting = _input.Player.Sprint.IsPressed();

        if (MoveInput != Vector2.zero) LastMoveDir = MoveInput.normalized;
    }
}
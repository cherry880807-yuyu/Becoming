using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum InputMode : byte
{
    Playing,
    UI,
    Locked
}
public class InputManager : Singleton<InputManager>
{
    public PlayerInputActions InputAction { get; private set; }
    private InputMode _InputMode;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InputAction = new PlayerInputActions();
        SetInputActionMap(InputMode.UI);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        InputAction.Disable();
        InputAction.Dispose();
    }

    public void SetInputActionMap(InputMode type)
    {
        if (_InputMode == type) return;
        _InputMode = type;

        switch (type)
        {
            case InputMode.Playing:
                InputAction.Player.Enable();
                InputAction.UI.Enable();
                break;
            case InputMode.UI:
                InputAction.Player.Disable();
                InputAction.UI.Enable();
                break;
            case InputMode.Locked:
                InputAction.Player.Disable();
                InputAction.UI.Disable();
                break;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private Stack<GameObject> uiStack = new Stack<GameObject>();
    private PlayerInputActions.UIActions _uiInput;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void OnEnable()
    {
        _uiInput = InputManager.Instance.InputAction.UI;
        _uiInput.Cancel.performed += HandleUIBackPerformed;
    }
    private void OnDisable()
    {
        _uiInput.Cancel.performed -= HandleUIBackPerformed;
    }

    //------------------------
    public void ShowUI(GameObject obj)
    {
        if (uiStack.Contains(obj)) return;
        obj.SetActive(true);
        uiStack.Push(obj);
        ApplyTopMode();
    }

    public void HideUI(GameObject obj)
    {
        if (uiStack.Count == 0 || !ReferenceEquals(uiStack.Peek(), obj)) return;
        uiStack.Pop();
        obj.SetActive(false);
        ApplyTopMode();
    }
    //------------------------

    private void ApplyTopMode()
    {
        InputMode mode = uiStack.Count > 0 ? InputMode.UI : InputMode.Playing;
        InputManager.Instance.SetInputActionMap(mode);
    }

    private void HandleUIBackPerformed(InputAction.CallbackContext _)
    {
        if (uiStack.Count > 0) HideUI(uiStack.Peek());
        //else OpenPauseMenu(); 
    }

    private bool HasOpenUI()
    {
        return uiStack.Count > 0;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseButton : MonoBehaviour
{
    protected Button button;
    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

    }
    protected virtual void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }


    protected abstract void OnClick();


}

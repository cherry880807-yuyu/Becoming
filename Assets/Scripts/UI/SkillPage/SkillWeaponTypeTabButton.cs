using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class SkillWeaponTypeTabButton : BaseButton //天賦頁切換武器分類的按鈕
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text labelText;

    private WeaponFamilySO mutationType;
    private Action<WeaponFamilySO> onClicked;

    protected override void Awake()
    {
        base.Awake();
        if (background == null && button != null) background = button.image;
    }

    public void Setup(
        WeaponFamilySO mutationType,
        string label,
        bool selected,
        bool unlocked,
        Color selectedColor,
        Color unselectedColor,
        Color lockedColor,
        Action<WeaponFamilySO> onClicked)
    {
        this.mutationType = mutationType;
        this.onClicked = onClicked;

        if (labelText != null) labelText.text = label;
        if (button != null) button.interactable = unlocked;
        if (background != null) background.color = !unlocked ? lockedColor : selected ? selectedColor : unselectedColor;
    }

    protected override void OnClick()
    {
        onClicked?.Invoke(mutationType);
    }
}

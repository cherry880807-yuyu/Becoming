using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillMutationItem : BaseButton, IPointerEnterHandler, IPointerExitHandler
{
    private static readonly Color LockedIconColor = new Color(0.27f, 0.27f, 0.27f,1f);

    [SerializeField] private Image iconImage;
    [SerializeField] private Image outline;
    private MutationDataSO mutationData;
    private SkillMutationTooltip tooltip;
    private Action<MutationDataSO> onClicked;
    private bool isSelected;
    private bool isUnlocked;
    private bool isSelectable;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Setup(MutationDataSO mutationData, bool unlocked, bool selected, SkillMutationTooltip tooltip, Action<MutationDataSO> onClicked)
    {
        this.mutationData = mutationData;
        this.tooltip = tooltip;
        this.onClicked = onClicked;
        SetState(unlocked, selected, unlocked && mutationData.RequiresSelection);
    }

    public void SetState(bool unlocked, bool selected, bool selectable)
    {
        isUnlocked = unlocked;
        isSelected = selected;
        isSelectable = selectable;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (mutationData == null)  return;

        if (iconImage != null)
        {
            iconImage.sprite = mutationData.icon;
            iconImage.color = isUnlocked ? Color.white : LockedIconColor;
        }

        if (outline != null) outline.enabled = isSelected;

        if (button != null) button.interactable = isSelectable;
    }

    protected override void OnClick()
    {
        if (isSelectable && mutationData != null)  onClicked?.Invoke(mutationData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null && mutationData != null) tooltip.Show(mutationData, isUnlocked,(RectTransform)transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null) tooltip.Hide();
    }
}

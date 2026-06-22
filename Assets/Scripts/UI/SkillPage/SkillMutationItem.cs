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

    protected override void Awake()
    {
        base.Awake();
    }

    public void Setup(MutationDataSO mutationData, bool unlocked, bool selected, SkillMutationTooltip tooltip, Action<MutationDataSO> onClicked)
    {
        this.mutationData = mutationData;
        this.tooltip = tooltip;
        this.onClicked = onClicked;
        SetState(unlocked, selected);
    }

    public void SetState(bool unlocked, bool selected)
    {
        isUnlocked = unlocked;
        isSelected = selected;
        UpdateUI();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (mutationData == null)
            return;

        if (iconImage != null)
        {
            iconImage.sprite = mutationData.icon;
            iconImage.color = isUnlocked ? Color.white : LockedIconColor;
        }

        if (outline != null)
            outline.enabled = isSelected;

        if (button != null)
            button.interactable = isUnlocked && mutationData.RequiresSelection;
    }

    protected override void OnClick()
    {
        if (isUnlocked && mutationData != null)
            onClicked?.Invoke(mutationData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null && mutationData != null)
            tooltip.Show(mutationData, isUnlocked,(RectTransform)transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.Hide();
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillMutationItem : BaseButton
{
    private static readonly Color LockedIconColor = new Color(0.27f, 0.27f, 0.27f);

    private Image iconImage;
    private Outline outline; //TODO 注意Outline的Canvas Batching效能成本
    private MutationDataSO mutationData;
    private Action<MutationDataSO> onClicked;
    private bool isSelected;
    private bool isUnlocked;

    protected override void Awake()
    {
        base.Awake();
        iconImage = GetComponent<Image>();
        outline = GetComponent<Outline>();
    }

    public void Setup(MutationDataSO mutationData, bool unlocked, bool selected, Action<MutationDataSO> onClicked)
    {
        this.mutationData = mutationData;
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
        {
            outline.enabled = isSelected;
        }

        if (button != null)
            button.interactable = isUnlocked && mutationData.RequiresSelection;
    }

    protected override void OnClick()
    {
        if (isUnlocked && mutationData != null) onClicked?.Invoke(mutationData);
    }
}

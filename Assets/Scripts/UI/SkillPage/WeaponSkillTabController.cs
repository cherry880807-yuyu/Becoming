using System;
using System.Collections.Generic;
using UnityEngine;

public readonly struct WeaponSkillTabConfig
{
    public WeaponFamilySO WeaponType { get; }
    public string Label { get; }
    public bool IsUnlocked { get; }

    public WeaponSkillTabConfig(WeaponFamilySO weaponType, string label, bool isUnlocked)
    {
        WeaponType = weaponType;
        Label = label;
        IsUnlocked = isUnlocked;
    }
}

public class WeaponSkillTabController : PageTabController //負責武器分類按鈕
{
    [Header("Dynamic Buttons")]
    [SerializeField] private Transform buttonRoot;
    [SerializeField] private SkillWeaponTypeTabButton buttonPrefab;
    [SerializeField] private Color lockedColor = new Color(0.8f, 0.8f, 0.8f);

    private readonly List<WeaponSkillTabConfig> tabConfigs = new();//UI 顯示用的武器分類資料列表
    private readonly List<SkillWeaponTypeTabButton> spawnedButtons = new(); //已生成的按鈕列表
    private WeaponFamilySO selectedWeaponType;

    public event Action<WeaponFamilySO> WeaponTypeSelected;

    public void SetTabs(IReadOnlyList<WeaponSkillTabConfig> configs, WeaponFamilySO selectedType)
    {
        tabConfigs.Clear();
        if (configs != null) tabConfigs.AddRange(configs);

        selectedWeaponType = selectedType;
        RefreshButtons();
    }

    public void SetSelectedWeaponType(WeaponFamilySO selectedType)
    {
        selectedWeaponType = selectedType;
        RefreshButtons();
    }

    private void RefreshButtons()
    {
        ClearButtons();
        if (buttonRoot == null || buttonPrefab == null) return;

        foreach (WeaponSkillTabConfig config in tabConfigs)
        {
            SkillWeaponTypeTabButton button = Instantiate(buttonPrefab, buttonRoot);
            button.Setup(
                config.WeaponType,
                config.Label,
                config.WeaponType == selectedWeaponType,
                config.IsUnlocked,
                selectedColor,
                unselectedColor,
                lockedColor,
                HandleWeaponTypeClicked);
            spawnedButtons.Add(button);
        }
    }

    private void ClearButtons()
    {
        foreach (SkillWeaponTypeTabButton button in spawnedButtons)
        {
            if (button != null) Destroy(button.gameObject);
        }

        spawnedButtons.Clear();
    }

    private void HandleWeaponTypeClicked(WeaponFamilySO weaponType)
    {
        if (weaponType == null || weaponType == selectedWeaponType) return;
        WeaponTypeSelected?.Invoke(weaponType);
    }
}

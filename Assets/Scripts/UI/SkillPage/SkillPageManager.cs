using System.Collections.Generic;
using UnityEngine;

public sealed class SkillPageManager : MonoBehaviour//負責天賦顯示流程
{
    [Header("Source")]
    [SerializeField] private WeaponCatalogSO weaponCatalog;

    [Header("Views")]
    [SerializeField] private WeaponSkillTabController weaponSkillTabController;
    [SerializeField] private SkillMutationView mutationView;

    private readonly List<WeaponFamilySO> weaponTypes = new();//從武器庫取得的武器類型列表
    private readonly List<WeaponSkillTabConfig> weaponTabConfigs = new(); //武器類型列表轉換成給UI使用的資料列表
    private readonly HashSet<WeaponFamilySO> ownedWeaponTypes = new();//將玩家擁有的武器轉換為已解鎖的的武器類型列表
    private WeaponFamilySO selectedWeaponType;
    private bool hasInventorySnapshot;

    private void OnEnable()
    {
        weaponSkillTabController.WeaponTypeSelected += HandleWeaponTypeSelected;
        EventBus.Subscribe<WeaponChangedEvent>(HandleWeaponChanged);
        EventBus.Subscribe<WeaponInventoryChangedEvent>(HandleWeaponInventoryChanged);
        EventBus.Subscribe<MutationUnlockedEvent>(HandleMutationUnlocked);

        SyncInventorySnapshotFromPlayer();
        RebuildWeaponTabs();
    }

    private void OnDisable()
    {
        weaponSkillTabController.WeaponTypeSelected -= HandleWeaponTypeSelected;
        EventBus.Unsubscribe<WeaponChangedEvent>(HandleWeaponChanged);
        EventBus.Unsubscribe<WeaponInventoryChangedEvent>(HandleWeaponInventoryChanged);
        EventBus.Unsubscribe<MutationUnlockedEvent>(HandleMutationUnlocked);
    }

    public void RebuildWeaponTabs() //重建武器分類按鈕資料
    {
        CollectWeaponTypesFromCatalog();
        BuildWeaponTabConfigs();

        selectedWeaponType = ResolveSelection(selectedWeaponType);
        weaponSkillTabController?.SetTabs(weaponTabConfigs, selectedWeaponType);
        mutationView?.SetSelectedWeaponType(selectedWeaponType);
    }
    //------------------------訂閱事件------------------------
    private void HandleWeaponTypeSelected(WeaponFamilySO weaponType)
    {
        if (weaponType == null || weaponType == selectedWeaponType) return;
        if (!IsWeaponTypeOwned(weaponType)) return;

        selectedWeaponType = weaponType;
        weaponSkillTabController?.SetSelectedWeaponType(selectedWeaponType);
        mutationView?.SetSelectedWeaponType(selectedWeaponType);
    }
    private void HandleWeaponChanged(WeaponChangedEvent eventData) //玩家實際切換裝備武器時觸發
    {
        UpdateInventorySnapshot(eventData.ownedWeapons);
        RebuildWeaponTabs();
    }

    private void HandleWeaponInventoryChanged(WeaponInventoryChangedEvent eventData) //玩家解鎖新武器或武器庫變動時觸發
    {
        UpdateInventorySnapshot(eventData.ownedWeapons);
        RebuildWeaponTabs();
    }

    private void HandleMutationUnlocked(MutationUnlockedEvent eventData)
    {
        mutationView?.Rebuild();
    }

    //------------------------------------------------
    private void CollectWeaponTypesFromCatalog() //從 WeaponCatalogSO.Weapons 獲取資料並建立所有武器分類
    {
        weaponTypes.Clear();
        if (weaponCatalog == null || weaponCatalog.Weapons == null) return;

        foreach (WeaponDataSO weapon in weaponCatalog.Weapons)
        {
            if (weapon == null) continue;
            var weaponType = weapon.mutationType;
            if (weaponType == null || weaponType.isUniversal) return;
            if (!weaponTypes.Contains(weaponType)) weaponTypes.Add(weaponType);
        }
    }

    private void BuildWeaponTabConfigs() //將 weaponTypes 轉換成 UI 需要的資料
    {
        weaponTabConfigs.Clear();

        foreach (WeaponFamilySO weaponType in weaponTypes)
        {
            weaponTabConfigs.Add(new WeaponSkillTabConfig(
                weaponType,
                GetWeaponTypeLabel(weaponType),
                IsWeaponTypeOwned(weaponType)));
        }
    }

    private WeaponFamilySO ResolveSelection(WeaponFamilySO preferredType) //決定目前應該選中哪個武器分類
    {
        if (preferredType != null && weaponTypes.Contains(preferredType) && IsWeaponTypeOwned(preferredType)) return preferredType;

        WeaponFamilySO equippedType = GetEquippedWeaponType();
        if (equippedType != null && weaponTypes.Contains(equippedType)) return equippedType;

        foreach (WeaponFamilySO weaponType in weaponTypes)
        {
            if (IsWeaponTypeOwned(weaponType)) return weaponType;
        }

        return weaponTypes.Count > 0 ? weaponTypes[0] : null;
    }

    private string GetWeaponTypeLabel(WeaponFamilySO weaponType)//決定武器分類按鈕上顯示什麼文字
    {
        if (weaponType == null) return string.Empty;
        return string.IsNullOrWhiteSpace(weaponType.displayName)
            ? weaponType.name
            : weaponType.displayName;
    }

    private WeaponFamilySO GetEquippedWeaponType() //取得玩家目前實際裝備武器的分類
    {
        if (!PlayerLocator.IsInitialized || PlayerLocator.Instance.PlayerBrain == null) return null;

        return PlayerLocator.Instance.PlayerBrain
            .PlayerActorData?
            .WeaponInventorySystem?
            .EquippedWeapon?
            .mutationType;
    }

    private bool IsWeaponTypeOwned(WeaponFamilySO weaponType) //判斷玩家是否已擁有該武器分類
    {
        if (weaponType == null || weaponType.isUniversal) return true;
        if (hasInventorySnapshot) return ownedWeaponTypes.Contains(weaponType);
        return true;
    }

    private void SyncInventorySnapshotFromPlayer() //UI 開啟時，從玩家身上主動抓目前完整已擁有武器清單
    {
        if (!PlayerLocator.IsInitialized || PlayerLocator.Instance.PlayerBrain == null) return;
        PlayerWeaponInventorySystem inventorySystem = PlayerLocator.Instance.PlayerBrain.PlayerActorData?.WeaponInventorySystem;
        if (inventorySystem == null) return;
        UpdateInventorySnapshot(inventorySystem.OwnedWeapons);
    }

    private void UpdateInventorySnapshot(WeaponDataSO[] ownedWeapons) //把玩家已擁有的武器轉成已擁有的武器分類
    {
        if (ownedWeapons == null) return;

        ownedWeaponTypes.Clear();
        foreach (WeaponDataSO weapon in ownedWeapons)
        {
            if (weapon != null && weapon.mutationType != null) ownedWeaponTypes.Add(weapon.mutationType);
        }

        hasInventorySnapshot = true;
    }
}

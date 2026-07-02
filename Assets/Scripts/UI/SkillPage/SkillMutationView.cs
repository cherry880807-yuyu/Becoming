using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillMutationView : MonoBehaviour // 負責天賦 item 生成
{
    [Serializable]
    private class MutationPageBinding
    {
        public string pageName = "Page";
        public SkillType category = SkillType.Passive;
        public WeaponFamilySO fixedMutationType = null;
        public bool useSelectedWeaponType = false;
        public List<MutationSectionBinding> sections = new();
    }

    [Serializable]
    private class MutationSectionBinding
    {
        public string sectionName = "Section";
        public SubCategory subCategory = SubCategory.Movement;
        public Transform content = null;
    }

    [Header("View")]
    [SerializeField] private SkillMutationItem itemPrefab;
    [SerializeField] private SkillMutationTooltip tooltip;
    [SerializeField] private List<MutationPageBinding> pages = new();

    private readonly Dictionary<MutationDataSO, SkillMutationItem> spawnedItems = new();
    private WeaponFamilySO selectedWeaponType;

    private void OnEnable()
    {
        Rebuild();
    }

    private void OnDisable()
    {
        if (tooltip != null) tooltip.Hide();
    }

    public void SetSelectedWeaponType(WeaponFamilySO mutationType)
    {
        selectedWeaponType = mutationType;
        Rebuild();
    }

    public void Rebuild()
    {
        ClearItems();
        if (!MutationManager.IsInitialized || MutationManager.Instance.AllMutations == null) return;

        foreach (MutationDataSO mutation in MutationManager.Instance.AllMutations)
            AddItem(mutation);
    }

    private void AddItem(MutationDataSO mutation)
    {
        if (mutation == null || spawnedItems.ContainsKey(mutation)) return;

        Transform parent = FindSection(mutation);
        if (parent == null) return;

        SkillMutationItem item = CreateItem(parent);
        if (item == null) return;

        item.Setup(
            mutation,
            MutationManager.Instance.IsUnlocked(mutation),
            MutationManager.Instance.IsSelected(mutation),
            tooltip,
            HandleItemClicked);
        item.SetState(
            MutationManager.Instance.IsUnlocked(mutation),
            MutationManager.Instance.IsSelected(mutation),
            MutationManager.Instance.CanSelectMutation(mutation));
        spawnedItems.Add(mutation, item);
    }

    private SkillMutationItem CreateItem(Transform parent)
    {
        return itemPrefab != null ? Instantiate(itemPrefab, parent) : null;
    }

    private void ClearItems()
    {
        foreach (SkillMutationItem item in spawnedItems.Values)
        {
            if (item != null) Destroy(item.gameObject);
        }

        spawnedItems.Clear();
    }

    private void UpdateAllItemState()
    {
        if (!MutationManager.IsInitialized) return;

        foreach (KeyValuePair<MutationDataSO, SkillMutationItem> pair in spawnedItems)
        {
            pair.Value.SetState(
                MutationManager.Instance.IsUnlocked(pair.Key),
                MutationManager.Instance.IsSelected(pair.Key),
                MutationManager.Instance.CanSelectMutation(pair.Key));
        }
    }

    private void HandleItemClicked(MutationDataSO mutation)
    {
        if (!MutationManager.IsInitialized) return;
        if (mutation == null || mutation.IsPassive) return;
        if (!MutationManager.Instance.CanSelectMutation(mutation)) return;

        MutationManager.Instance.SelectMutation(mutation);
        UpdateAllItemState();
    }

    private Transform FindSection(MutationDataSO mutation)
    {
        if (mutation == null) return null;

        foreach (MutationPageBinding page in pages)
        {
            if (page == null || page.category != mutation.category) continue;

            WeaponFamilySO pageMutationType = page.useSelectedWeaponType
                ? selectedWeaponType
                : page.fixedMutationType;

            if (pageMutationType != null && pageMutationType != mutation.mutationType) continue;
            if (page.sections == null) continue;

            foreach (MutationSectionBinding section in page.sections)
            {
                if (section == null || section.content == null) continue;
                if (section.subCategory == mutation.subCategory) return section.content;
            }
        }

        return null;
    }
}

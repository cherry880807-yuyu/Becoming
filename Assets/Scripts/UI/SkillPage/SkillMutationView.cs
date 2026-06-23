using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMutationView : MonoBehaviour
{
    [Serializable]
    private class CategoryContainer
    {
        public SkillType category = SkillType.Passive;
        public SubCategory passiveCategory = SubCategory.Movement;
        public Transform content = null;
    }

    [SerializeField] private SkillMutationItem itemPrefab;
    [SerializeField] private SkillMutationTooltip tooltip;
    [SerializeField] private List<CategoryContainer> containers = new();

    private readonly Dictionary<MutationDataSO, SkillMutationItem> spawnedItems = new();

    private void OnEnable()
    {
        EventBus.Subscribe<MutationUnlockedEvent>(HandleMutationUnlocked);
        Rebuild();
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<MutationUnlockedEvent>(HandleMutationUnlocked);
        tooltip.Hide();
    }

    public void Rebuild()
    {
        ClearItems();

        if (!MutationManager.IsInitialized || MutationManager.Instance.AllMutations == null)
            return;

        foreach (var mutation in MutationManager.Instance.AllMutations)
            AddItem(mutation);
    }

    private void HandleMutationUnlocked(MutationUnlockedEvent eventData)
    {
        if (!spawnedItems.ContainsKey(eventData.mutation)) AddItem(eventData.mutation);
        UpdateItemState(eventData.mutation);
    }

    private void AddItem(MutationDataSO mutation)
    {
        if (mutation == null || spawnedItems.ContainsKey(mutation)) return;

        Transform parent = FindContainer(mutation);
        if (parent == null) return;

        SkillMutationItem item = CreateItem(parent);
        item.Setup(
            mutation,
            MutationManager.Instance.IsUnlocked(mutation),
            MutationManager.Instance.IsSelected(mutation),
            tooltip,
            HandleItemClicked);
        spawnedItems.Add(mutation, item);
    }

    private void HandleItemClicked(MutationDataSO mutation)
    {
        if (!MutationManager.IsInitialized) return;
        if (mutation.IsPassive) return;

        MutationManager.Instance.SelectMutation(mutation);
        UpdateAllItemState();
    }

    private Transform FindContainer(MutationDataSO mutation)
    {
        foreach (var container in containers)
        {
            if (container.content == null || container.category != mutation.category)
                continue;

            if (mutation.category != SkillType.Passive || container.passiveCategory == mutation.passiveCategory)
                return container.content;
        }

        return null;
    }

    private SkillMutationItem CreateItem(Transform parent)
    {
        if (itemPrefab != null) return Instantiate(itemPrefab, parent);
        return null;
    }

    private void UpdateItemState(MutationDataSO mutation)
    {
        if (!MutationManager.IsInitialized || mutation == null) return;

        if (spawnedItems.TryGetValue(mutation, out SkillMutationItem item))
        {
            item.SetState(MutationManager.Instance.IsUnlocked(mutation), MutationManager.Instance.IsSelected(mutation));
        }
    }

    private void UpdateAllItemState()
    {
        if (!MutationManager.IsInitialized) return;

        foreach (var pair in spawnedItems)
            pair.Value.SetState(MutationManager.Instance.IsUnlocked(pair.Key), MutationManager.Instance.IsSelected(pair.Key));
    }

    private void ClearItems()
    {
        foreach (var item in spawnedItems.Values)
        {
            if (item != null) Destroy(item.gameObject);
        }
        spawnedItems.Clear();
    }
}

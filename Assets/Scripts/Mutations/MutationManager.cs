using System.Collections.Generic;
using UnityEngine;

public class MutationManager : Singleton<MutationManager>
{
    [SerializeField] private List<MutationDataSO> allMutations;
    [SerializeField] private List<MutationDataSO> unlocked = new();
    [SerializeField] private List<MutationDataSO> selectedMutations = new();

    public IReadOnlyList<MutationDataSO> AllMutations => allMutations;
    public IReadOnlyList<MutationDataSO> UnlockedMutations => unlocked;
    public IReadOnlyList<MutationDataSO> SelectedMutations => selectedMutations;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    //--------------------------------------------------計算是否解鎖
    public void EvaluateMutations(MutationContext context) //TODO 避免無差別檢查
    {
        if (allMutations == null) return;
        foreach (var mutation in allMutations)
        {
            if (mutation == null || unlocked.Contains(mutation)) continue;
            if (CanUnlock(mutation, context)) UnlockMutation(mutation);
        }
    }
    //--------------------------------------------------檢測
    private bool CanUnlock(MutationDataSO mutation, MutationContext context)
    {
        if (mutation.conditions == null || mutation.conditions.Count <= 0) return false;
        foreach (var condition in mutation.conditions)
        {
            if (condition == null || !condition.Evaluate(context)) return false;
        }
        return true;
    }

    public bool IsUnlocked(MutationDataSO mutation)
    {
        return mutation != null && unlocked.Contains(mutation);
    }

    public bool IsSelected(MutationDataSO mutation)
    {
        return mutation != null && selectedMutations.Contains(mutation);
    }

    private bool IsSameSelectionGroup(MutationDataSO left, MutationDataSO right)
    {
        return left.category == right.category && left.selectionGroup == right.selectionGroup;
    }
    //--------------------------------------------------解鎖、選擇與選擇超過數量的處理
    private void UnlockMutation(MutationDataSO mutation)
    {
        unlocked.Add(mutation);
        Debug.Log($"Unlock Mutation : {mutation.mutationName}");
        EventBus.Publish(new MutationUnlockedEvent { mutation = mutation });
        if (mutation.IsPassive) EnableMutation(mutation);
    }

    public void SelectMutation(MutationDataSO mutation)
    {
        if (!IsUnlocked(mutation) || !mutation.RequiresSelection) return;

        if (selectedMutations.Contains(mutation))
        {
            DisableMutation(mutation);
            return;
        }

        EnforceSelectionLimit(mutation);
        EnableMutation(mutation);
    }

    private void EnforceSelectionLimit(MutationDataSO mutation)
    {
        int limit = Mathf.Max(1, mutation.selectionLimit);
        int selectedCount = 0;

        for (int i = 0; i < selectedMutations.Count; i++)
        {
            MutationDataSO selected = selectedMutations[i];
            if (selected != null && selected.RequiresSelection && IsSameSelectionGroup(selected, mutation)) selectedCount++;
        }

        while (selectedCount >= limit)
        {
            MutationDataSO oldest = FindOldestSelectedInGroup(mutation);
            if (oldest == null) break;
            DisableMutation(oldest);
            selectedCount--;
        }
    }

    //--------------------------------------------------技能啟用與停用
    private void EnableMutation(MutationDataSO mutation)
    {
        if (mutation == null || selectedMutations.Contains(mutation)) return;
        selectedMutations.Add(mutation);
        ApplyEffects(mutation);
    }

    private void DisableMutation(MutationDataSO mutation)
    {
        if (mutation == null || !selectedMutations.Remove(mutation)) return;
        RemoveEffects(mutation);
    }

    //--------------------------------------------------效果啟用與停用
    private void ApplyEffects(MutationDataSO mutation)
    {
        GameObject target = GetPlayerTarget();
        if (target == null || mutation == null || mutation.effects == null) return;
        foreach (var effect in mutation.effects)
        {
            if (effect != null) effect.Apply(target);
        }
    }

    private void RemoveEffects(MutationDataSO mutation)
    {
        GameObject target = GetPlayerTarget();
        if (target == null || mutation == null || mutation.effects == null) return;
        foreach (var effect in mutation.effects)
        {
            if (effect != null) effect.Remove(target);
        }
    }

    //--------------------------------------------------
    private MutationDataSO FindOldestSelectedInGroup(MutationDataSO mutation)
    {
        foreach (var selected in selectedMutations)
        {
            if (selected != null && selected.RequiresSelection && IsSameSelectionGroup(selected, mutation)) return selected;
        }
        return null;
    }

    private GameObject GetPlayerTarget()
    {
        if (!PlayerLocator.IsInitialized || PlayerLocator.Instance.PlayerTransform == null) return null;
        return PlayerLocator.Instance.PlayerTransform.gameObject;
    }
}

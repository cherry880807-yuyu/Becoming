using System.Collections.Generic;
using UnityEngine;

public class MutationManager : Singleton<MutationManager>
{
    [SerializeField] private List<MutationDataSO> allMutations;
    [SerializeField] private List<MutationDataSO> unlocked = new();
    [SerializeField] private List<MutationDataSO> selectedMutations = new();
    private readonly HashSet<MutationDataSO> appliedMutations = new();
    private WeaponDataSO currentWeapon;
    private WeaponFamilySO currentMutationType;

    public IReadOnlyList<MutationDataSO> AllMutations => allMutations;
    public IReadOnlyList<MutationDataSO> UnlockedMutations => unlocked;
    public IReadOnlyList<MutationDataSO> SelectedMutations => selectedMutations;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<WeaponChangedEvent>(OnWeaponChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<WeaponChangedEvent>(OnWeaponChanged);
    }

    //--------------------------------------------------計算是否解鎖
    public void EvaluateMutations(MutationContext context) //TODO 避免無差別檢查
    {
        if (allMutations == null) return;
        if (context != null && context.equippedWeapon != currentWeapon)
            SetCurrentWeapon(context.equippedWeapon, context.equippedMutationType);

        foreach (var mutation in allMutations)
        {
            if (mutation == null || unlocked.Contains(mutation)) continue;
            if (!CanEvaluateForCurrentType(mutation, context)) continue;
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

    public bool CanSelectMutation(MutationDataSO mutation)
    {
        if (mutation == null || mutation.IsPassive) return false;
        if (!IsUnlocked(mutation)) return false;
        return IsRequiredTypeOwned(mutation);
    }

    private bool IsSameSelectionGroup(MutationDataSO left, MutationDataSO right)
    {
        return left.category == right.category &&
               left.subCategory == right.subCategory &&
               left.mutationType == right.mutationType;
    }
    //--------------------------------------------------解鎖、選擇與選擇超過數量的處理
    private void UnlockMutation(MutationDataSO mutation)
    {
        unlocked.Add(mutation);
        Debug.Log($"Unlock Mutation : {mutation.mutationName}");
        if (mutation.IsPassive) EnableMutation(mutation);
        EventBus.Publish(new MutationUnlockedEvent { mutation = mutation });
    }

    public void SelectMutation(MutationDataSO mutation)
    {
        if (!CanSelectMutation(mutation)) return;

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
        TryApplyEffects(mutation);
    }

    private void DisableMutation(MutationDataSO mutation)
    {
        if (mutation == null || !selectedMutations.Remove(mutation)) return;
        TryRemoveEffects(mutation);
    }

    //--------------------------------------------------效果啟用與停用
    private void ApplyEffects(MutationDataSO mutation)
    {
        GameObject target = GetPlayerTarget();
        if (target == null || mutation == null || mutation.effects == null) return;
        foreach (var effect in mutation.effects)
        {
            if (effect != null) effect.Apply(target, mutation);
        }
    }

    private void RemoveEffects(MutationDataSO mutation)
    {
        GameObject target = GetPlayerTarget();
        if (target == null || mutation == null || mutation.effects == null) return;
        foreach (var effect in mutation.effects)
        {
            if (effect != null) effect.Remove(target, mutation);
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

    private bool CanEvaluateForCurrentType(MutationDataSO mutation, MutationContext context)
    {
        if (IsUniversalMutation(mutation)) return true;
        return context != null && context.equippedMutationType == mutation.mutationType;
    }

    private bool CanApplyForCurrentWeapon(MutationDataSO mutation)
    {
        return IsUniversalMutation(mutation) || mutation.mutationType == currentMutationType;
    }

    private bool IsRequiredTypeOwned(MutationDataSO mutation)
    {
        if (IsUniversalMutation(mutation)) return true;
        if (!PlayerLocator.IsInitialized || PlayerLocator.Instance.PlayerBrain == null) return false;

        return PlayerLocator.Instance.PlayerBrain
            .PlayerActorData?
            .WeaponInventorySystem?
            .IsTypeOwned(mutation.mutationType) == true;
    }

    private void TryApplyEffects(MutationDataSO mutation)
    {
        if (mutation == null || appliedMutations.Contains(mutation)) return;
        if (!CanApplyForCurrentWeapon(mutation)) return;

        ApplyEffects(mutation);
        appliedMutations.Add(mutation);
    }

    private void TryRemoveEffects(MutationDataSO mutation)
    {
        if (mutation == null || !appliedMutations.Remove(mutation)) return;
        RemoveEffects(mutation);
    }

    private void OnWeaponChanged(WeaponChangedEvent e)
    {
        SetCurrentWeapon(e.weapon, e.weapon != null ? e.weapon.mutationType : null);
    }

    private void SetCurrentWeapon(WeaponDataSO weapon, WeaponFamilySO mutationType)
    {
        if (currentWeapon == weapon && currentMutationType == mutationType) return;

        currentWeapon = weapon;
        currentMutationType = mutationType;
        RefreshAppliedEffectsForCurrentWeapon();
    }

    private void RefreshAppliedEffectsForCurrentWeapon()
    {
        for (int i = selectedMutations.Count - 1; i >= 0; i--)
            TryRemoveEffects(selectedMutations[i]);

        for (int i = 0; i < selectedMutations.Count; i++)
            TryApplyEffects(selectedMutations[i]);
    }

    private bool IsUniversalMutation(MutationDataSO mutation)
    {
        return mutation == null ||
               mutation.mutationType == null ||
               mutation.mutationType.isUniversal;
    }
}

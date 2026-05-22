using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutationManager : Singleton<MutationManager>
{
    // 1.new mutationData 
    // 2.new mutationCondition 
    // 3.new mutationEffect 
    // 4.在MutationContext 新建要記錄的參數 
    // 5.新建struct event 讓Mutation Tracker做追蹤

    [SerializeField]
    private List<MutationDataSO> allMutations;

    [SerializeField]
    private List<MutationDataSO> unlocked = new();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void EvaluateMutations(MutationContext context)
    {
        foreach (var mutation in allMutations)
        {
            if (unlocked.Contains(mutation))
                continue;

            bool success = true;

            foreach (var condition in mutation.conditions)
            {
                if (!condition.Evaluate(context))
                {
                    success = false;
                    break;
                }
            }

            if (success)
            {
                UnlockMutation(mutation);
            }
        }
    }

    private void UnlockMutation(MutationDataSO mutation)
    {
        unlocked.Add(mutation);
        Debug.Log($"解鎖 Mutation : {mutation.mutationName}");

        EventBus.Publish(new MutationUnlockedEvent
        {
            mutation = mutation
        });
        foreach (var effect in mutation.effects)
        {
            effect.Apply(PlayerLocator.Instance.PlayerTransform.gameObject);
        }
    }
}
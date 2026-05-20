using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//成就追蹤
public class MutationTracker : MonoBehaviour
{
    [SerializeField]
    MutationContext Context = new MutationContext();


    private void OnEnable()
    {
        EventBus.Subscribe<AttackThreeTimesEvent>(OnEnemyAttacked);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<AttackThreeTimesEvent>(OnEnemyAttacked);
    }

    private void OnEnemyAttacked(AttackThreeTimesEvent e)
    {
        Context.test_TotalAttackCount++;
        MutationManager.Instance.EvaluateMutations(Context);
    }

}
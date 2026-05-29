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
        EventBus.Subscribe<AttackEnemyEvent>(OnEnemyAttacked);
        EventBus.Subscribe<DodgeSucceededEvent>(OnPlayerDodgeSucceed);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<AttackEnemyEvent>(OnEnemyAttacked);
        EventBus.Unsubscribe<DodgeSucceededEvent>(OnPlayerDodgeSucceed);
    }

    private void OnEnemyAttacked(AttackEnemyEvent e)
    {
        Context.test_TotalAttackCount++;
        MutationManager.Instance.EvaluateMutations(Context);
    }

    private void OnPlayerDodgeSucceed(DodgeSucceededEvent e)
    {
        Context.dodgeSucceedCount++;
        MutationManager.Instance.EvaluateMutations(Context);
    }

}
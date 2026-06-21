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
        EventBus.Subscribe<JumpEvent>(OnPlayerJumped);
        EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<AttackEnemyEvent>(OnEnemyAttacked);
        EventBus.Unsubscribe<DodgeSucceededEvent>(OnPlayerDodgeSucceed);
        EventBus.Unsubscribe<JumpEvent>(OnPlayerJumped);
        EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnEnemyAttacked(AttackEnemyEvent e)
    {
        Context.totalAttackCount++;
        MutationManager.Instance.EvaluateMutations(Context);
    }

    private void OnPlayerDodgeSucceed(DodgeSucceededEvent e)
    {
        Context.dodgeSucceedCount++;
        MutationManager.Instance.EvaluateMutations(Context);
    }

    private void OnPlayerJumped(JumpEvent e)
    {
        Context.totalJumpCount++;
        MutationManager.Instance.EvaluateMutations(Context);
    }

    private void OnPlayerDied(PlayerDiedEvent e)
    {
        Context.deathCount++;
        MutationManager.Instance.EvaluateMutations(Context);
    }

}
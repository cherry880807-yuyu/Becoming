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
        EventBus.Subscribe<PlayerSprintDistanceEvent>(OnPlayerSprintDistanceChanged);
        EventBus.Subscribe<DashEvent>(OnPlayerDashed);
        EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);
        EventBus.Subscribe<CampfireHealEvent>(OnCampfireHealed);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<AttackEnemyEvent>(OnEnemyAttacked);
        EventBus.Unsubscribe<DodgeSucceededEvent>(OnPlayerDodgeSucceed);
        EventBus.Unsubscribe<JumpEvent>(OnPlayerJumped);
        EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
        EventBus.Unsubscribe<PlayerSprintDistanceEvent>(OnPlayerSprintDistanceChanged);
        EventBus.Unsubscribe<DashEvent>(OnPlayerDashed);
        EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
        EventBus.Unsubscribe<CampfireHealEvent>(OnCampfireHealed);
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

    private void OnPlayerSprintDistanceChanged(PlayerSprintDistanceEvent e)
    {
        Context.totalSprintDistance += e.distance;
        MutationManager.Instance.EvaluateMutations(Context);
    }

    private void OnPlayerDashed(DashEvent e)
    {
        Context.totalDashCount++;
        MutationManager.Instance.EvaluateMutations(Context);
    }

    private void OnEnemyDied(EnemyDiedEvent e)
    {
        Context.enemyKillsSinceCampfireHeal++;
        MutationManager.Instance.EvaluateMutations(Context);
    }

    private void OnCampfireHealed(CampfireHealEvent e)
    {
        Context.enemyKillsSinceCampfireHeal = 0;
        MutationManager.Instance.EvaluateMutations(Context);
    }

}

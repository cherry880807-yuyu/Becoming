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
        EventBus.Subscribe<PlayerHealthChangedEvent>(OnPlayerHealthChanged);
        EventBus.Subscribe<WeaponChangedEvent>(OnWeaponChanged);
    }

    private IEnumerator Start()
    {
        while (!MutationManager.IsInitialized)
            yield return null;

        SyncCurrentWeaponFromPlayer();
        EvaluateMutations();
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
        EventBus.Unsubscribe<PlayerHealthChangedEvent>(OnPlayerHealthChanged);
        EventBus.Unsubscribe<WeaponChangedEvent>(OnWeaponChanged);
    }

    private void OnEnemyAttacked(AttackEnemyEvent e)
    {
        Context.totalAttackCount++;
        EvaluateMutations();
    }

    private void OnPlayerDodgeSucceed(DodgeSucceededEvent e)
    {
        Context.dodgeSucceedCount++;
        if (e.wouldBeLethal) Context.totalLethalDodgeCount++;
        EvaluateMutations();
    }

    private void OnPlayerJumped(JumpEvent e)
    {
        Context.totalJumpCount++;
        EvaluateMutations();
    }

    private void OnPlayerDied(PlayerDiedEvent e)
    {
        Context.deathCount++;
        EvaluateMutations();
    }

    private void OnPlayerSprintDistanceChanged(PlayerSprintDistanceEvent e)
    {
        Context.totalSprintDistance += e.distance;
        EvaluateMutations();
    }

    private void OnPlayerDashed(DashEvent e)
    {
        Context.totalDashCount++;
        EvaluateMutations();
    }

    private void OnEnemyDied(EnemyDiedEvent e)
    {
        Context.enemyKillsSinceCampfireHeal++;
        EvaluateMutations();
    }

    private void OnCampfireHealed(CampfireHealEvent e)
    {
        Context.enemyKillsSinceCampfireHeal = 0;
        EvaluateMutations();
    }

    private void OnPlayerHealthChanged(PlayerHealthChangedEvent e)
    {
        Context.playerHealthPercent = e.healthPercent;
        EvaluateMutations();
    }

    private void OnWeaponChanged(WeaponChangedEvent e)
    {
        Context.equippedWeapon = e.weapon;
        Context.equippedMutationType = e.weapon != null ? e.weapon.mutationType : null;
        EvaluateMutations();
    }

    private void EvaluateMutations()
    {
        if (!MutationManager.IsInitialized) return;
        MutationManager.Instance.EvaluateMutations(Context);
    }

    private void SyncCurrentWeaponFromPlayer()
    {
        if (!PlayerLocator.IsInitialized || PlayerLocator.Instance.PlayerBrain == null) return;
        Context.equippedWeapon = PlayerLocator.Instance.PlayerBrain
            .PlayerActorData?
            .WeaponInventorySystem?
            .EquippedWeapon;
        Context.equippedMutationType = Context.equippedWeapon != null ? Context.equippedWeapon.mutationType : null;
    }

}

using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerWeaponInventorySystem
{
    private readonly PlayerBrain player;
    private readonly PlayerCombatSystem combatSystem;
    private readonly HashSet<WeaponDataSO> ownedWeapons = new();

    private WeaponDataSO equippedWeapon;
    public WeaponDataSO EquippedWeapon => equippedWeapon;
    public WeaponDataSO[] OwnedWeapons => GetOwnedWeaponsSnapshot();

    public PlayerWeaponInventorySystem(
        PlayerBrain player,
        PlayerCombatSystem combatSystem,
        WeaponDataSO defaultWeapon)
    {
        this.player = player;
        this.combatSystem = combatSystem;
        UnlockWeapon(defaultWeapon);
        EquipWeapon(defaultWeapon);
    }

    public bool IsOwned(WeaponDataSO weapon)
    {
        return weapon != null && ownedWeapons.Contains(weapon);
    }

    public bool IsTypeOwned(WeaponFamilySO mutationType)
    {
        if (mutationType == null || mutationType.isUniversal) return true;

        foreach (WeaponDataSO weapon in ownedWeapons)
        {
            if (weapon != null && weapon.mutationType == mutationType)
                return true;
        }

        return false;
    }

    public bool UnlockWeapon(WeaponDataSO weapon)
    {
        if (weapon == null || !ownedWeapons.Add(weapon)) return false;

        PublishInventoryChanged();
        Debug.Log("Weapon unlocked: " + weapon.name);
        return true;
    }

    public bool EquipWeapon(WeaponDataSO weapon)
    {
        if (!IsOwned(weapon) || combatSystem == null) return false;
        if (equippedWeapon == weapon) return true;

        equippedWeapon = weapon;
        combatSystem.ApplyWeapon(weapon);
        PublishWeaponChanged();
        PublishInventoryChanged();
        return true;
    }

    private void PublishWeaponChanged()
    {
        EventBus.Publish(new WeaponChangedEvent
        {
            player = player,
            weapon = equippedWeapon,
            ownedWeapons = GetOwnedWeaponsSnapshot()
        });
    }

    private void PublishInventoryChanged() //用於更新選擇武器的UI
    {
        EventBus.Publish(new WeaponInventoryChangedEvent
        {
            player = player,
            equippedWeapon = equippedWeapon,
            ownedWeapons = GetOwnedWeaponsSnapshot()
        });
    }

    private WeaponDataSO[] GetOwnedWeaponsSnapshot()
    {
        WeaponDataSO[] snapshot = new WeaponDataSO[ownedWeapons.Count];
        ownedWeapons.CopyTo(snapshot);
        return snapshot;
    }
}

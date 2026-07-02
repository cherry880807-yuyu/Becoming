using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Catalog")]
public class WeaponCatalogSO : ScriptableObject
{
    [SerializeField] private List<WeaponDataSO> weapons = new();

    public IReadOnlyList<WeaponDataSO> Weapons => weapons;
}

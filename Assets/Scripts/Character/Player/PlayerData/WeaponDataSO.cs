using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackPowerType
{
    LightAttack,
    HeavyAttack,
}
public enum ComboType
{
    Ground = 0,
    Air = 1,
    Dash = 2
}

[CreateAssetMenu(menuName = "Weapon/WeaponDataSO")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Weapon")]
    public GameObject weaponPrefab;

    [Header("Combo")]
    public ComboStep[] groundSteps;
    public ComboStep[] airSteps;
    public ComboStep[] dashSteps;

    public float resetTime;

    public ComboStep[] GetSteps(ComboType comboType) => comboType switch
    {
        ComboType.Ground => groundSteps,
        ComboType.Air => airSteps,
        ComboType.Dash => dashSteps,
        _ => groundSteps
    };

}

[System.Serializable]
public class ComboStep
{
    public AnimationClip anim;

    public int damage;
    public AttackPowerType attackPowerType;
    public float hitStopTime => GetHitStopTime(attackPowerType);

    [Header("Knockback")]
    public float knockbackForce;
    [Range(90f, -90f)]
    public float KnockbackAngle = 45f;


    float GetHitStopTime(AttackPowerType attackPowerType)
    {
        switch (attackPowerType)
        {
            case AttackPowerType.LightAttack:
                return 0.05f;
            case AttackPowerType.HeavyAttack:
                return 0.08f;
        }
        return 0;
    }

}
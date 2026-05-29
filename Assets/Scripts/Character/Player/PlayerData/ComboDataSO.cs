using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackPowerType
{
    LightAttack,
    HeavyAttack,
}
[CreateAssetMenu(menuName = "Weapon/ComboDataSO")]
public class ComboDataSO : ScriptableObject
{
    [Header("Combo")]
    public ComboStep[] steps;
    public float resetTime;

    [Header("Weapon")]
    public GameObject weaponPrefab;
}

[System.Serializable]
public class ComboStep
{
    public int damage;
    public AttackPowerType attackPowerType;
    public float hitStopTime => GetHitStopTime(attackPowerType);
    public float knockbackForce;
    public AnimationClip anim;

    float GetHitStopTime(AttackPowerType attackPowerType)
    {
        switch (attackPowerType)
        {
            case AttackPowerType.LightAttack:
                return 0.03f;
            case AttackPowerType.HeavyAttack:
                return 0.08f;
        }
        return 0;
    }

}
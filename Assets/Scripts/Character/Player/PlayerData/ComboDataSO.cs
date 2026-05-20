using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Combat/ComboDataSO")]
public class ComboDataSO : ScriptableObject
{
    [Header("Combo")]
    public ComboStep[]  steps;
    public float resetTime;

    [Header("Weapon")]
    public GameObject weaponPrefab;
}

[System.Serializable]
public class ComboStep
{
    public int damage;
    public float hitStopTime;
    public float  knockbackForce;
    public AnimationClip anim;
}
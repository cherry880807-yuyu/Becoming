using System;
using UnityEditor.U2D.Animation;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Character Data")]
public class CharacterDataSO : ScriptableObject
{
    [Header("Identity")]
    public string characterId;        // "stickman_warrior" / "stickman_mage"
    public string Name;
    public GameObject prefab;         // 可選，給角色選擇系統用

    [Header("Health")]
    public int maxHP = 100;
    public int maxShield = 0;

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/AttackData/GroundMelee AttackData")]
public class GroundMeleeAttackDataSO : AttackDataSO
{
    [Header("Range")]
    public float attackRange   = 1.5f;

    [Header("Timing")]
    public float attackDuration = 0.6f;  // 跟動畫長度對齊
    public float cooldown       = 1.5f;

    [Header("Damage")]
    public int   damage        = 10;
    public float knockback     = 4f;
}
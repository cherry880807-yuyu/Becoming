using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct DamageInfo
{
    public int damage;
    public Vector2 hitDirection;
    public float knockbackForce;
    public Vector2 attackerPosition;
    public DamageInfo(int damage, Vector2 hitDirection, float knockbackForce,Vector2 attackerPosition)
    {
        this.damage = damage;
        this.hitDirection = hitDirection;
        this.knockbackForce = knockbackForce;
        this.attackerPosition = attackerPosition;
    }
}
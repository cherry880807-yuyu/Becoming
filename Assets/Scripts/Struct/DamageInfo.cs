using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct DamageInfo
{
    public int damage;
    public Vector2 hitDirection;
    public float knockbackForce;
    public float hitStopTime;

    public DamageInfo(int damage,Vector2 hitDirection,float knockbackForce,float hitStopTime)
    {
        this.damage = damage;
        this.hitDirection = hitDirection;
        this.knockbackForce = knockbackForce;
        this.hitStopTime = hitStopTime;
    }
}
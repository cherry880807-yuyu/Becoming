using UnityEngine;

public readonly struct HitConfig
{
    public readonly int Damage;
    public readonly Vector2 HitDirection;
    public readonly float KnockbackForce;
    public readonly Vector2 AttackerPosition;
    public readonly float HitStopTime;

    public HitConfig(
        int damage,
        Vector2 hitDirection,
        float knockbackForce,
        Vector2 attackerPosition,
        float hitStopTime)
    {
        Damage = damage;
        HitDirection = hitDirection;
        KnockbackForce = knockbackForce;
        AttackerPosition = attackerPosition;
        HitStopTime = hitStopTime;
    }
}

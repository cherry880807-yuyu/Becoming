using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Team
{
    Player,
    Enemy,
    Neutral
}

public class HurtBox2D : MonoBehaviour
{
    protected Collider2D col;

    public IDamageable Owner { get; private set; }
    public Team Team;
    private List<IInvincibleSource> sources = new();

    public InvincibleType GetCurrentInvincibleType()
    {
        foreach (var s in sources)
        {
            var type = s.GetInvincibleType();
            if (type != InvincibleType.None) return type;
        }

        return InvincibleType.None;
    }

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        Owner = GetComponentInParent<IDamageable>();
    }

    public void Register(IInvincibleSource source)
    {
        if (!sources.Contains(source)) sources.Add(source);
    }

    public void Unregister(IInvincibleSource source)
    {
        sources.Remove(source);
    }

}
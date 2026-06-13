using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum InvincibleType
{
    None,
    Dash,
    Buff,
}

public static class InvincibleTypeDB
{
    public static readonly Dictionary<InvincibleType, string> Text = new()
    {
        { InvincibleType.Dash, "閃避" },
        { InvincibleType.Buff, "無敵" },
    };
}

public interface IInvincibleSource
{
    InvincibleType GetInvincibleType();
}
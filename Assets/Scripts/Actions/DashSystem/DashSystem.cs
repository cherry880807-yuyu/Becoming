using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSystem
{
    private IDash dash;
    private MonoBehaviour owner;
    public bool IsDashing { get; private set; }

    public DashSystem(IDash dash, MonoBehaviour owner)
    {
        this.dash = dash;
        this.owner = owner;
    }

    public void Execute(Rigidbody2D rb, Vector2 dir)
    {
        if (IsDashing) return;

        owner.StartCoroutine(DashRoutine(rb, dir));
    }

    private IEnumerator DashRoutine(Rigidbody2D rb, Vector2 dir)
    {
        IsDashing = true;
        yield return dash.Dash(rb, dir);
        IsDashing = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSystem : IInvincibleSource
{
    private IDash dash;
    private MonoBehaviour owner;
    private float dashCostReductionPercent;
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

    public float GetModifiedDashCost(float baseCost)
    {
        float multiplier = Mathf.Max(0f, 1f - dashCostReductionPercent / 100f);
        return baseCost * multiplier;
    }

    public void AddDashCostReductionPercent(float percent)
    {
        dashCostReductionPercent += percent;
    }

    public void RemoveDashCostReductionPercent(float percent)
    {
        dashCostReductionPercent -= percent;
    }

    private IEnumerator DashRoutine(Rigidbody2D rb, Vector2 dir)
    {
        IsDashing = true;
        EventBus.Publish(new ResetAttackComboEvent());
        yield return dash.Dash(rb, dir);
        IsDashing = false;
    }

    public InvincibleType GetInvincibleType()
    {
        return IsDashing ? InvincibleType.Dash : InvincibleType.None;
    }
}

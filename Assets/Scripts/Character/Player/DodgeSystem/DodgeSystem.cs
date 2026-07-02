using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeSystem : IInvincibleSource
{
    private IDodge dodge;
    private MonoBehaviour owner;
    private float dodgeCostReductionPercent;
    public bool IsDodging { get; private set; }


    public DodgeSystem(IDodge dodge, MonoBehaviour owner)
    {
        this.dodge = dodge;
        this.owner = owner;
    }

    public void Execute(Rigidbody2D rb, Vector2 dir)
    {
        if (IsDodging) return;

        owner.StartCoroutine(DodgeRoutine(rb, dir));
    }

    public float GetModifiedDodgeCost(float baseCost)
    {
        float multiplier = Mathf.Max(0f, 1f - dodgeCostReductionPercent / 100f);
        return baseCost * multiplier;
    }

    public void AddDodgeCostReductionPercent(float percent)
    {
        dodgeCostReductionPercent += percent;
    }

    public void RemoveDodgeCostReductionPercent(float percent)
    {
        dodgeCostReductionPercent -= percent;
    }

    private IEnumerator DodgeRoutine(Rigidbody2D rb, Vector2 dir)
    {
        IsDodging = true;
        EventBus.Publish(new ResetAttackComboEvent());
        yield return dodge.Dodge(rb, dir);
        IsDodging = false;
    }

    public InvincibleType GetInvincibleType()
    {
        return IsDodging ? InvincibleType.Dodge : InvincibleType.None;
    }
}

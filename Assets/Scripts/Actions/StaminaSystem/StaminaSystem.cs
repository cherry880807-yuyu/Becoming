using UnityEngine;

public class StaminaSystem
{
    private StaminaData data;
    private float current;

    public StaminaSystem(StaminaData data)
    {
        this.data = data;
        current = data.maxStamina;
    }

    // =========================
    // QUERY
    // =========================
    public float GetCurrent() => current;

    public float GetPercent()
    {
        return current / data.maxStamina;
    }

    public bool CanUse(float cost)
    {
        return current >= cost;
    }

    // =========================
    // ACTION
    // =========================
    public void Consume(float cost)
    {
        current -= cost;
        current = Mathf.Max(0, current);
    }

    public void Regen(float deltaTime)
    {
        current += data.regenRate * deltaTime;
        current = Mathf.Clamp(current, 0, data.maxStamina);
    }

    // =========================
    // OPTIONAL (未來 mutation 用)
    // =========================
    public void ModifyMax(float amount)
    {
        data.maxStamina += amount;
        current = Mathf.Clamp(current, 0, data.maxStamina);
    }

    public void ModifyRegen(float amount)
    {
        data.regenRate += amount;
    }
}
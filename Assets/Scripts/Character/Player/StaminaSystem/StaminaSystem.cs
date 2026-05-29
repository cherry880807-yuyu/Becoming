using UnityEngine;

public class StaminaSystem
{
    private StaminaDataSO data;
    private float current;

    public StaminaSystem(StaminaDataSO data)
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
    public bool Consume(float cost)
    {
        if (current < cost) return false;

        current -= cost;
        return true;
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
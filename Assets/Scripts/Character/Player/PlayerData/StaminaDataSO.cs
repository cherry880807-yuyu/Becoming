using UnityEngine;

[CreateAssetMenu(menuName = "Player/Stamina Data")]
public class StaminaDataSO : ScriptableObject
{
    public float maxStamina = 100f;
    public float regenRate = 25f;
}
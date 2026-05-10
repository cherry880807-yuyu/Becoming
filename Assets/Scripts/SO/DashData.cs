using UnityEngine;

[CreateAssetMenu(menuName = "Player/Dash Data")]
public class DashData : ScriptableObject
{
     public float dashDistance = 3f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.8f;
    public float dashCost = 30f;
}
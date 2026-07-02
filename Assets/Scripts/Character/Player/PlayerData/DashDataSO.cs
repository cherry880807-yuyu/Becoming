using UnityEngine;

[CreateAssetMenu(menuName = "Player/Dash Data")]
public class DodgeDataSO : ScriptableObject
{
     public float dodgeDistance = 3f;
    public float dodgeDuration = 0.15f;
    public float dodgeCost = 30f;
}
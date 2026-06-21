using UnityEngine;

[CreateAssetMenu(menuName = "Player/Movement Data")]
public class MovementDataSO : ScriptableObject
{
    [Header("走路/跑步")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;

    [Header("跳躍")]
    public float jumpForce = 12f;
    public int baseAirJumps=0;

     [Header("下墜")]
    public float gravity = -30f;
    public float fallMultiplier = 2.5f;
    public float downPlatformForce=10f;
}
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Movement Data")]
public class MovementData : ScriptableObject
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 12f;
}
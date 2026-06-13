
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Enemy AI Data")]
public class EnemyAIDataSO : CharacterDataSO
{

    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float patrolWaitTime = 1.5f;  // 到達巡邏點後等待時間

    [Header("Chase")]
    public float chaseSpeed = 4.5f;
    public float detectionRange = 6f;    // 發現玩家距離
    public float loseTargetRange = 10f;  // 超過此距離放棄追蹤

    [Header("Combat")]
    public float hurtDuration = 0.4f;    // Hurt 狀態持續時間
    public float knockbackForce = 5f;
}

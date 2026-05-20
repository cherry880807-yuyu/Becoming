using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemy/AttackData/BirdDive AttackData")]
public class BirdAttackDataSO : ScriptableObject
{
    [Header("盤旋行為")]
    public float orbitRadiusX = 10f;   // 橢圓水平半徑，越大繞越寬
    public float orbitRadiusY = 3f;   // 橢圓垂直半徑，小於X = 扁橢圓
    public float orbitHeightOffset = 8f;//盤旋中心在玩家上方幾個單位
    public float orbitSpeed = 90f;//每秒旋轉幾度，越大繞越快
    public float orbitFollowSpeed = 5f;//追上橢圓目標點的速度，太小會脫軌
    public float orbitRadiusLerpSpeed = 2f;//半徑從入場距離收斂到目標的速度

    [Header("俯衝觸發條件")]
    public float diveRange = 10f; //觸發俯衝的最大距離
    public float minHeightAbovePlayer = 6f;//鳥需在玩家上方幾單位才能俯衝

    [Header("俯衝各階段時間")]
    public float aimDuration = 0.5f;//Aim 階段長度，飄向玩家上方
    public float windupDuration = 0.35f;//Windup 蓄力時間，此時鎖定方向
    public float diveDuration = 1.0f;//俯衝飛行時間，不追蹤
    public float recoverDuration = 0.6f;//落地喘息時間，結束後 IsFinished

    [Header("俯衝速度")]
    public float diveSpeed = 15f;//俯衝速度 / Aim 飄移速度

    [Header("俯衝傷害")]
    public int diveDamage = 20;

    [Header("俯衝冷卻")]
    public float diveCooldown = 5f; //從 Exit 起算，幾秒後才能再俯衝



}
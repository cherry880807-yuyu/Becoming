using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//角色成就狀態數據
[Serializable]
public class MutationContext
{
    [Header("死亡次數")]
    public int deathCount;

    [Header("累計攻擊次數")]
    public int totalAttackCount;

    [Header("閃避成功次數")]
    public int dodgeSucceedCount;

     [Header("累計跳躍次數")]
    public int totalJumpCount;

    [Header("累計奔跑距離")]
    public float totalSprintDistance;

    [Header("累計衝刺次數")]
    public int totalDashCount;

    [Header("不依靠篝火回血累計擊殺怪物數")]
    public int enemyKillsSinceCampfireHeal;

}

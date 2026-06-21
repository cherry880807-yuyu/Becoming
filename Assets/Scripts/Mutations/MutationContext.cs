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

}
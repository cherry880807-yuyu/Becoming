using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData : MonoBehaviour
{

}

//成就類型
public struct AttackEnemyEvent
{
    public float hitTime;
}

// Mutation 條件追蹤監聽這個
public struct DodgeSucceededEvent//閃避技能
{
   // public DamageSourceType DamageSource; 未來可追蹤「閃避雷擊幾次」之類的條件
}



public struct MutationUnlockedEvent //解所成就
{
    public MutationDataSO mutation;
}


public struct DamageDealtEvent
{
    public int Damage;
    public Vector3 WorldPosition;
}

public struct HealEvent
{
    public int HealAmount;
    public Vector3 WorldPosition;
}




public struct EnemyDiedEvent
{
    public Vector3 WorldPosition;
    public string EnemyId; // 給 MutationManager 用
}

public struct PlayerDiedEvent
{

}
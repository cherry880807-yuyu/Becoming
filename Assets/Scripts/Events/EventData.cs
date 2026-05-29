using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData : MonoBehaviour
{

}

//成就類型
public struct AttackThreeTimesEvent
{
    public int attackCount;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData : MonoBehaviour
{

}

//---------------------------------------------成就類型
public struct MutationUnlockedEvent //成功解所成就
{
    public MutationDataSO mutation;
}

public struct AttackEnemyEvent//成功攻擊次數
{
    public float hitTime;
}

public struct DodgeSucceededEvent : IFloatingTextEvent  //成功閃避技能
{
    // public DamageSourceType DamageSource; 未來可追蹤「閃避雷擊幾次」之類的條件
    public string text;
    public Vector3 WorldPosition;

    public FloatingTextData GetFloatingText()
    {
        return new FloatingTextData
        {
            text = text,
            Type = FloatingTextType.System,
            WorldPosition = WorldPosition
        };
    }
}

//---------------------------------------------
public struct DamageDealtEvent : IFloatingTextEvent
{
    public int Damage;
    public Vector3 WorldPosition;
    public FloatingTextData GetFloatingText()
    {
        return new FloatingTextData
        {
            text = Damage.ToString(),
            Type = FloatingTextType.Damage,
            WorldPosition = WorldPosition
        };
    }
}

public struct HealEvent : IFloatingTextEvent
{
    public int HealAmount;
    public Vector3 WorldPosition;
    public FloatingTextData GetFloatingText()
    {
        return new FloatingTextData
        {
            text = HealAmount.ToString(),
            Type = FloatingTextType.Heal,
            WorldPosition = WorldPosition,
        };
    }
}
//---------------------------------------------

public struct EnemyDiedEvent
{
    public Vector3 WorldPosition;
    public string EnemyId; // 給 MutationManager 用
}

public struct PlayerDiedEvent
{

}
//---------------------------------------------
public struct ExitRoomEvent
{
    public RoomDataSO nextRoom;
    public Vector2 spawnPosition;
}

//---------------------------------------------
public struct ResetAttackComboEvent{}

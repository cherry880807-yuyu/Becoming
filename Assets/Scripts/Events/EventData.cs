using UnityEngine;

public class EventData : MonoBehaviour
{
}

public struct MutationUnlockedEvent
{
    public MutationDataSO mutation;
}

public struct AttackEnemyEvent
{
    public float hitTime;
}

public struct DodgeSucceededEvent : IFloatingTextEvent
{
    public string text;
    public Vector3 WorldPosition;
    public int incomingDamage;
    public bool wouldBeLethal;

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

public struct JumpEvent
{
}

public struct DashEvent
{
    public Vector3 WorldPosition;
    public Vector2 FacingRight;
}

public struct PlayerSprintDistanceEvent
{
    public float distance;
}

public struct PlayerHealthChangedEvent
{
    public int currentHP;
    public int maxHP;
    public float healthPercent;
}

public struct DeathProtectionStateChangedEvent
{
    public bool isUnlocked;
    public bool isAvailable;
}

public struct CampfireHealEvent
{
    public PlayerBrain player;
    public int healAmount;
}

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

public struct PlayerDamageDealtEvent
{
    public int Damage;
    public Vector3 WorldPosition;
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

public struct EnemyDiedEvent
{
    public Vector3 WorldPosition;
    public string EnemyId;
}

public struct PlayerDiedEvent
{
    public float DiedTime;
    public Vector3 WorldPosition;
}

public struct PlayerRespawnedEvent
{
    public Vector3 RespawnPosition;
}

public struct GameStartedEvent
{
}

public struct ExitRoomEvent
{
    public RoomDataSO nextRoom;
    public Vector2 spawnPosition;
}

public struct ResetAttackComboEvent
{
}

public struct WeaponChangedEvent
{
    public PlayerBrain player;
    public WeaponDataSO weapon;
    public WeaponDataSO[] ownedWeapons;
}

public struct WeaponInventoryChangedEvent
{
    public PlayerBrain player;
    public WeaponDataSO equippedWeapon;
    public WeaponDataSO[] ownedWeapons;
}

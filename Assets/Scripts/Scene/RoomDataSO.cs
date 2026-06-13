using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum ExitDirection { Left, Right, Up, Down, Portal }
[CreateAssetMenu(menuName = "World/Room Data")]
public class RoomDataSO : ScriptableObject
{
    public string roomId;
    public AssetReference sceneReference;   // Addressables
    public RoomEntrance[] entrances;

    public Vector2 GetEntranceSpawnPosition(ExitDirection fromDirection)
    {
        foreach (var entrance in entrances)
        {
            if (entrance.fromDirection == fromDirection)  return entrance.spawnPosition;
        }

        Debug.LogWarning($"[RoomDataSO] {roomId} 找不到 {fromDirection} 的入口，使用預設位置");
        return entrances.Length > 0 ? entrances[0].spawnPosition : Vector2.zero;
    }
}

[Serializable]
public class RoomEntrance
{
    public ExitDirection fromDirection;
    public Vector2 spawnPosition;           // 進入這張地圖時的出生點
}


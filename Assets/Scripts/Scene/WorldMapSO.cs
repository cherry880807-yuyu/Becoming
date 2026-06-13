using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/World Map")]
public class WorldMapSO : ScriptableObject
{
    public ThemeDataSO[] themes;
    public RoomDataSO startRoom; // 遊戲起始房間
}
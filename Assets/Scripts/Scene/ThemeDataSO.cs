using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Theme Data")]
public class ThemeDataSO : ScriptableObject
{
    public string themeId;
    public string themeName;
    public RoomDataSO startRoom;
    public RoomDataSO[] allRooms;
}
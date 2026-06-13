using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomExitTrigger : MonoBehaviour
{
    [SerializeField] private RoomDataSO _nextRoom;
    [SerializeField] private ExitDirection _nextRoomEnterDirection;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var spawnPos = _nextRoom.GetEntranceSpawnPosition(_nextRoomEnterDirection);
        EventBus.Publish(new ExitRoomEvent
        {
            nextRoom=_nextRoom,
            spawnPosition=spawnPos
        });
    }
}
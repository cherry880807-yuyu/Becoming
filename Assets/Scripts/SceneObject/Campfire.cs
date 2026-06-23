using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    [Header("每間隔治療量")]
    [SerializeField] int healPerSecond = 20;
    [Header("治療間隔時間")]
    [SerializeField] float tickRate = 1f;

    [Header("重生點")]
    [SerializeField] private RoomDataSO _roomData;

    [SerializeField] PlayerBrain player;
    private float timer;

    private Animator animator;

    void Awake()
    {
        animator=GetComponent<Animator>();
    }
    public void SetRegistered(bool isRegistered)
    {
        animator.SetBool("IsSpawnCampFire",isRegistered);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerBrain>() is PlayerBrain pb)
        {
            player = pb;
            RespawnManager.Instance.RegisterRespawnPoint(_roomData, this);
        }


        //TODO 設定重生點
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerBrain>() is PlayerBrain pb) if (player == pb) player = null;
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer >= tickRate)
        {
            player.Heal(healPerSecond);
            EventBus.Publish(new CampfireHealEvent
            {
                player = player,
                healAmount = healPerSecond
            });
            timer = 0f;
        }
    }
}

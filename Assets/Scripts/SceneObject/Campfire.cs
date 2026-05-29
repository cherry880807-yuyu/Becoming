using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    [Header("每間隔治療量")]
    [SerializeField] int healPerSecond = 20;
    [Header("治療間隔時間")]
    [SerializeField] float tickRate = 1f; 

    private PlayerBrain player;
    private float timer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerBrain pb)) player = pb;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerBrain pb)) if (player == pb) player = null;
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer >= tickRate)
        {
            player.Heal(healPerSecond);
            timer = 0f;
        }
    }
}

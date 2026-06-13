using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStopEffect : MonoBehaviour
{

    private Coroutine _coroutine;
    private void OnEnable()
    {
        EventBus.Subscribe<AttackEnemyEvent>(OnEnemyAttacked);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<AttackEnemyEvent>(OnEnemyAttacked);
    }

    private void OnEnemyAttacked(AttackEnemyEvent e)
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(HitStop(e.hitTime));
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}

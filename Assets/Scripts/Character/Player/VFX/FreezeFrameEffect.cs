using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FreezeFrameEffect : MonoBehaviour
{
    [SerializeField] float freezeDuration = 0.06f;
    private Coroutine _coroutine;
    private void OnEnable()
    {
        EventBus.Subscribe<DodgeSucceededEvent>(OnPlayerDodgeSucceeded);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<DodgeSucceededEvent>(OnPlayerDodgeSucceeded);
    }

    private void OnPlayerDodgeSucceeded(DodgeSucceededEvent e)
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(FreezeRoutine());
    }

    private IEnumerator FreezeRoutine()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(freezeDuration); 
        Time.timeScale = 1f;
        _coroutine = null;
    }


}
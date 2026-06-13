using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TransitionFader : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration = 0.4f;
    void Awake()
    {
        _canvasGroup=GetComponent<CanvasGroup>();
        _canvasGroup.alpha=0f;
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(0f, 1f);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        _canvasGroup.alpha = from;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = to;
    }
}
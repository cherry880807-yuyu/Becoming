using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public enum FloatingTextType { Noraml, Damage, Heal, System }

public struct FloatingTextData
{
    public string text;
    public FloatingTextType Type;
    public Vector3 WorldPosition;
}

[RequireComponent(typeof(TextMeshPro))]
public class FloatingText : MonoBehaviour
{
    private TextMeshPro _tmp;

    // 設定從外部注入，不寫死
    private FloatingTextConfig _config;


    public void Init(FloatingTextConfig config)
    {
        _config = config;
        _tmp = GetComponent<TextMeshPro>();
    }

    /// <summary>Pool 取出時呼叫，設定內容並開始動畫</summary>
    public void Play(FloatingTextData data)
    {
        transform.position = data.WorldPosition;
        transform.localScale = Vector3.one;

        switch (data.Type)
        {
            case FloatingTextType.Noraml:
                _tmp.text = data.text;
                _tmp.color = _config.normalColor;
                break;
            case FloatingTextType.Damage:
                _tmp.text = $"-{data.text}";
                _tmp.color = _config.damageColor;
                break;

            case FloatingTextType.Heal:
                _tmp.text = $"+{data.text}";
                _tmp.color = _config.healColor;
                break;
            case FloatingTextType.System:
                _tmp.text = data.text;
                _tmp.color = _config.systemColor;
                break;
        }


        _tmp.fontSize = _config.normalFontSize;
        StopAllCoroutines();
        StartCoroutine(FloatAndFade());
    }

    private IEnumerator FloatAndFade()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Color startColor = _tmp.color;

        while (elapsed < _config.duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _config.duration;

            // 往上飄
            transform.position = startPos + Vector3.up * (_config.floatHeight * t);

            // 後半段淡出
            float alpha = t < 0.5f ? 1f : 1f - ((t - 0.5f) / 0.5f);
            _tmp.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // 歸還 Pool
        FloatingTextPool.Instance.Return(this);
    }
}
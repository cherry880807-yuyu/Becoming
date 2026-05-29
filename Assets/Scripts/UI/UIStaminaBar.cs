using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class UIStaminaBar : MonoBehaviour
{
    [SerializeField] private PlayerBrain player;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Colors")]
    [SerializeField] private Gradient staminaGradient;
    [Header("Show Timer")]
    [SerializeField] private float visibleDuration = 2f;
    [SerializeField] private float fadeSpeed = 5f;

    private Image staminaBar;


    private float visibleTimer;
    private float lastPercent = 1f;



    void Awake()
    {
        staminaBar = GetComponent<Image>();
    }
    void Update()
    {
        float percent = player.PlayerActorData.StaminaSystem.GetPercent();
        staminaBar.fillAmount = percent;
        staminaBar.color = staminaGradient.Evaluate(percent);

        if (!Mathf.Approximately(percent, lastPercent))
        {
            visibleTimer = visibleDuration;
            lastPercent = percent;
        }
        visibleTimer -= Time.deltaTime;

        float targetAlpha = visibleTimer > 0 ? 1f : 0f;

        canvasGroup.alpha = Mathf.Lerp(
            canvasGroup.alpha,
            targetAlpha,
            Time.deltaTime * fadeSpeed
        );
    }
}

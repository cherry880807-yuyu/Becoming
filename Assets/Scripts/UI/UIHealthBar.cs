using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] BaseBrain target;
    private Image hpBar;

    private float displayHP=1;
    private float targetHP;

    float smoothSpeed = 1f;

    void Awake()
    {
        hpBar = GetComponent<Image>();
    }
    void OnEnable()
    {
        target.OnHPChanged += UpdateBar;
    }
    void OnDisable()
    {
        target.OnHPChanged -= UpdateBar;
    }

    void Start()
    {
        UpdateBar(target.MaxHP);
    }
    void Update()
    {
        displayHP = Mathf.MoveTowards(displayHP, targetHP, smoothSpeed * Time.deltaTime);
        hpBar.fillAmount = displayHP;
    }


    void UpdateBar(int hp)
    {
        targetHP = hp / (float)target.MaxHP;
    }


}
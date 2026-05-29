using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem
{

    private PlayerActorData actorData;
    private readonly ComboDataSO comboData;
    private readonly ComboInputBuffer _buffer;

    private readonly float _comboWindowStart = 0.6f;

    public int currentComboStepIndex { get; private set; }
    private bool _isAttacking;
    private bool _comboChecked;


    public int BaseDamage => comboData.steps[currentComboStepIndex].damage;
    public int FinalDamage => BaseDamage + bonusDamage;
    private int bonusDamage;

    public AttackSystem(PlayerActorData actorData, ComboDataSO comboData)
    {
        this.actorData = actorData;
        this.comboData = comboData;
        _buffer = new ComboInputBuffer();
    }
    public void OnAttackInput()
    {
        if (!_isAttacking)
        {
            StartCombo(actorData);
        }
        else
        {
            _buffer.Enqueue();
        }
    }
    public void EvaluateCombo()
    {
        if (!_isAttacking) return;

        float normalizedTime = actorData.AnimationSystem.GetAttackNormalizedTime();

        // 離開攻擊狀態（動畫播完）
        if (normalizedTime < 0f)
        {
            EndCombo();
            return;
        }

        // 進入 Combo Window，檢查一次
        if (!_comboChecked && normalizedTime >= _comboWindowStart)
        {
            _comboChecked = true;

            if (_buffer.Dequeue() && currentComboStepIndex < comboData.steps.Length)
            {
                PlayStep(actorData);
            }
        }
    }

    private void StartCombo(PlayerActorData actorData)
    {
        _isAttacking = true;
        currentComboStepIndex = 0;
        PlayStep(actorData);
    }

    private void PlayStep(PlayerActorData actorData)
    {
        _comboChecked = false;
        actorData.AnimationSystem.PlayAttack(currentComboStepIndex);
        currentComboStepIndex++;
    }

    private void EndCombo()
    {
        _isAttacking = false;
        currentComboStepIndex = 0;
        _comboChecked = false;
        _buffer.Clear();
    }



    public void AddBonusDamage(int amount)
    {
        bonusDamage += amount;
    }

    public void RemoveBonusDamage(int amount)
    {
        bonusDamage -= amount;
    }


}
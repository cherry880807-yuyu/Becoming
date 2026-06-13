using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class AttackSystem
{

    private PlayerActorData _actorData;
    private WeaponDataSO _currentWeapon;// 可切換
    private ComboType _currentComboType;
    private ComboStep[] _currentSteps;

    private readonly ComboInputBuffer _buffer;

    private readonly float _comboWindowStart = 0.3f;
    private readonly float _comboWindowEnd = 0.75f;

    private int currentComboStepIndex = 0;
    public bool IsAttacking { get; private set; }
    private bool _attackStarted;
    private bool _canQueueNext;
    private float _comboEndTime;

    public ComboStep CurrentStep { get; private set; }

    public int BaseDamage { get; private set; }
    public int FinalDamage => BaseDamage + bonusDamage;
    private int bonusDamage;


    public AttackSystem(PlayerActorData actorData, WeaponDataSO defaultWeapon)
    {
        _actorData = actorData;
        _currentWeapon = defaultWeapon;
        _buffer = new ComboInputBuffer();
        EventBus.Subscribe<ResetAttackComboEvent>(OnComboReset);
    }
    public void Dispose()
    {
        EventBus.Unsubscribe<ResetAttackComboEvent>(OnComboReset);
    }
    public void OnAttackInput()
    {

        if (!IsAttacking)
        {
            ComboType newType = ResolveComboType();
            if (newType != _currentComboType)
            {
                EndCombo(true);
                _currentComboType = newType;
            }
            _currentSteps = _currentWeapon.GetSteps(newType);
            Debug.Log(ResolveComboType());
            StartCombo(_actorData);
            return;
        }
        if (_canQueueNext) _buffer.Enqueue();
    }
    // 武器切換（換武器時呼叫）
    public void SetWeapon(WeaponDataSO weapon)
    {
        _currentWeapon = weapon;
        EndCombo(true);
    }
    //------------------------------------------

    public void EvaluateCombo()
    {
        if (!IsAttacking)
        {
            if (_comboEndTime > 0f && Time.time - _comboEndTime > _currentWeapon.resetTime)
                EndCombo(true);
            return;
        }

        float normalizedTime = _actorData.AnimationSystem.GetAttackNormalizedTime();

        if (normalizedTime >= 0) _attackStarted = true;

        if (_attackStarted && normalizedTime < 0f)
        {
            IsAttacking = false;
            _comboEndTime = Time.time;
            if (_buffer.Dequeue())
            {
                IsAttacking = true;
                PlayStep(_actorData);
                return;
            }
            _buffer.Clear();
            return;
        }
        _canQueueNext = normalizedTime >= _comboWindowStart && normalizedTime <= _comboWindowEnd;
    }

    //------------------------------
    private ComboType ResolveComboType()
    {
        if (_actorData.MovementSystem.IsSprint) return ComboType.Dash;
        if (!_actorData.MovementSystem.IsGrounded) return ComboType.Air;
        return ComboType.Ground;
    }

    private void StartCombo(PlayerActorData _actorData)
    {
        IsAttacking = true;
        PlayStep(_actorData);
    }

    private void PlayStep(PlayerActorData _actorData)
    {
        _attackStarted = false;
        CurrentStep = _currentSteps[currentComboStepIndex];
        BaseDamage = CurrentStep.damage;
        Debug.Log($"Play Attack Step : {currentComboStepIndex},Combo Type:{ResolveComboType()}");
        _actorData.AnimationSystem.PlayAttack(currentComboStepIndex, ResolveComboType());
        currentComboStepIndex++;
        if (currentComboStepIndex >= _currentSteps.Length) currentComboStepIndex = 0;
    }

    private void EndCombo(bool resetStep = false)
    {
        IsAttacking = false;
        _attackStarted = false;
        _buffer.Clear();
        _comboEndTime = 0f;
        if (resetStep) currentComboStepIndex = 0;
    }
    private void OnComboReset(ResetAttackComboEvent _)
    {
        EndCombo(true);
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
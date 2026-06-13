using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用敵人 Brain
/// 不做攻擊邏輯，攻擊由子類別 override StartAttackPattern()
/// </summary>
public class EnemyBrain : BaseBrain, IDamageable
{
    [Header("Basic Enemy AI Data")]
    [SerializeField] protected EnemyAIDataSO enemyAIData;
    [SerializeField] private bool canPatrol = true;
    [SerializeField] private Transform[] _patrolPoints;


    // ── StateMachine ────────────────────────────────────
    protected StateMachine _stateMachine;

    protected EnemyIdleState _idleState;
    protected EnemyPatrolState _patrolState;
    protected EnemyChaseState _chaseState;
    protected EnemyAttackState _attackState;
    protected EnemyHurtState _hurtState;
    protected EnemyDeadState _deadState;

    // ── Actor Data（Component 快取） ─────────────────────
    public EnemyActorData ActorData { get; private set; }

    // ── Hit Flash ───────────────────────────────────────
    private Color _originalColor;
    private Coroutine _flashRoutine;
    // WaitForSeconds 靜態快取，避免每次 new
    private static readonly WaitForSeconds _flashWait = new WaitForSeconds(0.1f);

    // ────────────────────────────────────────────────────


    protected override void Awake()
    {
        SetMaxHP(enemyAIData.maxHP);
        SetShield(enemyAIData.maxShield);
        BuildActorData();
        BuildStates();
        BuildStateMachine();
        _originalColor = ActorData.SpriteRenderer.color;
    }
    protected override void Start()
    {
        // Player 定位走 IPlayerLocator，解耦 FindGameObjectWithTag
        ActorData.PlayerTransform = PlayerLocator.Instance.PlayerTransform;
    }

    // ── Build ────────────────────────────────────────────
    private void BuildActorData()
    {
        ActorData = new EnemyActorData(
            this,
            transform,
            GetComponent<Rigidbody2D>(),
            GetComponent<Animator>(),
            GetComponent<Collider2D>(),
            GetComponent<SpriteRenderer>(),
            enemyAIData
        );
    }

    /// <summary>子類別可 override 新增額外 State</summary>
    protected virtual void BuildStates()
    {
        _idleState = new EnemyIdleState(ActorData);
        _patrolState = new EnemyPatrolState(ActorData, _patrolPoints);
        _chaseState = new EnemyChaseState(ActorData, new GroundChasePattern(ActorData.EnemyAIDataSO.chaseSpeed));
        //_attackState = new EnemyAttackState(ActorData,new GroundMeleeAttack());
        _hurtState = new EnemyHurtState(ActorData);
        _deadState = new EnemyDeadState(ActorData);
    }

    protected virtual void BuildStateMachine()
    {
        _stateMachine = new StateMachine();
        if (canPatrol) _stateMachine.Initialize(_patrolState);
        else _stateMachine.Initialize(_idleState);
    }

    // ── Unity Loop ───────────────────────────────────────
    private void Update()
    {
        _stateMachine.Update(Time.deltaTime);
        HandleTransitions();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate(Time.fixedDeltaTime);
    }

    // ── Transitions ──────────────────────────────────────
    /// <summary>
    /// 子類別可 override 加入自己的 Transition 邏輯
    /// 記得呼叫 base.HandleTransitions() 保留通用規則
    /// </summary>
    protected virtual void HandleTransitions()
    {
        var current = _stateMachine.CurrentState;

        if (!IsAlive && current != _deadState)
        {
            _stateMachine.ChangeState(_deadState);
            return;
        }

        if (current == _hurtState && _hurtState.IsFinished)
        {

            if (_stateMachine.PreviousState == _attackState || _stateMachine.PreviousState == null)
            {
                _stateMachine.ChangeState(_chaseState);
                return;
            }
            _stateMachine.RevertToPreviousState();
            return;
        }

        if (current == _deadState || current == _hurtState) return;

        if (_attackState != null)
        {

            if (current == _attackState && _attackState.IsFinished)
            {
                _stateMachine.ChangeState(_chaseState);
                return;
            }
            if (current == _chaseState && _attackState.HasReadyPattern())
            {
                _stateMachine.ChangeState(_attackState);
                return;
            }

        }

        if ((current == _idleState || current == _patrolState) && ActorData.CanDetectPlayer())
        {
            _stateMachine.ChangeState(_chaseState);
            return;
        }

        if (current == _chaseState && !ActorData.CanDetectPlayer() && ActorData.DistanceToPlayer() > enemyAIData.loseTargetRange)
        {
            if (canPatrol) _stateMachine.ChangeState(_patrolState);
            else _stateMachine.ChangeState(_idleState);
            return;
        }



    }


    // ── IDamageable ──────────────────────────────────────
    public void TakeDamage(DamageInfo info)
    {
        if (!IsAlive) return;
        ApplyDamage(info.damage, info.hitDirection, info.knockbackForce);
        if (_flashRoutine != null) StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(HitFlash());

        if (!IsAlive)
        {
            _stateMachine.ChangeState(_deadState); // 直接切 Dead，不過 Hurt
            return;
        }

        bool isAttacking = _attackState != null && _stateMachine.CurrentState == _attackState;
        if (isAttacking && !_attackState.CurrentPatternCanBeInterrupted) return;
        if (isAttacking && _attackState.CurrentPatternCanBeInterrupted) _attackState.ForceExit();
        _stateMachine.ChangeState(_hurtState);

    }

    protected override void OnApplyKnockback(Vector2 dir, float force)
    {
        _hurtState.SetKnockback(dir,force);
    }

    protected override void HandleDeath()
    {
        _stateMachine.ChangeState(_deadState);

        // 跨系統廣播
        EventBus.Publish(new EnemyDiedEvent
        {
            WorldPosition = transform.position,
            EnemyId = enemyAIData.characterId
        });
    }


    private IEnumerator HitFlash()
    {
        ActorData.SpriteRenderer.color = Color.red;
        yield return _flashWait;
        ActorData.SpriteRenderer.color = _originalColor;
    }

    // ── Death ────────────────────────────────────────────
    // public void OnDeathAnimationEnd() => _deadState.OnDeathAnimationEnd();
    public void OnDeathAnimationEnd()
    {
        // TODO: 接 Event Bus → MutationManager / LootSystem
        // GameEventBus.Emit(new EnemyDiedEvent(transform.position, enemyAIData.enemyId));
        Destroy(gameObject, 5f);
    }

}
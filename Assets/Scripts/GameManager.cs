using System.Collections;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Playing,
    Dying,
    Respawning
}

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu;
     

    [Header("設定")]
    [SerializeField] private float _dyingSlowMotionScale = 0.1f;
    [SerializeField] private float _dyingDuration = 1.5f;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
        EventBus.Subscribe<PlayerRespawnedEvent>(OnPlayerRespawned);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
        EventBus.Unsubscribe<PlayerRespawnedEvent>(OnPlayerRespawned);
    }

    // ── 外部呼叫入口 ──────────────────────────────────────
    public void StartGame()
    {
        ChangeState(GameState.Playing);
        EventBus.Publish(new GameStartedEvent());
    }

    // ── Event 訂閱 ────────────────────────────────────────
    private void OnPlayerDied(PlayerDiedEvent e)
    {
        if (CurrentState != GameState.Playing) return;
        StartCoroutine(DyingRoutine());
    }

    private void OnPlayerRespawned(PlayerRespawnedEvent e)
    {
        ChangeState(GameState.Playing);
    }

    // ── 死亡流程 ──────────────────────────────────────────
    private IEnumerator DyingRoutine()
    {
        ChangeState(GameState.Dying);

        // 慢動作
        Time.timeScale = _dyingSlowMotionScale;
        yield return new WaitForSecondsRealtime(_dyingDuration);
        Time.timeScale = 1f;

        ChangeState(GameState.Respawning);
        RespawnManager.Instance.Respawn();
    }

    // ── 工具 ──────────────────────────────────────────────
    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"[GameFlow] → {newState}");
    }
}
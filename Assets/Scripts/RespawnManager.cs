using System;
using System.Collections;
using UnityEngine;

public class RespawnManager : Singleton<RespawnManager>
{
    [Header("設定")]
    [SerializeField] private float _respawnHPPercent = 1f;
    [SerializeField] private float _respawnDelay = 0.8f;

    private Campfire _currentCampfire;
    private RoomDataSO _respawnRoom;
    private Vector3 _respawnPosition;
    private bool _hasRespawnPoint = false;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    // ── Campfire 呼叫這個來登記重生點 ────────────────────
    public void RegisterRespawnPoint(RoomDataSO room, Campfire campfire)
    {
        if (_currentCampfire != null && _currentCampfire != campfire) _currentCampfire.SetRegistered(false);

        _currentCampfire = campfire;
        _respawnRoom = room;
        _respawnPosition = campfire.transform.position;
        _hasRespawnPoint = true;
         campfire.SetRegistered(true);
        Debug.Log($"[RespawnSystem]登記重生點：{_respawnPosition}");
    }

    // ── GameFlowManager 呼叫 ─────────────────────────────
    public void Respawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSecondsRealtime(_respawnDelay);

        RoomDataSO targetRoom;
        Vector2 targetPosition;

        if (_hasRespawnPoint)
        {
            targetRoom = _respawnRoom;
            targetPosition = _respawnPosition;
        }
        else
        {
            targetRoom = SceneController.Instance.StartRoom;
            targetPosition = Vector2.zero;
            Debug.LogWarning("[RespawnManager] 無重生點，回起始房間");
        }

        SceneController.Instance.TransitionTo(targetRoom, targetPosition);

        bool roomLoaded = false;
        Action onLoaded = () => roomLoaded = true;
        SceneController.Instance.OnRoomLoaded += onLoaded;
        yield return new WaitUntil(() => roomLoaded);
        SceneController.Instance.OnRoomLoaded -= onLoaded;

        // 恢復 HP
        var player = PlayerLocator.Instance.PlayerBrain;

        if (player == null)
        {
            Debug.LogError("[RespawnManager] PlayerBrain 取得失敗");
            yield break;
        }

        player.Respawn(Mathf.RoundToInt(player.MaxHP * _respawnHPPercent));
        player.enabled = true;


        EventBus.Publish(new PlayerRespawnedEvent
        {
            RespawnPosition = _respawnPosition
        });
    }
}

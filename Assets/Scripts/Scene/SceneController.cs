using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
public class SceneController : Singleton<SceneController>
{

    [SerializeField] private WorldMapSO _worldMap;
    [SerializeField] private TransitionFader _fader; // 淡入淡出 UI

    private RoomDataSO _currentRoom;

    private bool _isTransitioning;
    private bool _hasLoadedScene;
    private AsyncOperationHandle<SceneInstance> _currentSceneHandle;

    public event Action OnRoomLoaded;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ExitRoomEvent>(OnExitRoom);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ExitRoomEvent>(OnExitRoom);
    }

    void Start()
    {
        //StartCoroutine(LoadPersistent());
        TransitionTo(_worldMap.startRoom, new Vector2(0f, 0f));

    }
    private IEnumerator LoadPersistent()
    {
        var handle = SceneManager.LoadSceneAsync("PersistentScene", LoadSceneMode.Additive);
        yield return handle;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("PersistentScene"));
    }

    void OnExitRoom(ExitRoomEvent e)
    {
        TransitionTo(e.nextRoom, e.spawnPosition);
    }

    public void TransitionTo(RoomDataSO nextRoom, Vector2 spawnPos)
    {
        if (_isTransitioning) return;
        StartCoroutine(TransitionRoutine(nextRoom, spawnPos));
    }


    private IEnumerator TransitionRoutine(RoomDataSO nextRoom, Vector2 spawnPos)
    {
        _isTransitioning = true;

        yield return _fader.FadeOut();

        var loadHandle = nextRoom.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        yield return loadHandle;
        SceneManager.SetActiveScene(loadHandle.Result.Scene);

        PlayerLocator.Instance.SetPosition(spawnPos);
        OnRoomLoaded?.Invoke();


        if (_hasLoadedScene && _currentSceneHandle.IsValid()) yield return Addressables.UnloadSceneAsync(_currentSceneHandle);

        _currentSceneHandle = loadHandle;
        _hasLoadedScene = true;
        _currentRoom = nextRoom;

        yield return _fader.FadeIn();

        _isTransitioning = false;
    }
}

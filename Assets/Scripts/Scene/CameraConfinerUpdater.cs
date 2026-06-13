using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CinemachineConfiner2D))]
public class CameraConfinerUpdater : MonoBehaviour
{

    private CinemachineConfiner2D _confiner;
    void Awake()
    {
        _confiner = GetComponent<CinemachineConfiner2D>();
    }
    void Start()
    {
        SceneController.Instance.OnRoomLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneController.Instance.OnRoomLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded()
    {
        StartCoroutine(ApplyConfinerNextFrame());
    }

    private IEnumerator ApplyConfinerNextFrame()
    {
        yield return null; 

        var go = GameObject.FindGameObjectWithTag("CameraConfiner");

        if (go == null)
        {
            Debug.LogWarning("CameraConfiner not found in scene");
            yield break;
        }

        var collider = go.GetComponent<PolygonCollider2D>();

        if (collider == null)
        {
            Debug.LogWarning("PolygonCollider2D missing on CameraConfiner");
            yield break;
        }

        _confiner.m_BoundingShape2D = collider;
        _confiner.InvalidateCache();
    }
}
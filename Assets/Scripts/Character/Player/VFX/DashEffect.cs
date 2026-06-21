using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffect : MonoBehaviour
{
    [SerializeField] private GameObject dustPrefab;

    void OnEnable()
    {
        EventBus.Subscribe<DashEvent>(SpawnDashDust);
    }
    void OnDisable()
    {
        EventBus.Unsubscribe<DashEvent>(SpawnDashDust);
    }
    void SpawnDashDust(DashEvent e)
    {
        Quaternion rotation = e.FacingRight.x > 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

        Instantiate(
            dustPrefab,
            e.WorldPosition,
            rotation
        );

    }
}

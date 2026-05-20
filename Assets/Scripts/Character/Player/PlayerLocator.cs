using UnityEngine;


public class PlayerLocator : Singleton<PlayerLocator>
{

    public Transform PlayerTransform { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    
    public void Register(Transform playerTransform)
    {
        PlayerTransform = playerTransform;
    }
}
using UnityEngine;


public class PlayerLocator : Singleton<PlayerLocator>
{

    public Transform PlayerTransform { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void Register(Transform playerTransform)
    {
        PlayerTransform = playerTransform;
    }
    public void SetPosition(Vector2 position)
    {
        PlayerTransform.position = position;
        PlayerTransform.GetComponent<PlayerBrain>().PlayerActorData.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}
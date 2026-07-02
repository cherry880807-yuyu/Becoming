using System.Collections;
using UnityEngine;


public class PlayerLocator : Singleton<PlayerLocator>
{
    public PlayerBrain PlayerBrain { get; private set; }
    public Transform PlayerTransform { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void Register(Transform playerTransform)
    {
        PlayerTransform = playerTransform;
        PlayerBrain = playerTransform.GetComponent<PlayerBrain>();
    }
    public void Unregister(Transform playerTransform)
    {
        if (PlayerTransform == playerTransform)
        {
            PlayerTransform = null;
            PlayerBrain = null;
        }
    }
    public void SetPosition(Vector2 position)
    {
        EnsurePlayerReference();
        if (PlayerBrain == null || PlayerTransform == null || PlayerBrain.PlayerActorData == null)
        {
            Debug.LogError("[PlayerLocator] PlayerBrain is not registered, cannot set player position.");
            return;
        }

        var collider = PlayerBrain.PlayerActorData.Collider;
        collider.enabled = false;

        PlayerTransform.position = position;
        PlayerBrain.PlayerActorData.Rigidbody.bodyType = RigidbodyType2D.Dynamic;

        PlayerBrain.StartCoroutine(ReenableCollider(collider));
    }
    private IEnumerator ReenableCollider(Collider2D collider)
    {
        yield return null; // 等一幀
        collider.enabled = true;
    }
    private void EnsurePlayerReference()
    {
        if (PlayerBrain != null && PlayerTransform != null)
            return;

        PlayerBrain player = FindObjectOfType<PlayerBrain>();
        if (player != null)
            Register(player.transform);
    }
}

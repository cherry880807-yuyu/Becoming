using UnityEngine;

public class EnemyBrain : BaseBrain
{
    public ActorData ActorData { get; private set; }

    [SerializeField] private float moveSpeed = 3f;

    private Transform target;

    private void Awake()
    {
        ActorData = new ActorData
        {

           Rigidbody = GetComponent<Rigidbody2D>(),
            SpriteRenderer = GetComponent<SpriteRenderer>()
        };

        target = GameObject.FindGameObjectWithTag("Player").transform;
    }


    protected override void Update()
    {
        if (target == null) return;
    }

}
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextPool : Singleton<FloatingTextPool>
{
    [SerializeField] private FloatingText _prefab;
    [SerializeField] private FloatingTextConfig _config;

    [SerializeField] private int _initialSize = 10;

    private readonly Queue<FloatingText> _pool = new();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < _initialSize; i++) Return(CreateNew());
    }
    void OnEnable()
    {
        EventBus.Subscribe<DamageDealtEvent>(OnDamage);
        EventBus.Subscribe<HealEvent>(OnHeal);
        EventBus.Subscribe<DodgeSucceededEvent>(OnDodgeSucceeded);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<DamageDealtEvent>(OnDamage);
        EventBus.Unsubscribe<HealEvent>(OnHeal);
        EventBus.Unsubscribe<DodgeSucceededEvent>(OnDodgeSucceeded);
    }

    private void OnDamage(DamageDealtEvent e)
    {
        Spawn(new FloatingTextData
        {
            text = e.Damage.ToString(),
            Type = FloatingTextType.Damage,
            WorldPosition = e.WorldPosition,
        });
    }
    private void OnHeal(HealEvent e)
    {
        Spawn(new FloatingTextData
        {
            text = e.HealAmount.ToString(),
            Type = FloatingTextType.Heal,
            WorldPosition = e.WorldPosition,
        });
    }

    private void OnDodgeSucceeded(DodgeSucceededEvent e)
    {
        Spawn(new FloatingTextData
        {
            text = e.text.ToString(),
            Type = FloatingTextType.System,
            WorldPosition = e.WorldPosition,
        });
    }


    private void Spawn(FloatingTextData data)
    {
        var text = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();
        text.gameObject.SetActive(true);
        text.Play(data);
    }

    public void Return(FloatingText text)
    {
        text.gameObject.SetActive(false);
        _pool.Enqueue(text);
    }

    private FloatingText CreateNew()
    {
        var obj = Instantiate(_prefab, transform);
        obj.Init(_config);
        return obj;
    }
}
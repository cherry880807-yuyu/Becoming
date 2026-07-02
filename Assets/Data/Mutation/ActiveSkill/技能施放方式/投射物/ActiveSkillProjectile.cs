using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public sealed class ActiveSkillProjectile : MonoBehaviour, IActiveSkillSpawnedObject
{

    private Color fallbackColor = new Color(0.45f, 0.9f, 1f, 0.75f);
    private static Sprite fallbackSprite;

    private SpriteRenderer visual;
    private readonly HashSet<IDamageable> damagedTargets = new();
    private Vector2 direction = Vector2.right;
    private HitConfig hitConfig;
    private float speed;
    private float lifetime;
    private bool destroyOnHit;
    private int maxHitCount;
    private float expiresAt;
    private bool initialized;

    private void Awake()
    {
        Collider2D projectileCollider = GetComponent<Collider2D>();
        projectileCollider.isTrigger = true;
        visual = GetComponentInChildren<SpriteRenderer>();
        EnsureFallbackVisual();
    }

    private void OnEnable()
    {
        expiresAt = float.PositiveInfinity;
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * (speed * Time.deltaTime));

        if (Time.time >= expiresAt)
            Destroy(gameObject);
    }

    public void Initialize(ActiveSkillContext context, ActiveSkillProjectileConfig config)
    {
        direction = config.direction.sqrMagnitude > 0f ? config.direction.normalized : Vector2.right;
        speed = Mathf.Max(0f, config.speed);
        lifetime = Mathf.Max(0f, config.lifetime);
        destroyOnHit = config.destroyOnHit;
        maxHitCount = config.maxHitCount;
        hitConfig = config.hitConfig;
        expiresAt = Time.time + lifetime;
        initialized = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized || hitConfig.Damage <= 0) return;
        if (other == null || !other.TryGetComponent(out HurtBox2D hurtBox)) return;
        if (hurtBox.Team == Team.Player) return;

        IDamageable damageable = hurtBox.Owner;
        if (damageable == null || !damagedTargets.Add(damageable)) return;

        InvincibleType invincibleType = hurtBox.GetCurrentInvincibleType();
        if (invincibleType != InvincibleType.None)
        {
            EventBus.Publish(new DodgeSucceededEvent
            {
                text = InvincibleTypeDB.Text.TryGetValue(invincibleType, out string text) ? text : "Dodge",
                WorldPosition = hurtBox.transform.position + Vector3.up * 1.5f,
                incomingDamage = hitConfig.Damage,
                wouldBeLethal = hurtBox.Owner is BaseBrain brain &&
                                Mathf.Max(0, hitConfig.Damage - brain.CurrentShield) >= brain.CurrentHP
            });
            return;
        }

        EventBus.Publish(new PlayerDamageDealtEvent
        {
            Damage = hitConfig.Damage,
            WorldPosition = hurtBox.transform.position
        });

        damageable.TakeDamage(hitConfig);
        EventBus.Publish(new AttackEnemyEvent { hitTime = hitConfig.HitStopTime });

        if (destroyOnHit || (maxHitCount > 0 && damagedTargets.Count >= maxHitCount))
            Destroy(gameObject);
    }

    private void EnsureFallbackVisual()
    {
        if (visual != null && visual.sprite != null) return;

        if (visual == null)
        {
            GameObject visualObject = new GameObject("Visual");
            visualObject.transform.SetParent(transform, false);
            visualObject.transform.localScale = new Vector3(2f, 0.35f, 1f);
            visual = visualObject.AddComponent<SpriteRenderer>();
        }

        visual.sprite = GetFallbackSprite();
        visual.color = fallbackColor;
    }

    private static Sprite GetFallbackSprite()
    {
        if (fallbackSprite != null) return fallbackSprite;

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        fallbackSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        return fallbackSprite;
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Active Skill Cast/Spawn Prefab")]
public sealed class SpawnPrefabActiveSkillCastEffect : ActiveSkillCastEffect //其中一種 CastEffect，負責生成 prefab，然後把方向、速度、生命週期、最大命中數、已算好的傷害資料打包成ActiveSkillProjectileConfig交給生成物
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2 spawnOffset = new Vector2(1f, 0f);
    [SerializeField] private bool mirrorOffsetByFacing = true;
    [SerializeField] private bool mirrorScaleByFacing = true;
    [Header("Projectile")]
    [SerializeField] private float initialSpeed;
    [SerializeField] private float lifetime = 1.2f;
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private int maxHitCount = 1;

    public override bool CanExecute(ActiveSkillCastRequest request)
    {
        return base.CanExecute(request) && request.Skill?.damageProfile != null;
    }

    public override void Execute(ActiveSkillCastRequest request)
    {
        ActiveSkillContext context = request.Context;
        float facingX = context.Facing.x < 0f ? -1f : 1f;
        Vector2 offset = spawnOffset;
        if (mirrorOffsetByFacing) offset.x *= facingX;

        Vector2 spawnPosition = context.Origin + (Vector3)offset;
        if (prefab == null)
        {
            Debug.LogWarning($"{nameof(SpawnPrefabActiveSkillCastEffect)} requires a prefab.", this);
            return;
        }

        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);

        if (mirrorScaleByFacing)
        {
            Vector3 scale = instance.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * facingX;
            instance.transform.localScale = scale;
        }

        if (!instance.TryGetComponent(out IActiveSkillSpawnedObject spawnedObject))
        {
            Debug.LogWarning($"{prefab.name} does not have {nameof(IActiveSkillSpawnedObject)}.", instance);
            return;
        }

        spawnedObject.Initialize(context, BuildProjectileConfig(request, facingX));
    }

    private ActiveSkillProjectileConfig BuildProjectileConfig(ActiveSkillCastRequest request, float facingX)
    {
        Vector2 direction = new Vector2(facingX, 0f);
        return new ActiveSkillProjectileConfig
        {
            direction = direction,
            speed = initialSpeed,
            lifetime = lifetime,
            destroyOnHit = destroyOnHit,
            maxHitCount = maxHitCount,
            hitConfig = request.Skill.damageProfile.CreateHitConfig(request.Context, direction)
        };
    }
}
[System.Serializable]
public struct ActiveSkillProjectileConfig //projectile 初始化資料
{
    public Vector2 direction;
    public float speed;
    public float lifetime;
    public bool destroyOnHit;
    public int maxHitCount;
    public HitConfig hitConfig;
}

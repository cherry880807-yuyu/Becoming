using UnityEngine;


[CreateAssetMenu(menuName = "Mutation/Active Skill/Active Skill Data")]
public class ActiveSkillDataSO : ScriptableObject//技能本體資料
{
    public string skillId;
    [Min(0f)] public float minHoldDuration;
    [Min(0f)] public float cooldown;
    [Min(0f)] public float staminaCost;
    [Header("Animation")]
    public ActiveSkillAnimationProfile animationProfile = new();
    public SkillDamageProfile damageProfile = new();
    public ActiveSkillCastEffect castEffect;

    public string SkillKey => string.IsNullOrWhiteSpace(skillId) ? name : skillId;

    public bool CanTrigger(ActiveSkillContext context)
    {
        if (minHoldDuration > 0f && context.HoldDuration < minHoldDuration) return false;
        return true;
    }
}

[System.Serializable]
public sealed class ActiveSkillAnimationProfile
{
    public AnimationClip chargeClip;
    public AnimationClip releaseClip;
    public AnimationClip cancelClip;
}

public enum SkillDamageMode //技能傷害計算方式
{
    Fixed,
    BasedOnNormalAttack
}

[System.Serializable]
public sealed class SkillDamageProfile//技能傷害資料設定
{
    [SerializeField] private SkillDamageMode damageMode = SkillDamageMode.BasedOnNormalAttack;
    [SerializeField] private int fixedDamage = 10;
    [SerializeField] private float normalAttackMultiplier = 1f;
    [SerializeField] private int bonusDamage;
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private float hitStopTime = 0.05f;

    public HitConfig CreateHitConfig(ActiveSkillContext context, Vector2 hitDirection)
    {
        return new HitConfig(
            CalculateDamage(context),
            hitDirection.sqrMagnitude > 0f ? hitDirection.normalized : Vector2.right,
            knockbackForce,
            context.Player != null ? (Vector2)context.Player.transform.position : (Vector2)context.Origin,
            hitStopTime);
    }

    private int CalculateDamage(ActiveSkillContext context)
    {
        int baseDamage = damageMode == SkillDamageMode.BasedOnNormalAttack
            ? context.ActorData?.PlayerNormalAttackInputSystem?.FinalDamage ?? 0
            : fixedDamage;

        return Mathf.Max(0, Mathf.RoundToInt(baseDamage * normalAttackMultiplier) + bonusDamage);
    }

}

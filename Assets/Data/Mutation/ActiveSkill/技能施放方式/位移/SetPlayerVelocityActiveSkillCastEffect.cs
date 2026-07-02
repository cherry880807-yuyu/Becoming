using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Active Skill Cast/Set Player Velocity")]
public sealed class SetPlayerVelocityActiveSkillCastEffect : ActiveSkillCastEffect
{
    [SerializeField] private Vector2 velocity = new Vector2(12f, 0f);
    [SerializeField] private bool useFacingForX = true;
    [SerializeField] private bool preserveCurrentX;
    [SerializeField] private bool preserveCurrentY;
    [Header("Damage")]
    [SerializeField] private Vector2 hitBoxOffset = new Vector2(1.6f, 0f);
    [SerializeField] private Vector2 hitBoxSize = new Vector2(3.2f, 1.4f);

    public override bool CanExecute(ActiveSkillCastRequest request)
    {
        ActiveSkillContext context = request.Context;
        return base.CanExecute(request) && context.ActorData.Rigidbody != null;
    }

    public override void Execute(ActiveSkillCastRequest request)
    {
        ActiveSkillContext context = request.Context;
        Rigidbody2D rigidbody = context.ActorData.Rigidbody;
        float x = preserveCurrentX ? rigidbody.velocity.x : velocity.x;
        float y = preserveCurrentY ? rigidbody.velocity.y : velocity.y;

        if (useFacingForX)
            x *= context.Facing.x < 0f ? -1f : 1f;

        rigidbody.velocity = new Vector2(x, y);

        float facingX = context.Facing.x < 0f ? -1f : 1f;
        Vector2 hitCenter = (Vector2)context.Origin + new Vector2(hitBoxOffset.x * facingX, hitBoxOffset.y);
        Vector2 hitDirection = new Vector2(facingX, 0f);
        ActiveSkillDamageUtility.DamageBox(
            context,
            hitCenter,
            hitBoxSize,
            request.Skill.damageProfile.CreateHitConfig(context, hitDirection));
    }
}

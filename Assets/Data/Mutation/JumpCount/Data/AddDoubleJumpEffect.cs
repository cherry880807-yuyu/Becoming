using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Effect/AddDoubleJump Effect")]
public class AddDoubleJumpEffect : MutationEffect
{
    [SerializeField] private int bonusJump;


    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerBrain>().PlayerActorData.JumpSystem.AddBonusAirJumps(bonusJump);
    }

    public override void Remove(GameObject target)
    {
        target.GetComponent<PlayerBrain>().PlayerActorData.JumpSystem.RemoveBonusAirJumps(bonusJump);
    }
}
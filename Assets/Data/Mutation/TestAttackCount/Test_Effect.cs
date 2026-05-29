using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Effect/Test Effect")]
public class ChainLightningEffect: MutationEffect
{
    [SerializeField]private int damage;


    public override void Apply( GameObject target)
    {
        target.GetComponent<PlayerBrain>().PlayerActorData.AttackSystem.AddBonusDamage(damage);
    }

    public override void Remove(GameObject target)
    {
        target.GetComponent<PlayerBrain>().PlayerActorData.AttackSystem.RemoveBonusDamage(damage);
    }
}
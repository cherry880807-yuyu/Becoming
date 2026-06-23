using UnityEngine;

[CreateAssetMenu(menuName = "Mutation/Effect/AddBonusDamage Effect")]
public class AddBonusDamageEffect: MutationEffect
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
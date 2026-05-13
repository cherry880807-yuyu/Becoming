using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private ComboData comboData;
    public ComboData ComboData => comboData;
    private Collider2D hitbox;

    private ComboStep currentStep;


    void Awake()
    {
        hitbox = GetComponent<Collider2D>();
    }

    public void SetStep(ComboStep step)
    {
        currentStep = step;
    }

    public void DoHitCheck(int damage)
    {

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            hitbox.bounds.center,
            hitbox.bounds.size,
            0f,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            IDamageable dmg = hit.GetComponent<IDamageable>();

            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }

    }


}
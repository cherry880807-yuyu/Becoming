using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private ComboDataSO comboData;
    public ComboDataSO ComboDataSO => comboData;
    private Collider2D hitbox;

    [SerializeField]
    private ComboStep currentStep;


    void Awake()
    {
        hitbox = GetComponent<Collider2D>();
    }

    public void SetStep(int comboStepIndex)
    {
        currentStep = comboData.steps[comboStepIndex];
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
                Vector2 hitDir = (hit.transform.position - transform.position).normalized;

                DamageInfo info = new DamageInfo(damage, hitDir, currentStep.knockbackForce, currentStep.hitStopTime);
                dmg.TakeDamage(info);

                EnemyBrain enemyBrain = hit.GetComponent<EnemyBrain>();

                if (enemyBrain != null & enemyBrain.ActorData.IsAlive)
                {
                    EventBus.Publish(new AttackEnemyEvent { hitTime = currentStep.hitStopTime });
                }
                else
                {
                    Debug.Log("鞭打屍體!");
                }
            }
        }

    }


}
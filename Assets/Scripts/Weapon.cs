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
                Vector2 hitDir = (hit.transform.position - transform.position).normalized;

                DamageInfo info = new DamageInfo(damage, hitDir, currentStep.knockbackForce, currentStep.hitStopTime);
                dmg.TakeDamage(info);

                if (hit.GetComponent<EnemyBrain>().ActorData.IsAlive)
                {
                    HitStop(currentStep.hitStopTime);
                    EventBus.Publish<AttackThreeTimesEvent>(new AttackThreeTimesEvent());
                }
                else
                {
                    Debug.Log("鞭打屍體!");
                }




            }
        }

    }

    void HitStop(float duration, float timeScale = 0f)
    {
        StartCoroutine(Stop(duration, timeScale));
    }

    IEnumerator Stop(float duration, float timeScale)
    {
        Time.timeScale = timeScale;
        Debug.Log("僵直");
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        Debug.Log("僵直結束");
    }


}
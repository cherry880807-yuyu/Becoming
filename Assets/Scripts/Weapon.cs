using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    private bool active = true;
    private ComboStep currentStep;

    public void SetStep(ComboStep step)
    {
        currentStep = step;
    }
    public void EnableHitbox()
    {
        active = true;
    }

    public void DisableHitbox()
    {
        active = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;

        IDamageable dmg = other.GetComponent<IDamageable>();


        if (dmg != null)
        {
            dmg.TakeDamage(10);
            Debug.Log(dmg+"受到"+10+"點傷害");
        }
    }
}

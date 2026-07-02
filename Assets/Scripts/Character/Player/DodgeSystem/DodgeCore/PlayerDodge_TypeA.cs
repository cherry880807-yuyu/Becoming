using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge_TypeA : IDodge
{
    private DodgeDataSO data;

    public PlayerDodge_TypeA(DodgeDataSO data)
    {
        this.data = data;
    }

    public IEnumerator Dodge(Rigidbody2D rb, Vector2 dir)
    {
        float elapsed = 0f;
        float distance = data.dodgeDistance;
        float duration = data.dodgeDuration;

        Vector2 start = rb.position;
        Vector2 target = start + dir.normalized * distance;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0, 1, t); // 👈 手感關鍵

            rb.MovePosition(Vector2.Lerp(start, target, t));

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}

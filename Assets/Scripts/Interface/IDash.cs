using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDash
{
    IEnumerator Dash(Rigidbody2D rb, Vector2 dir);
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDodge
{
    IEnumerator Dodge(Rigidbody2D rb, Vector2 dir);
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementCore
{
    void Move(Rigidbody2D rb, Vector2 input);
}
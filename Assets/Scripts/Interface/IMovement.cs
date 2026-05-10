using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    void Move(Rigidbody2D rb, Vector2 input);
}
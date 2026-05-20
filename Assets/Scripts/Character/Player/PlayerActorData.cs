using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActorData
{
    public Vector2 Facing;

    //Component
    public Rigidbody2D Rigidbody;
    public Animator Animator;
    public Collider2D Collider;
    public SpriteRenderer SpriteRenderer;

    //System
    public MovementSystem MovementSystem;
    public DashSystem DashSystem;
    public AttackSystem AttackSystem;
    public AnimationSystem AnimationSystem;

    // optional
    public StaminaSystem StaminaSystem;

    
    public PlayerActorData(Rigidbody2D rigidbody, Animator animator, Collider2D collider, SpriteRenderer spriteRenderer)
    {
        Rigidbody = rigidbody;
        Animator = animator;
        Collider = collider;
        SpriteRenderer = spriteRenderer;
    }
}


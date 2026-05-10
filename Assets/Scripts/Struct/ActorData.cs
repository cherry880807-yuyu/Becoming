using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorData
{
    public Vector2 Facing;
    
    public Rigidbody2D Rigidbody;
    public Animator Animator;
    public Collider2D Collider;
    public SpriteRenderer SpriteRenderer;

    public MovementSystem MovementSystem;
    public DashSystem DashSystem;
    public AttackSystem AttackSystem;
    public AnimationSystem AnimationSystem;

    // optional
    public StaminaSystem StaminaSystem;
}
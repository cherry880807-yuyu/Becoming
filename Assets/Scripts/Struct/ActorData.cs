using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorData
{
    public Rigidbody2D Rigidbody;
    public Animator Animator;
    public Collider2D Collider;

    public MovementSystem MovementSystem;
    public DashSystem DashSystem;
    public AttackSystem AttackSystem;
    public AnimationSystem AnimationSystem;

     // optional
    public StaminaSystem StaminaSystem;
}
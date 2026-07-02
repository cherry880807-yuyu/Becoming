using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActorData
{
    public Vector2 Facing = new Vector2(1f, 0f);

    //Component
    public Rigidbody2D Rigidbody;
    public Animator Animator;
    public Collider2D Collider;

    //System
    public MovementSystem MovementSystem;
    public JumpSystem JumpSystem;
    public DodgeSystem DodgeSystem;
    public PlayerNormalAttackInputSystem PlayerNormalAttackInputSystem;
    public PlayerWeaponInventorySystem WeaponInventorySystem;
    public AnimationSystem AnimationSystem;
    public PlayerCombatSystem CombatSystem;
    public SkillSystem SkillSystem;
    public PlayerSkillInputSystem SkillInputSystem;


    // optional
    public StaminaSystem StaminaSystem;


    public PlayerActorData(Rigidbody2D rigidbody, Animator animator, Collider2D collider)
    {
        Rigidbody = rigidbody;
        Animator = animator;
        Collider = collider;
    }
}


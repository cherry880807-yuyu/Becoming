using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_TypeA : IMovementCore, ISprintable
{
    private float moveSpeed;
    private float sprintSpeed;
    private float sprintSpeedMultiplier = 1f;
    private bool isSprinting;

    public PlayerMovement_TypeA(float moveSpeed, float sprintSpeed)
    {
        this.moveSpeed = moveSpeed;
        this.sprintSpeed = sprintSpeed;
    }

    public void Move(Rigidbody2D rb, Vector2 input)
    {
        float speed = isSprinting ? sprintSpeed * Mathf.Max(0f, sprintSpeedMultiplier) : moveSpeed;
        rb.velocity = new Vector2(input.x * speed, rb.velocity.y);
    }


    public void SetSprint(bool sprint)
    {
        isSprinting = sprint;
    }

    public void AddSprintSpeedMultiplier(float multiplier)
    {
        sprintSpeedMultiplier += multiplier;
    }

    public void RemoveSprintSpeedMultiplier(float multiplier)
    {
        sprintSpeedMultiplier -= multiplier;
    }

}

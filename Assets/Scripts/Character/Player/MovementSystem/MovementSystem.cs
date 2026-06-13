using System.Collections.Generic;
using UnityEngine;

public class MovementSystem
{
    private PlayerActorData actorData;
    private IMovementCore core;
    private List<IMovementModifier> modifiers = new(); //例如：減速（slow）
    private List<IMovementEffect> effects = new();
    public Vector2 Facing { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsSprint { get; private set; }


    private float _lastGroundedTime;
    public MovementSystem(PlayerActorData actorData, IMovementCore core)
    {
        this.actorData = actorData;
        this.core = core;
    }

    public void AddModifier(IMovementModifier mod)
    {
        modifiers.Add(mod);
    }
    public void ClearModifiers()
    {
        modifiers.Clear();
    }

    public void Move(Rigidbody2D rb, Vector2 input)
    {
        foreach (var mod in modifiers)
        {
            input = mod.Modify(input);
        }

        core.Move(rb, input);

        foreach (var effect in effects)
        {
            effect.Apply();
        }
    }
    public void SetSprint(bool value)
    {
        IsSprint=value;
        if (core is ISprintable sprintableCore)
        {
            sprintableCore.SetSprint(value);
        }
    }

    public void CheckGrounded_ByBoxCast()
    {
        Bounds b = actorData.Collider.bounds;
        float checkDistance = 0.2f;
        Vector2 center = new Vector2(b.center.x, b.min.y);
        Vector2 boxSize = new Vector2(b.size.x * 0.9f, 0.1f);
        bool hitGround = Physics2D.BoxCast(
           center,
           boxSize,
           0f,
           Vector2.down,
           checkDistance,
           LayerMask.GetMask("Ground", "Platform")
       ).collider != null;

        if (hitGround&& actorData.Rigidbody.velocity.y <= 0.05f)
        {
            _lastGroundedTime = Time.time;
        }
        IsGrounded = Time.time - _lastGroundedTime < 0.01f;
    }
    public void CheckGrounded_ByRayCast()
    {
        Bounds b = actorData.Collider.bounds;
        float offest = 0.02f;
        float _rayLength = 0.25f;
        float width = b.size.x * 0.4f;
        LayerMask mask = LayerMask.GetMask("Ground", "Platform"); ;

        Vector2 bottom = new Vector2(b.center.x, b.min.y + offest);
        Vector2 leftOrigin = bottom + Vector2.left * width;
        Vector2 rightOrigin = bottom + Vector2.right * width;
        Vector2 centerOrigin = bottom;

        bool hitGround =
            Physics2D.Raycast(leftOrigin, Vector2.down, _rayLength, mask) ||
            Physics2D.Raycast(centerOrigin, Vector2.down, _rayLength, mask) ||
            Physics2D.Raycast(rightOrigin, Vector2.down, _rayLength, mask);

        if (hitGround)
        {
            _lastGroundedTime = Time.time;
        }

        IsGrounded = Time.time - _lastGroundedTime < 0.05f;

        Debug.DrawRay(leftOrigin, Vector2.down * _rayLength, Color.red);
        Debug.DrawRay(centerOrigin, Vector2.down * _rayLength, Color.green);
        Debug.DrawRay(rightOrigin, Vector2.down * _rayLength, Color.blue);
    }

    public Collider2D GetCurrentPlatform(Collider2D col)
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            col.bounds.center,
            col.bounds.size,
            0f,
            Vector2.down,
            0.1f,
            LayerMask.GetMask("Platform")
        );
        return hit.collider;
    }

}
using System.Collections.Generic;
using UnityEngine;

public class MovementSystem
{
    private IMovementCore core;
    private List<IMovementModifier> modifiers = new();
    public Vector2 Facing { get; private set; }
    public MovementSystem(IMovementCore core)
    {
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
    }
    public void SetSprint(bool value)
    {
        if (core is ISprintable sprintableCore)
        {
            sprintableCore.SetSprint(value);
        }
    }
}
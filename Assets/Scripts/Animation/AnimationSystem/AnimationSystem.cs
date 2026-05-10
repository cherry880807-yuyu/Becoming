using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem
{
    private IAnimation animation;

    public AnimationSystem(IAnimation animation)
    {
        this.animation = animation;
    }

    public void SetState(CharacterState state)
    {
        animation.SetState(state);
    }
}
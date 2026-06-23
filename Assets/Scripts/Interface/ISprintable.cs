using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISprintable
{
    void SetSprint(bool value);
    void AddSprintSpeedMultiplier(float multiplier);
    void RemoveSprintSpeedMultiplier(float multiplier);
}

using UnityEngine;

public interface IPlayerSkillInputSource
{
    Vector2 MoveInput { get; }
    bool IsSprinting { get; }
    bool WasCommandPressed(PlayerCommand command, float withinSeconds);
    bool WasCommandReleased(PlayerCommand command, float withinSeconds);
    float GetCommandHoldDuration(PlayerCommand command);
    bool MatchPressedCommandSequence(float withinSeconds, params PlayerCommand[] sequence);
}

using System.Collections.Generic;
using UnityEngine;

public enum PlayerCommand
{
    Attack,
    Jump,
    Dash,
    Down,
    Sprint,
    DirectionUp,
    DirectionDown,
    DirectionLeft,
    DirectionRight,
    Ultimate
}

public enum PlayerCommandPhase
{
    Pressed,
    Released
}

public readonly struct PlayerCommandRecord
{
    public readonly PlayerCommand Command;
    public readonly PlayerCommandPhase Phase;
    public readonly float Time;
    public readonly float HoldDuration;

    public PlayerCommandRecord(
        PlayerCommand command,
        PlayerCommandPhase phase,
        float time,
        float holdDuration = 0f)
    {
        Command = command;
        Phase = phase;
        Time = time;
        HoldDuration = holdDuration;
    }
}

public sealed class PlayerCommandBuffer
{
    private const int MaxRecords = 32;

    private readonly List<PlayerCommandRecord> records = new();
    private readonly Dictionary<PlayerCommand, float> holdStartedAt = new();

    public void PushPressed(PlayerCommand command)
    {
        float time = Time.time;
        holdStartedAt[command] = time;
        Push(new PlayerCommandRecord(command, PlayerCommandPhase.Pressed, time));
    }

    public void PushReleased(PlayerCommand command)
    {
        float time = Time.time;
        float holdDuration = GetHoldDuration(command, time);
        holdStartedAt.Remove(command);
        Push(new PlayerCommandRecord(command, PlayerCommandPhase.Released, time, holdDuration));
    }

    public bool WasPressedRecently(PlayerCommand command, float withinSeconds)
    {
        return ContainsRecent(command, PlayerCommandPhase.Pressed, withinSeconds);
    }

    public bool WasReleasedRecently(PlayerCommand command, float withinSeconds)
    {
        return ContainsRecent(command, PlayerCommandPhase.Released, withinSeconds);
    }

    public float GetHoldDuration(PlayerCommand command)
    {
        return GetHoldDuration(command, Time.time);
    }

    public bool MatchPressedSequence(float withinSeconds, params PlayerCommand[] sequence)
    {
        if (sequence == null || sequence.Length == 0) return false;

        float now = Time.time;
        int sequenceIndex = sequence.Length - 1;

        for (int i = records.Count - 1; i >= 0 && sequenceIndex >= 0; i--)
        {
            PlayerCommandRecord record = records[i];
            if (now - record.Time > withinSeconds) break;
            if (record.Phase != PlayerCommandPhase.Pressed) continue;
            if (record.Command != sequence[sequenceIndex]) continue;

            sequenceIndex--;
        }

        return sequenceIndex < 0;
    }

    public void Clear()
    {
        records.Clear();
        holdStartedAt.Clear();
    }

    private void Push(PlayerCommandRecord record)
    {
        records.Add(record);
        if (records.Count > MaxRecords)
            records.RemoveAt(0);
    }

    private bool ContainsRecent(PlayerCommand command, PlayerCommandPhase phase, float withinSeconds)
    {
        float now = Time.time;
        for (int i = records.Count - 1; i >= 0; i--)
        {
            PlayerCommandRecord record = records[i];
            if (now - record.Time > withinSeconds) return false;
            if (record.Command == command && record.Phase == phase) return true;
        }

        return false;
    }

    private float GetHoldDuration(PlayerCommand command, float now)
    {
        return holdStartedAt.TryGetValue(command, out float startedAt)
            ? Mathf.Max(0f, now - startedAt)
            : 0f;
    }
}

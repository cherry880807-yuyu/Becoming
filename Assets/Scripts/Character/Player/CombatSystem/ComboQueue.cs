using UnityEngine;

public sealed class ComboQueue
{
    private readonly float queueDuration;
    private bool hasQueuedStep;
    private float expiresAt;

    public ComboQueue(float queueDuration = 0.4f)
    {
        this.queueDuration = queueDuration;
    }

    public void QueueNextStep()
    {
        hasQueuedStep = true;
        expiresAt = Time.time + queueDuration;
    }

    public bool TryConsume()
    {
        if (!hasQueuedStep) return false;

        if (Time.time > expiresAt)
        {
            Clear();
            return false;
        }

        Clear();
        return true;
    }

    public void Clear()
    {
        hasQueuedStep = false;
        expiresAt = 0f;
    }
}

using System;
using System.Collections.Generic;

public static class EventBus
{
    //<成就類型,成就達成後執行的委派>
    private static Dictionary<Type, Delegate> eventTable = new();

    public static void Subscribe<T>(Action<T> listener)
    {
        Type type = typeof(T);

        if (eventTable.TryGetValue(type, out Delegate existing))
        {
            eventTable[type] = Delegate.Combine(existing, listener);
        }
        else
        {
            eventTable[type] = listener;
        }
    }

    public static void Unsubscribe<T>(Action<T> listener)
    {
        Type type = typeof(T);

        if (eventTable.TryGetValue(type, out Delegate existing))
        {
            Delegate current = Delegate.Remove(existing, listener);

            if (current == null)
                eventTable.Remove(type);
            else
                eventTable[type] = current;
        }
    }

    public static void Publish<T>(T eventData)
    {
        Type type = typeof(T);

        if (eventTable.TryGetValue(type, out Delegate del))
        {
            ((Action<T>)del)?.Invoke(eventData);
        }
    }
}
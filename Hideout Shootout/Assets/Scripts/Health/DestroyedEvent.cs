using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestroyedEventArgs> OnDestroy;

    public void CallDestroyedEvent(bool playerDied, int points)
    {
        OnDestroy?.Invoke(this, new DestroyedEventArgs() { playerDied = playerDied, points = points});
    }
}

public class DestroyedEventArgs: EventArgs
{
    public bool playerDied;
    public int points;
}

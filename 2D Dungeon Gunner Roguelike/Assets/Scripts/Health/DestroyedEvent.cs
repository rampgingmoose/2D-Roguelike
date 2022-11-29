using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestroyedEventArgs> OnDestroy;

    public void CallDestroyedEvent(bool playerDied)
    {
        OnDestroy?.Invoke(this, new DestroyedEventArgs() { playerDied = playerDied});
    }
}

public class DestroyedEventArgs: EventArgs
{
    public bool playerDied;
}

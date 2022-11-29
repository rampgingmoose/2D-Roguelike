using System;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthEvent : MonoBehaviour
{
    public event Action<HealthEvent, HealthEventArgs> OnHealthChanged;

    public void CallHealthChangedEvent(float healthPercent, int healthAmount, int damageAmount)
    {
        OnHealthChanged?.Invoke(this, new HealthEventArgs() { healhPercent = healthPercent, healthAmount = healthAmount,
            damageAmount = damageAmount});
    }
}

public class HealthEventArgs : EventArgs
{
    public float healhPercent;
    public int healthAmount;
    public int damageAmount;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startinghealth;
    private int currentHealth;

    public void SetStartingHealth(int startingHealth)
    {
        this.startinghealth = startingHealth;
        currentHealth = startingHealth;
    }

    public int GetStartingHealth()
    {
        return startinghealth;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FireWeaponEvent : MonoBehaviour
{
    public event Action<FireWeaponEvent, FireWeaponeventArgs> OnFireWeapon;

    public void CallFireWeaponEvent(bool fire, bool firePreviousFrame,AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirecitonVector)
    {
        OnFireWeapon?.Invoke(this, new FireWeaponeventArgs() { fire = fire, firePreviousFrame = firePreviousFrame, 
            aimDirection = aimDirection, aimAngle = aimAngle, weaponAimAngle = weaponAimAngle, weaponAimDirectionVector = weaponAimDirecitonVector});        
    }
}

public class FireWeaponeventArgs : EventArgs
{
    public bool fire;
    public bool firePreviousFrame;
    public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
}

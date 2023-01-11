using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnWeaponAim?.Invoke(this, new AimWeaponEventArgs() { aimDirection = aimDirection, aimAngle = aimAngle, 
            weaponAimAngle = weaponAimAngle, weaponAimDirectionVector = weaponAimDirectionVector});
    }
}

public class AimWeaponEventArgs : EventArgs
{
    public AimDirection aimDirection;
    //Angle between our mouse cursor and player pivot point
    public float aimAngle;
    //Angle between our mouse cursor and the weapon pivot point on the player prefab
    public float weaponAimAngle;
    //direction vector used to calculate weapon aim angle
    public Vector3 weaponAimDirectionVector;
} 

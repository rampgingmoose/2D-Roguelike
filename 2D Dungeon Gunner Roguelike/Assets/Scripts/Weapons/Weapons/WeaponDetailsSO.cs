using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    #region Header WEAPON BASE DETAILS
    [Header("WEAPON BASE DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("Weapon Name")]
    #endregion
    public string weaponName;
    #region Tooltip
    [Tooltip("The sprite for the weapon - the sprite should have the 'generate physics shape' option selected")]
    #endregion
    public Sprite weaponSprite;

    #region Header WEAPON CONFIGURATION
    [Space(10)]
    [Header("WEAPON CONFIGURATION")]
    #endregion Header WEAPON CONFIGURATION
    #region Tooltip
    [Tooltip("Weapon Shoot Position - the offset position for the end of the weapon from the sprite pivot point")]
    #endregion Tooltip
    public Vector3 weaponShootPosition;
    #region Tooltip
    [Tooltip("Weapon current ammoArray")]
    #endregion
    public AmmoDetailsSO weaponCurrentAmmo;
    #region Tooltip
    [Tooltip("Weapon shoot effect SO - contains particle effect parameters to be used in conjunction with the weaponShootEffectPrefab")]
    #endregion
    public WeaponShootEffectSO weaponShootEffectSO;
    #region
    [Tooltip("The firing Sound effect SO for the weapon")]
    #endregion
    public SoundEffectSO weaponFiringSoundEffect;
    #region
    [Tooltip("The reloading sound effect SO for the weapon")]
    #endregion
    public SoundEffectSO weaponReloadingSoundEffect;

    #region Header WEAPON OPERATING VALUES
    [Space(10)]
    [Header("WEAPON OPERATING VALUES")]
    #endregion 
    #region
    [Tooltip("Select if the weapon has infinite ammoArray")]
    #endregion
    public bool hasInfiniteAmmo = false;
    #region Tooltip
    [Tooltip("Select if the weapon has infinite clip capacity")]
    #endregion
    public bool hasInfiniteClipCapacity;
    #region Tooltip
    [Tooltip("The weapon capacity - shots before a relaod")]
    #endregion
    public int weaponClipCapacity = 6;
    #region Tooltip
    [Tooltip("Weapon ammoArray capacity - the maximum number of rounds that can be held for this weapon")]
    #endregion
    public int weaponAmmoCapacity = 100;
    #region Tooltip
    [Tooltip("Weapon Fire Rate - 0.2 means 5 shots a second")]
    #endregion
    public float weaponFireRate = 0.2f;
    #region
    [Tooltip("Weapon Precharge Time - time in seconds to hold fire button down before firing")]
    #endregion
    public float weaponPrechargeTime = 0f;
    #region
    [Tooltip("This is the weapon reload time in seconds")]
    #endregion
    public float weaponReloadTime = 0f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }

        if (!hasInfiniteClipCapacity)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipCapacity), weaponClipCapacity, false);
        }
    }
#endif
    #endregion
}

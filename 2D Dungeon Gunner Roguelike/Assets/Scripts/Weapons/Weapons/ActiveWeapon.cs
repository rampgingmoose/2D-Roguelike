using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SetActiveWeaponEvent))]
public class ActiveWeapon : MonoBehaviour
{
    #region
    [Tooltip("Populate with the SpriteRenderer on the chld Weapon gameObject")]
    #endregion
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    #region
    [Tooltip("Populate wtih the PolygonCollider2D on the child Weapon gameObject")]
    #endregion
    [SerializeField] private PolygonCollider2D weaponPolgonCollider2D;
    #region
    [Tooltip("Poplate with the Transform on the WeaponShootPosition")]
    #endregion
    [SerializeField] private Transform weaponShootPositionTransform;
    #region Tooltip
    [Tooltip("Populate with the Transform on the WeaponEffectPositionTransform")]
    #endregion
    [SerializeField] private Transform weaponEffectPositionTransform;

    private SetActiveWeaponEvent setWeaponEvent;
    private Weapon currentWeapon;

    private void Awake()
    {
        setWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        setWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        setWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        //Set current weapon sprite
        currentWeapon = weapon;
        weaponSpriteRenderer.sprite = currentWeapon.weaponDetailsSO.weaponSprite;

        //If the weapon has a polygon collider and a sprite then set it to the weapon sprite physics shape
        if(weaponPolgonCollider2D != null && weaponSpriteRenderer.sprite != null)
        {
            //Get sprite physics shape - this returns the sprite physics shape points as a list of Vector2s
            List<Vector2> spritePhysicsShapePointsList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

            //Set polygon collider on weapon to pick up physics shape for sprite - set collider points to sprite physics shape points
            weaponPolgonCollider2D.points = spritePhysicsShapePointsList.ToArray();
        }

        //Set weapon shoot position
        weaponShootPositionTransform.localPosition = currentWeapon.weaponDetailsSO.weaponShootPosition;
    }

    public AmmoDetailsSO GetCurrentAmmo()
    {
        return currentWeapon.weaponDetailsSO.weaponCurrentAmmo;
    }

    public Weapon GetCurrentWeapon()
    {       
        return currentWeapon;
    }

    public Vector3 GetShootPosition()
    {
        return weaponShootPositionTransform.position;
    }

    public Vector3 GetShootEffectPosition()
    {
        return weaponEffectPositionTransform.position;
    }

    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolgonCollider2D), weaponPolgonCollider2D);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform), weaponShootPositionTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform), weaponEffectPositionTransform);
    }
#endif
    #endregion
}

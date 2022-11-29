using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Select the layers that the enemy bullets will hit")]
    #endregion
    [SerializeField] private LayerMask layerMask;

    #region Tooltip
    [Tooltip("Populate this with the WeaponShootPosition child gameobject transform")]
    #endregion
    [SerializeField] private Transform weaponShootPosition;
    
    private Enemy enemy;
    private EnemyDetailsSO enemyDetailsSO;
    private float firingIntervalTimer;
    private float firingDurationTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemyDetailsSO = enemy.enemyDetails;

        firingIntervalTimer = WeaponShootInterval();
        firingDurationTimer = WeaponShootDuration();
    }

    private void Update()
    {
        //Update Timers
        firingIntervalTimer -= Time.deltaTime;

        //Interval Timer
        if (firingIntervalTimer < 0f)
        {
            if (firingDurationTimer >= 0f)
            {
                firingDurationTimer -= Time.deltaTime;

                FireEnemyWeapon();
            }
            else
            {
                //Reset timers
                firingIntervalTimer = WeaponShootInterval();
                firingDurationTimer = WeaponShootDuration();
            }
        }
    }

    /// <summary>
    /// Calculate a random weapon shoot duration between the min and max
    /// </summary>
    private float WeaponShootDuration()
    {
        return Random.Range(enemyDetailsSO.firingDurationMin, enemyDetailsSO.firingDurationMax);
    }

    private float WeaponShootInterval()
    {
        return Random.Range(enemyDetailsSO.firingIntervalMin, enemyDetailsSO.firingIntervalMax);
    }

    /// <summary>
    /// Fire the weapon
    /// </summary>
    private void FireEnemyWeapon()
    {
        //Player Distance
        Vector3 playerDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;

        //Calculate direction vector of player from weapon shoot position
        Vector3 weaponDirection = GameManager.Instance.GetPlayer().GetPlayerPosition() - weaponShootPosition.position;

        //Calculate weapon to player angle
        float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        //Get enemy angle to player
        float enemyAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirectionVector);

        //Set enemy aim direction
        AimDirection enemyAimDirection = HelperUtilities.GetAimDirection(enemyAngleDegrees);

        //Trigger weapon aim event
        enemy.aimWeaponEvent.CallAimWeaponEvent(enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);

        //Only fire if enemy has a weapon
        if (enemyDetailsSO.enemyWeapon != null)
        {
            //Get ammo range
            float enemyAmmoRange = enemyDetailsSO.enemyWeapon.weaponCurrentAmmo.ammoRange;

            //Is player in range
            if (playerDirectionVector.magnitude <= enemyAmmoRange)
            {
                //Does this enemy require line of sight to the player before firing?
                if (enemyDetailsSO.firingLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection, enemyAmmoRange)) 
                    return;

                enemy.fireWeaponEvent.CallFireWeaponEvent(true, true, enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);
            }
        }
    }

    private bool IsPlayerInLineOfSight(Vector3 weaponDirection, float enemyAmmoRange)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(weaponShootPosition.position, (Vector2)weaponDirection, enemyAmmoRange , layerMask);

        if (raycastHit2D && raycastHit2D.transform.CompareTag(Settings.playerTag))
        {
            return true;
        }

        return false;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPosition), weaponShootPosition);
    }
#endif
    #endregion
}

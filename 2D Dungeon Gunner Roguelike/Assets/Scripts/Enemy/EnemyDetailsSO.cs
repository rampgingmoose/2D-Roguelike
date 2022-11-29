using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject
{
    #region BASE ENEMY DETAILS
    [Space(10)]
    [Header("BASE ENEMY DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("The name of the enemy")]
    #endregion
    public string enemyName;

    #region Tooltip
    [Tooltip("The prefab for the enemy")]
    #endregion
    public GameObject enemyPrefab;

    #region Tooltip
    [Tooltip("Distance to the player before enemy starts chasing")]
    #endregion
    public float chaseDistance = 50f;

    #region Tooltip
    [Tooltip("This is the standard lit shader material for the enemy (used after the enemy materializes")]
    #endregion
    public Material enemyStandardMaterial;

    #region Header MATERIALIZE SETTINGS
    [Space(10)]
    [Header("MATERIALIZE SETTINGS")]
    #endregion

    #region Tooltip
    [Tooltip("The time in seconds that it takes for the enemy to materialize")]
    #endregion
    public float enemyMaterializeTime;

    #region Tooltip
    [Tooltip("The shader to be used when the enemy materializes")]
    #endregion
    public Shader enemyMaterializeShader;

    #region Tooltip
    [Tooltip("The color to use when the enemy Materializes. This is an HDR color so the intensity can be set to cause glowing/bloom")]
    #endregion
    public Color enemyMaterializeColor;

    #region Header ENEMY WEAPON SETTINGS
    [Space(10)]
    [Header("ENEMY WEAPON SETTINGS")]
    #endregion

    #region Tooltip
    [Tooltip("The weapon for the enemy - none if the enemy doesn't have a weapon")]
    #endregion
    public WeaponDetailsSO enemyWeapon;

    #region Tooltip
    [Tooltip("The minimum time delay interval in seconds between bursts of enemy shooting. This value should be greater than 0. A random" +
        "value will be selected between the min value and max value.")]
    #endregion
    public float firingIntervalMin = 0.1f;

    #region Tooltip
    [Tooltip("The maximum time delay interval in seconds between bursts of enemy shooting. This value should be greater than 0 and the min value." +
        "A random value will be selected between the min and max value.")]
    #endregion
    public float firingIntervalMax = 1f;

    #region Tooltip
    [Tooltip("The minimum firing duration that the enemy shoots for during a firing burst. This value should be greater than 0. A random" +
        "value will be selected between the min and max value.")]
    #endregion
    public float firingDurationMin = 1f;

    #region Tooltip
    [Tooltip("The maximum firing duration that the enemy shoots for during a firing burst. This value should be greater than 0 and the min value." +
        "A random value will be selected between the min and max value.")]
    #endregion
    public float firingDurationMax = 2f;

    #region Tooltip
    [Tooltip("Select this is line of sight is required of the player before the enemy fires. If line of sight isn't selected the enemy will fire" +
        "regardless of obstacles whenever the player is 'in range'")]
    #endregion
    public bool firingLineOfSightRequired;

    #region Header ENEMY HEALTH
    [Space(10)]
    [Header("ENEMY HEALTH")]
    #endregion
    #region Tooltip
    [Tooltip("The health of the enemy for each level")]
    #endregion
    public EnemyHealthDetails[] enemyHealthDetailsArray;

    #region
    [Tooltip("Select if has immunity period immediately after hit. If so specify the immunity time in seconds in the other field")]
    #endregion
    public bool isImmuneAfterHit = false;

    #region
    [Tooltip("Immunity time in seconds after being hit")]
    #endregion
    public float hitImmunityTime;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof(firingIntervalMax), 
            firingIntervalMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin, nameof(firingDurationMax),
            firingDurationMax, false);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);
        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }
#endif
    #endregion
}

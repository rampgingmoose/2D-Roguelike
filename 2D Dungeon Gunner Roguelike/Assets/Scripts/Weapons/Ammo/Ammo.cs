using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    #region
    [Tooltip("Populate with child TrailRenderer component")]
    #endregion
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] ParticleSystem smokeTrailParticles;

    private float ammoRange = 0f; // the range of each ammo
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetailsSO;
    private float ammoChargeTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement = false;

    private void Awake()
    {
        //cache SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //Ammo charge effect
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!isAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetailsSO.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        //Calculate distance vector to move ammo
        Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

        transform.position += distanceVector;

        ammoRange -= distanceVector.magnitude;

        if(ammoRange <= 0f)
        {
            DisableAmmo();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ammoDetailsSO.isSmokeTrail)
        {
            DetachSmokeParticles();
        }
        EnableAmmoHitEffect();
        DisableAmmo();
    }
    /// <summary>
    /// Initialize the ammo being fired - using the ammoDetails, the aimangle, weaponAngle, and weaponOverrideDirectionVector.
    /// If this ammo is part of a pattern the ammo movement can be overriden by setting overrideAmmoMovement to true
    /// </summary>
    public void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo
        this.ammoDetailsSO = ammoDetails;

        //Set Fire Direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        //Set ammo Sprite
        spriteRenderer.sprite = ammoDetails.ammoSprite;

        //Set initial ammo material depending on whether there is an ammo charge period
        if (ammoDetails.ammoChargeTime > 0f)
        {
            //Set ammo charge timer
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = false;
        }
        else
        {
            ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        //Set ammo range
        ammoRange = ammoDetails.ammoRange;

        //Set ammo speed
        this.ammoSpeed = ammoSpeed;

        //Override ammo Movement
        this.overrideAmmoMovement = overrideAmmoMovement;

        //Activate ammo gameobject
        gameObject.SetActive(true);

        #endregion

        #region AmmoTrail

        if (ammoDetails.isAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }

        #endregion

        if (ammoDetails.isSmokeTrail)
        {
            if (smokeTrailParticles != null && smokeTrailParticles.transform.parent == null)
            {
                smokeTrailParticles.transform.SetParent(transform, true);
                smokeTrailParticles.transform.position = transform.position;
                smokeTrailParticles.gameObject.SetActive(true);
            }
        }
        else
        {
            smokeTrailParticles.gameObject.SetActive(false);
        }        
    }

    /// <summary>
    /// Set ammo fire direction and angle based on the input angle and direction adjusted by the random speed
    /// </summary>
    private void SetFireDirection(AmmoDetailsSO ammoDetailsSO,float aimAngle,float weaponAimAngle,Vector3 weaponAimDirectionVector)
    {
        //Calculate random spread andle between min and max
        float randomSpread = Random.Range(ammoDetailsSO.ammoSpreadMin, ammoDetailsSO.ammoSpreadMax);

        //Get a random spread toggle of -1 or 1
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        //Adjust ammo fire angle by random spread
        fireDirectionAngle += spreadToggle * randomSpread;

        //Set ammo Rotation
        transform.eulerAngles = new Vector3(0f, 0, fireDirectionAngle);

        //Set ammo fire direction
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    /// <summary>
    /// Disable the ammo - thus returning it to the object pool
    /// </summary>
    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    private void AttachSmokeParticles()
    {
        if(smokeTrailParticles != null && smokeTrailParticles.transform.parent == null)
        {
            
        }
    }

    private void DetachSmokeParticles()
    {
        if(smokeTrailParticles != null)
        {
            smokeTrailParticles.transform.parent = null;
            smokeTrailParticles.Stop();
        }
    }

    private void EnableAmmoHitEffect()
    {
        if(ammoDetailsSO.ammoHitEffectSO != null && ammoDetailsSO.ammoHitEffectSO.ammoHitEffectPrefab != null)
        {
            //Get Ammo Hit Effect gameobject from the pool (with particle system component)
            AmmoHitEffect ammoHitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent(ammoDetailsSO.ammoHitEffectSO.ammoHitEffectPrefab,
                transform.position, Quaternion.identity);

            //Set Hit Effect
            ammoHitEffect.SetHitEffect(ammoDetailsSO.ammoHitEffectSO);

            //Set gameobject active (the particle system is set to automatically disable the gameobject once finished)
            ammoHitEffect.gameObject.SetActive(true);
        }
    }

    public void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }
#endif
    #endregion
}

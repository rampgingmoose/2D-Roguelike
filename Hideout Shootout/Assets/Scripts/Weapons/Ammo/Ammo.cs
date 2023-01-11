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

    private float ammoRange = 0f; // the range of each ammoArray
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    public AmmoDetailsSO ammoDetailsSO;
    private float ammoChargeTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement = false;
    private bool isColliding = false;

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

        //Don't move ammoArray if movement has been overriden - e.g. this ammoArray is part of an ammoArray pattern that's meant to stick together
        if (!overrideAmmoMovement)
        {
            //Calculate distance vector to move ammoArray
            Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

            transform.position += distanceVector;

            ammoRange -= distanceVector.magnitude;

            if (ammoRange <= 0f)
            {
                if (ammoDetailsSO.isPlayerAmmo)
                {
                    StaticEventsHandler.CallMultiplierEvent(false);
                }

                //detach smoke trail child component so the particle effect can fade on its own once finished
                if (ammoDetailsSO.isSmokeTrail)
                {
                    DetachSmokeParticles();
                }

                DisableAmmo();
            }
        }       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerControl playerControl = collision.gameObject.GetComponent<PlayerControl>();

        //detach smoke trail child component so the particle effect can fade on its own once finished
        if (ammoDetailsSO.isSmokeTrail)
        {
            DetachSmokeParticles();
        }

        //If already colliding with something return
        if (isColliding) return;

        //Deal damage To Collision Object
        DealDamage(collision);

        if (playerControl != null && playerControl.isPlayerRolling)
        {
            return;
        }
        else
        {
            //Show ammoArray hit effect
            EnableAmmoHitEffect();

            DisableAmmo();
        }
    }

    private void DealDamage(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        bool enemyHit = false;

        if (health != null)
        {
            //Set isColliding to prevent ammoArray dealing damage multiple times
            isColliding = true;

            health.TakeDamage(ammoDetailsSO.ammoDamage);

            //Enemy Hit
            if (health.enemy != null)
            {
                enemyHit = true;
            }
        }

        //If player ammoArray then update multiplier
        if (ammoDetailsSO.isPlayerAmmo)
        {
            if (enemyHit)
            {
                //multiplier
                StaticEventsHandler.CallMultiplierEvent(true);
            }
            else
            {
                //no multiplier
                StaticEventsHandler.CallMultiplierEvent(false);
            }
        }
    }

    /// <summary>
    /// Initialize the ammoArray being fired - using the ammoDetails, the aimangle, weaponAngle, and weaponOverrideDirectionVector.
    /// If this ammoArray is part of a pattern the ammoArray movement can be overriden by setting overrideAmmoMovement to true
    /// </summary>
    public void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo
        this.ammoDetailsSO = ammoDetails;

        //Initialize isColliding
        isColliding = false;

        //Set Fire Direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        //Set ammoArray Sprite
        spriteRenderer.sprite = ammoDetails.ammoSprite;

        //Set initial ammoArray material depending on whether there is an ammoArray charge period
        if (ammoDetails.ammoChargeTime > 0f)
        {
            //Set ammoArray charge timer
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

        //Set ammoArray range
        ammoRange = ammoDetails.ammoRange;

        //Set ammoArray speed
        this.ammoSpeed = ammoSpeed;

        //Override ammoArray Movement
        this.overrideAmmoMovement = overrideAmmoMovement;

        //Activate ammoArray gameobject
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
    /// Set ammoArray fire direction and angle based on the input angle and direction adjusted by the random speed
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

        //Adjust ammoArray fire angle by random spread
        fireDirectionAngle += spreadToggle * randomSpread;

        //Set ammoArray Rotation
        transform.eulerAngles = new Vector3(0f, 0, fireDirectionAngle);

        //Set ammoArray fire direction
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    /// <summary>
    /// Disable the ammoArray - thus returning it to the object pool
    /// </summary>
    private void DisableAmmo()
    {
        gameObject.SetActive(false);
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

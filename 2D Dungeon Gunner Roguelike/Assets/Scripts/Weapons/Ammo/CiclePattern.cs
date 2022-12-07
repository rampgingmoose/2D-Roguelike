using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CiclePattern : MonoBehaviour, IFireable
{
    [Header("Projectile Settings")]
    public int numberOfAmmo = 20;

    [Header("Private Variables")]
    private Vector3 startPoint;
    private const float radius = 1f;

    [Header("Ammo Details")]
    private float ammoRange;
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private AmmoDetailsSO ammoDetailsSO;
    [SerializeField] private Ammo[] ammoArray;
    private float ammoChargeTimer;

    private void Awake()
    {
        startPoint = transform.position;
    }

    private void Update()
    {
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }

        float angleStep = 360f / ammoArray.Length;
        float angle = 0f;

        //Direction calculations
        float projectileDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
        float projectileDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

        Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
        Vector3 projectileMoveDirection = (projectileVector - startPoint).normalized * ammoSpeed * Time.deltaTime;

        foreach(Ammo ammo in ammoArray)
        {
            ammo.GetComponent<Rigidbody>().velocity += projectileMoveDirection;
        }

        angle += angleStep;


        if (ammoDetailsSO.ammoChargeTime > 0f)
        {
            ammoChargeTimer = ammoDetailsSO.ammoChargeTime;
        }
        else
        {
            ammoChargeTimer = 0f;
        }
    }

    public void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        this.ammoDetailsSO = ammoDetails;

        this.ammoSpeed = ammoSpeed;

        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        //Set ammoArray range
        ammoRange = ammoDetailsSO.ammoRange;

        gameObject.SetActive(true);

        foreach(Ammo ammo in ammoArray)
        {
            ammo.InitializeAmmo(ammoDetails, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, true);
        }

        //Set ammoArray charge timer - this will hold the ammoArray briefly
        if (ammoDetails.ammoChargeTime > 0f)
        {
            ammoChargeTimer = ammoDetails.ammoChargeTime;
        }
        else
        {
            ammoChargeTimer = 0f;
        }
    }

    /// <summary>
    /// Set ammoArray fire direction based on the input angle and direction adjusted by the random speed
    /// </summary>
    private void SetFireDirection(AmmoDetailsSO ammoDetailsSO, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
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

        //Set ammoArray fire direction
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }
}

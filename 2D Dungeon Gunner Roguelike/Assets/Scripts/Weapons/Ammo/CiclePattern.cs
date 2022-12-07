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

        SetFireDirection(ammoArray, ammoDetailsSO);


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
    private void SetFireDirection(Ammo[] ammo, AmmoDetailsSO ammoDetailsSO)
    {
        float aimAngle = 360f / ammoArray.Length;
        float weaponAimAngle = 0f;
        ammoSpeed = Random.Range(ammoDetailsSO.ammoSpeedMin, ammoDetailsSO.ammoSpeedMax);

        for (int i = 0; i < ammo.Length; i++)
        {
            //Direction calculations
            float projectileDirXPosition = startPoint.x + Mathf.Sin((weaponAimAngle * Mathf.PI) / 180) * radius;
            float projectileDirYPosition = startPoint.y + Mathf.Cos((weaponAimAngle * Mathf.PI) / 180) * radius;

            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            fireDirectionVector = (projectileVector - startPoint).normalized * ammoSpeed * Time.deltaTime;

            ammo[i].GetComponent<Rigidbody2D>().velocity = new Vector2(fireDirectionVector.x, fireDirectionVector.y);

            weaponAimAngle += aimAngle;
        }
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

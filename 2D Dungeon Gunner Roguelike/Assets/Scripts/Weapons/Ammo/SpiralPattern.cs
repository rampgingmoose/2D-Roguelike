using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralPattern : MonoBehaviour, IFireable
{
    [Header("Private Variables")]
    private Vector3 startPoint;
    private const float radius = 1f;
    private float patternAimAngle { get; set; }

    [Header("Ammo Details")]
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private AmmoDetailsSO ammoDetailsSO;
    [SerializeField] private Ammo[] ammoArray;
    [SerializeField] private float ammoChargeTimer;
    [SerializeField] private float ammoRange;

    private void Awake()
    {
        startPoint = transform.position;
    }

    private void OnEnable()
    {
        foreach (Ammo ammo in ammoArray)
        {
            ammo.transform.position = transform.position;
        }
    }

    private void Update()
    {
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
    }

    private void FixedUpdate()
    {
        SetFireDirection(ammoArray, ammoDetailsSO);

        Vector3 distanceVector = (fireDirectionVector / 2f) * ammoSpeed * Time.fixedDeltaTime;

        transform.Rotate(new Vector3(0f, 0f, ammoDetailsSO.ammoRotationSpeed * Time.deltaTime));

        ammoRange -= distanceVector.magnitude;

        if (ammoRange <= 0)
        {
            DisableAmmo();
        }
    }

    public void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        this.ammoDetailsSO = ammoDetails;

        this.ammoSpeed = ammoSpeed;

        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(patternAimAngle);

        ammoRange = ammoDetailsSO.ammoRange;

        gameObject.SetActive(true);

        foreach (Ammo ammo in ammoArray)
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
        float aimAngle = (-1f + Mathf.Sqrt(5f)) / 2f;
        patternAimAngle = Mathf.PI * 2f * aimAngle;
        ammoSpeed = Random.Range(ammoDetailsSO.ammoSpeedMin, ammoDetailsSO.ammoSpeedMax);

        for (int i = 0; i <= ammoArray.Length - 1; i++)
        {
            //Direction calculations
            float projectileDirXPosition = ammo[i].transform.position.x + (aimAngle * Mathf.Cos(patternAimAngle));
            float projectileDirYPosition = ammo[i].transform.position.y + (aimAngle * Mathf.Sin(patternAimAngle));

            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            Vector3 projectileMoveDirectionVector = (projectileVector - ammo[i].transform.position).normalized * ammoSpeed * Time.fixedDeltaTime;

            ammo[i].GetComponent<Rigidbody2D>().velocity = new Vector3(projectileMoveDirectionVector.x, projectileMoveDirectionVector.y, 0f);

            patternAimAngle += aimAngle;
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

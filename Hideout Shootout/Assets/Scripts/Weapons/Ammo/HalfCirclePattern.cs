using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfCirclePattern : MonoBehaviour, IFireable
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

    private bool coroutine = false;

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
        CreatePattern(ammoArray, ammoDetailsSO);

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

        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

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
    private void CreatePattern(Ammo[] ammoArray, AmmoDetailsSO ammoDetailsSO)
    {
        float aimAngle = (-1f + Mathf.Sqrt(5f)) / 2f;
        patternAimAngle = Mathf.PI * 2f * aimAngle;
        ammoSpeed = Random.Range(ammoDetailsSO.ammoSpeedMin, ammoDetailsSO.ammoSpeedMax);

        for (int i = 0; i <= this.ammoArray.Length - 1; i++)
        {
            //Direction calculations
            float projectileDirXPosition = ammoArray[i].transform.position.x + (aimAngle * Mathf.Cos(patternAimAngle));
            float projectileDirYPosition = ammoArray[i].transform.position.y + (aimAngle * Mathf.Sin(patternAimAngle));

            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            Vector3 projectileMoveDirectionVector = (projectileVector - ammoArray[i].transform.position).normalized * ammoSpeed * Time.fixedDeltaTime;

            ammoArray[i].GetComponent<Rigidbody2D>().velocity = new Vector2(projectileMoveDirectionVector.x, projectileMoveDirectionVector.y) * i * ammoSpeed * Time.fixedDeltaTime;

            patternAimAngle += aimAngle;
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

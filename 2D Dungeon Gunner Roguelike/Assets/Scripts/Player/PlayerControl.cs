using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]
    #endregion
    [SerializeField] private MovementDetailsSO movementDetails;

    #region Tooltip

    [Tooltip("The player WeaponShootPosition gameObject in the hierarchy")]

    #endregion
    [SerializeField] private Transform weaponShootPosition;

    private Player player;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isPlayerRolling = false;
    private float playerRollCoolDownTimer = 0f;

    private void Awake()
    {
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    private void Update()
    {
        if (isPlayerRolling) return;

        MovementInput();

        WeaponInput();

        PlayerRollCoolDownTimer();
    }

    //<summary>
    //Player movement Input
    //<summary>
    private void MovementInput()
    {
        //Get movement input
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);

        //Create a direction vector based on the input
        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        //Adjust distance for diagonal movement (pythagorean approximation)
        if (horizontalMovement != 0 && verticalMovement != 0)
        {
            direction *= 0.7f;
        }

        //If there is movement either move or roll
        if (direction != Vector2.zero)
        {
            if (!rightMouseButtonDown)
            {
                //trigger movement event
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
            }
            //else player roll if it is not on cooldown
            else if (playerRollCoolDownTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
            }
        }
        //else trigger idle event
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }

    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    private IEnumerator PlayerRollRoutine(Vector3 direction)
    {
        //minDistance used to decide when to exit coroutine loop
        float minDistance = 0.2f;

        isPlayerRolling = true;

        Vector3 targetPosition = player.transform.position + direction * movementDetails.rollDistance;

        while(Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position,
                movementDetails.rollSpeed, direction,isPlayerRolling);


            yield return waitForFixedUpdate;
        }

        isPlayerRolling = false;

        playerRollCoolDownTimer = movementDetails.rollCooldownTime;

        player.transform.position = targetPosition;
    }

    private void PlayerRollCoolDownTimer()
    {
        if (playerRollCoolDownTimer >= 0f)
        {
            playerRollCoolDownTimer -= Time.deltaTime;
        }
    }

    //<summary>
    //Weapon Input
    //<summary>
    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        //Aim weapon input
        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, 
        out AimDirection playerAimDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        //Calculate direction vector of mouse from weapon shoot position
        weaponDirection = (mouseWorldPosition - weaponShootPosition.position);

        //Calculate direction vector of mouse cursor from player transform position
        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        //Get weapon to cursor angle
        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        //Get player to cursor angle
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        //Set player aim direction
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if collided with something stop player roll coroutine
        StopPlayerRollRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        StopPlayerRollRoutine();
    }

    private void StopPlayerRollRoutine()
    {
        if(playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);

            isPlayerRolling = false;
        }
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Player))]
public class PlayerControl : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]
    #endregion
    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private bool leftMouseDownPreviousFrame = false;
    private int currentWeaponIndex = 1;
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

        SetPlayerAnimationSpeed();
        
        SetStartingWeapon();
    }

    private void SetPlayerAnimationSpeed()
    {
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void SetStartingWeapon()
    {
        int index = 1;

        foreach(Weapon weapon in player.weaponList)
        {
            if (weapon.weaponDetailsSO == player.playerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
        }
        index++;
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

        //Fire weapon input
        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

        //Reload weapon input
        ReloadWeaponInput();
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, 
        out AimDirection playerAimDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        //Calculate direction vector of mouse from weapon shoot position
        weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

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

    private void FireWeaponInput(Vector3 weaponDirection,float weaponAngleDegrees, float playerAngleDegrees, 
        AimDirection playerAimDirection)
    {
        //Fire weapon when left mouse button is clicked
        if (Input.GetMouseButton(0))
        {
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame,playerAimDirection, playerAngleDegrees, 
                weaponAngleDegrees, weaponDirection);
            leftMouseDownPreviousFrame = true;
        }
        else
        {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void SetWeaponByIndex(int weaponIndex)
    {
        if (weaponIndex - 1 < player.weaponList.Count)
        {
            currentWeaponIndex = weaponIndex;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
        }
    }

    private void ReloadWeaponInput()
    {
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();

        //If current weapon is reloading return
        if (currentWeapon.isWeaponReloading) 
            return;

        //remaining ammo is less than clip capacity then return and not infinite ammo then return
        if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetailsSO.weaponClipCapacity && !currentWeapon.weaponDetailsSO.hasInfiniteAmmo)
            return;

        //If ammo in clip equals clip capacity then return
        if (currentWeapon.weaponRemainingAmmo == currentWeapon.weaponDetailsSO.weaponClipCapacity)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            //Call the reload weapon event
            player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
        }
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

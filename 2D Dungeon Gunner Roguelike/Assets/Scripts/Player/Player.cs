using UnityEngine;
using UnityEngine.Rendering;


#region REQUIRE COMPONENTS
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(Health))]
[RequireComponent (typeof(PlayerControl))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent (typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
#endregion REQUIRE COMPONENTS

public class Player : MonoBehaviour
{
    public PlayerDetailsSO playerDetails;
    public Health health;
    public IdleEvent idleEvent;
    public AimWeaponEvent aimWeaponEvent;
    public MovementByVelocityEvent movementByVelocityEvent;
    public MovementToPositionEvent movementToPositionEvent;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private void Awake()
    {
        //Load components
        health = GetComponent<Health>();
        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void Initailize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        SetPlayerHealth();
    }

    //<summary>
    //Set Player Health from PlayerDetailsSO
    //<summary>
    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }
}

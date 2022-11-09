using System.Collections.Generic;
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
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
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
    public FireWeaponEvent fireWeaponEvent;
    public WeaponFiredEvent weaponFiredEvent;
    public ReloadWeaponEvent reloadWeaponEvent;
    public WeaponReloadedEvent weaponReloadedEvent;
    public SetActiveWeaponEvent setActiveWeaponEvent;
    public ActiveWeapon activeWeapon;
    public MovementByVelocityEvent movementByVelocityEvent;
    public MovementToPositionEvent movementToPositionEvent;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public List<Weapon> weaponList = new List<Weapon>();

    private void Awake()
    {
        //Load components
        health = GetComponent<Health>();
        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent= GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent <WeaponReloadedEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
    }

    public void Initailize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        SetPlayerHealth();
        CreatePlayerStartingWeapons();
    }

    //<summary>
    //Set Player Health from PlayerDetailsSO
    //<summary>
    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }

    private void CreatePlayerStartingWeapons()
    {
        weaponList.Clear();

        //Populate weapon list from starting weapon
        foreach(WeaponDetailsSO weaponDetailsSO in playerDetails.startingWeaponList)
        {
            AddWeaponToPlayer(weaponDetailsSO);
        }
    }

    /// <summary>
    /// Add a weapon to the player weapon dictionary
    /// </summary>
    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetailsSO)
    {
        Weapon weapon = new Weapon() { weaponDetailsSO = weaponDetailsSO,weaponReloadTimer = 0f, weaponClipRemainingAmmo = weaponDetailsSO.weaponClipCapacity,
            weaponRemainingAmmo = weaponDetailsSO.weaponAmmoCapacity,isWeaponReloading = false};

        //Add weapon to the list
        weaponList.Add(weapon);

        //Set weapon position in the list
        weapon.weaponListPosition = weaponList.Count;

        //Set the added weapon as active
        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;
    }
}

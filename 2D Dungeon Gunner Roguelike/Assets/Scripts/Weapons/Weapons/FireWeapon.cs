using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float firePrechargeTimer = 0f;
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private AimWeapon aimWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;
    private ReloadWeaponEvent reloadWeaponEvent;

    private void Awake()
    {
        activeWeapon = GetComponent<ActiveWeapon>();
        aimWeapon = GetComponent<AimWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
    }

    private void OnEnable()
    {
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        fireRateCoolDownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Handle fire weapon event
    /// </summary>
    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponeventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

    /// <summary>
    /// fire weapon
    /// </summary>

    private void WeaponFire(FireWeaponeventArgs fireWeaponEventArgs)
    {
        //Handle weapon Precharge timer
        WeaponPrechargeTimer(fireWeaponEventArgs);

        //Weapon fire
        if (fireWeaponEventArgs.fire)
        {
            if (IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

                ResetCoolDownTimer();

                ResetPrechargeTimer();
            }
        }
    }

    /// <summary>
    /// Handle Weapon Precharge
    /// </summary>
    private void WeaponPrechargeTimer(FireWeaponeventArgs fireWeaponeventArgs)
    {
        //Weapon Precharge
        if (fireWeaponeventArgs.firePreviousFrame)
        {
            //Decrease precharge timer if fire button held previous frame
            firePrechargeTimer -= Time.deltaTime;
        }
        else
        {
            ResetPrechargeTimer();
        }
    }

    /// <summary>
    /// Returns true if the weapon is ready to fire, else returns false
    /// </summary>
    private bool IsWeaponReadyToFire()
    {
        //if there is no ammo and weapon doesn't have infinite amoo then return false.
        if (activeWeapon.GetCurrentWeapon().weaponRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetailsSO.hasInfiniteAmmo)
            return false;

        //If the weapon is reloading then return false
        if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
            return false;

        //If the weapon isn't precharged or is cooling down then return false
        if (firePrechargeTimer > 0f || fireRateCoolDownTimer > 0f)
            return false;

        //If the weapon has no ammo in the clip and the weapon doesn't have infinite ammo then return false
        if (!activeWeapon.GetCurrentWeapon().weaponDetailsSO.hasInfiniteClipCapacity && activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo <= 0)
        {
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);

            return false;
        }

        //Weapon is ready to fire - return true
        return true;
    }

    /// <summary>
    /// Set up the ammo using an ammo gameObject and component from the object pool
    /// </summary>
    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }
    /// <summary>
    /// Coroutine to spawn multiple ammo per shot if specified in the ammo details
    /// </summary>
    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        int ammoCounter = 0;

        //Get random ammo per shot
        int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);

        //Get random interval between ammo
        float ammoSpawnInterval;

        if (ammoPerShot > 1)
        {
            ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
        }
        else
        {
            ammoSpawnInterval = 0f;
        }

        //Loop for number of ammo per shot
        while (ammoCounter < ammoPerShot)
        {
            ammoCounter++;

            //Get ammo prefab from array
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            //Get random speed value
            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

            //Get GameObject with IFireable component
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);

            //Initialize ammo
            ammo.InitializeAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        //Reduce ammo clip count if not infinite clip capacity
        if (!activeWeapon.GetCurrentWeapon().weaponDetailsSO.hasInfiniteClipCapacity)
        {
            activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
            activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
        }

        //Call weapon fired event
        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());

        WeaponShootEffectMethod(aimAngle);

        WeaponSoundEffect();
    }

    private void ResetCoolDownTimer()
    {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetailsSO.weaponFireRate;
    }

    private void ResetPrechargeTimer()
    {
        //Reset precharge timer
        firePrechargeTimer = activeWeapon.GetCurrentWeapon().weaponDetailsSO.weaponPrechargeTime;
    }

    private void WeaponShootEffectMethod(float aimAngle)
    {
        //Process if there is a shoot effect & prefab
        if(activeWeapon.GetCurrentWeapon().weaponDetailsSO.weaponShootEffectSO != null && activeWeapon.GetCurrentWeapon().
            weaponDetailsSO.weaponShootEffectSO.weaponShootEffectPrefab != null)
        {
            //Get weapon shoot effect gameobject from the pool with particle system component
            WeaponShootEffect weaponShootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent(activeWeapon.
                GetCurrentWeapon().weaponDetailsSO.weaponShootEffectSO.weaponShootEffectPrefab,
                activeWeapon.GetShootEffectPosition(), Quaternion.identity);

            //Set shoot effect
            weaponShootEffect.SetShootEffect(activeWeapon.GetCurrentWeapon().weaponDetailsSO.weaponShootEffectSO, aimAngle);

            //Set gameobject active (the particle system is set to automatically disable the gameobject once finished
            weaponShootEffect.gameObject.SetActive(true);
        }
    }

    private void WeaponSoundEffect()
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetailsSO.weaponFiringSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetailsSO.weaponFiringSoundEffect);
        }
    }
}

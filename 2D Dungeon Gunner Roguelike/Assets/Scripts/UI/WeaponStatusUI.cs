using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponStatusUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with image component on the child WeaponImage gameObject")]
    #endregion
    [SerializeField] private Image weaponImage;
    #region Tooltip
    [Tooltip("Populate with the Transform from the child AmmoHolder gameObject")]
    #endregion
    [SerializeField] private Transform ammoHolderTransform;
    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child Reloadtext gameObject")]
    #endregion
    [SerializeField] private TextMeshProUGUI reloadText;
    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child ammoRemainingText gameObject")]
    #endregion
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child weaponNameText gameObject")]
    #endregion
    [SerializeField] private TextMeshProUGUI weaponNameText;
    #region Tooltip
    [Tooltip("Populate with the RectTransform of the child gameObject reloadBar")]
    #endregion
    [SerializeField] private Transform reloadBar;
    #region Tooltip
    [Tooltip("Populate with the Image component of the child gameObject BarImage")]
    #endregion
    [SerializeField] private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadtextCoroutine;

    private void Awake()
    {
        //Get player
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        //Subscribe to set active weapon event
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;

        //Subscribe to weapon fired event
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;

        //Subscribe to reload weapon event
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;

        //Subscribe to weapon reloaded event
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        //unsubscribe to weapon fired event
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;

        //unsubscribe to weapon fired event
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;

        //unsubscribe to reload weapon event
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;

        //unsubscribe to weapon reloaded event
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        //Update active weaponstatus on the UI
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    /// <summary>
    /// Handle set active weapon event on the UI
    /// </summary>
    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    /// <summary>
    /// Set active weapon on the UI
    /// </summary>
    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetailsSO);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        //If set weapon is still reloading the update reload bar
        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(weapon);
    }

    /// <summary>
    /// Handle Weapon fired event on the UI
    /// </summary>
    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    /// <summary>
    /// Weapon fired update UI
    /// </summary>
    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    /// <summary>
    /// Handle weapon reload event on the UI
    /// </summary>
    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    /// <summary>
    /// Reload weapon - update the reload bar on the UI
    /// </summary>
    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetailsSO.hasInfiniteClipCapacity)
            return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    /// <summary>
    /// Handle weapon reloaded event on the UI
    /// </summary>
    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    private void WeaponReloaded(Weapon weapon)
    {
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }
    }

    private void UpdateReloadText(Weapon weapon)
    {
        if((!weapon.weaponDetailsSO.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();

            blinkingReloadtextCoroutine = StartCoroutine(StartBlinkingReloadTextCoroutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }

    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetailsSO)
    {
        weaponImage.sprite = weaponDetailsSO.weaponSprite;
    }

    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListPosition + ")" + weapon.weaponDetailsSO.weaponName.ToUpper();
    }

    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetailsSO.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = weapon.weaponRemainingAmmo.ToString() + " / " + weapon.weaponDetailsSO.weaponClipCapacity.ToString();
        }
    }

    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        //Instantiate ammo Icon prefab
        for(int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
        {
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);

            ammoIconList.Add(ammoIcon);
        }
    }

    private void ClearAmmoLoadedIcons()
    {
        //Loop through icon gameobjects and destroy
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    /// <summary>
    /// Animate reload weapon bar coroutine
    /// </summary>
    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        //set the reload bar to red
        barImage.color = Color.red;

        //Animate the weapon reload bar
        while (currentWeapon.isWeaponReloading)
        {
            //update reload bar
            float barFill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetailsSO.weaponReloadTime;

            //update bar fill
            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

            yield return null;
        }
    }

    private void StopReloadWeaponCoroutine()
    {
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    /// <summary>
    /// Stop the reload weapon coroutine, fully fill reload bar and reset color to green
    /// </summary>
    private void ResetWeaponReloadBar()
    {
        StopReloadWeaponCoroutine();

        barImage.color = Color.green;

        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    /// <summary>
    /// start the coroutine to blink the reload weapon text
    /// </summary>
    private IEnumerator StartBlinkingReloadTextCoroutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// Stop the blinking reload text
    /// </summary>
    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();

        reloadText.text = "";
    }

    /// <summary>
    /// stop the blinking reload text coroutine
    /// </summary>
    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadtextCoroutine != null)
        {
            StopCoroutine(blinkingReloadtextCoroutine);
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }
#endif
    #endregion
}

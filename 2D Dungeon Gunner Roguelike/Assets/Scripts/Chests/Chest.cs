using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MaterializeEffect))]
public class Chest : MonoBehaviour, IUseable
{
    #region Tooltip
    [Tooltip("Set this to the color to be used for the materialization effect")]
    #endregion
    [ColorUsage(false, true)]
    [SerializeField] private Color materializeColor;
    #region Tooltip
    [Tooltip("Set this to the time it will take to materialize the chest")]
    #endregion
    [SerializeField] private float materializeTime;
    #region Tooltip
    [Tooltip("Populate with ItemSpawnPoint transform")]
    #endregion
    [SerializeField] private Transform itemSpawnPointTransform;

    private int healthPercent;
    private WeaponDetailsSO weaponDetails;
    private int ammoPercent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;
    private bool isEnabled = false;
    private ChestState chestState = ChestState.closed;
    private GameObject chestItemGameObject;
    private ChestItem chestItem;
    private TextMeshPro messageTextTMP;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
        messageTextTMP = GetComponent<TextMeshPro>();
    }

    public void Initialize(bool shouldMaterialize, int healthPercent, WeaponDetailsSO weaponDetails, int ammoPercent)
    {
        this.healthPercent = healthPercent;
        this.weaponDetails = weaponDetails;
        this.ammoPercent = ammoPercent;

        if (shouldMaterialize)
        {
            StartCoroutine(MaterializeChest());
        }
        else
        {
            EnableChest();
        }
    }

    private IEnumerator MaterializeChest()
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader, materializeColor, materializeTime,
            spriteRendererArray, GameResources.Instance.litMaterial));

        EnableChest();
    }

    /// <summary>
    /// Enable the chest
    /// </summary>
    private void EnableChest()
    {
        isEnabled = true;
    }

    /// <summary>
    /// Use the chest - action will vary depending on the chest state
    /// </summary>
    public void UseItem()
    {
        if (!isEnabled) return;

        switch (chestState)
        {
            case ChestState.closed:
                OpenChest();
                break;
            case ChestState.healthItem:
                CollectHealthItem();
                break;
            case ChestState.ammoItem:
                CollectAmmoItem();
                break;
            case ChestState.weaponItem:
                CollectWeaponItem();
                break;
            case ChestState.empty:
                return;

            default:
                return;
        }
    }

    private void OpenChest()
    {
        animator.SetBool(Settings.use, true);

        //chest open sound effect
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.chestOpenSoundEffect);

        //Check if player already has the weapon - if so set weapon to null
        if (weaponDetails != null)
        {
            if (GameManager.Instance.GetPlayer().isWeaponHeldByPlayer(weaponDetails))
                weaponDetails = null;
        }

        UpdateChestState();
    }

    /// <summary>
    /// Create items based on what should be spawned and the chest state
    /// </summary>
    private void UpdateChestState()
    {
        if (healthPercent != 0)
        {
            chestState = ChestState.healthItem;
            InstantiateHealthItem();
        }
        else if (ammoPercent != 0)
        {
            chestState = ChestState.ammoItem;
            InstantiateAmmoItem();
        }
        else if (weaponDetails != null)
        {
            chestState = ChestState.weaponItem;
            InstantiateWeaponItem();
        }
        else
        {
            chestState = ChestState.empty;
        }
    }

    /// <summary>
    /// Instantiate a chest item
    /// </summary>
    private void InstantiateItem()
    {
        chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform);

        chestItem = chestItemGameObject.GetComponent<ChestItem>();
    }

    /// <summary>
    /// Instantiate a health item for the player to collect
    /// </summary>
    private void InstantiateHealthItem()
    {
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.heartIcon, healthPercent.ToString() + "%", itemSpawnPointTransform.position, materializeColor);
    }

    /// <summary>
    /// Collect the health item and add it to the player's health
    /// </summary>
    private void CollectHealthItem()
    {
        //Check if item exists and has been materialized
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        //Add health to player
        GameManager.Instance.GetPlayer().health.AddHealth(healthPercent);

        //Play sound effect
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.healthPickUpSoundEffect);

        healthPercent = 0;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }

    /// <summary>
    /// Instantiate a ammo item for the player to collect
    /// </summary>
    private void InstantiateAmmoItem()
    {
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.bulletIcon, ammoPercent.ToString() + "%", itemSpawnPointTransform.position, materializeColor);
    }

    private void CollectAmmoItem()
    {
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        Player player = GameManager.Instance.GetPlayer();

        //Update ammo for current weapon
        player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), ammoPercent);

        //Play pickup sound effect
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.ammoPickupSoundEffect);

        ammoPercent = 0;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }

    /// <summary>
    /// Instantiate a weapon item for the player to collect
    /// </summary>
    private void InstantiateWeaponItem()
    {
        InstantiateItem();

        chestItemGameObject.GetComponent<ChestItem>().Initialize(weaponDetails.weaponSprite, weaponDetails.weaponName, itemSpawnPointTransform.position, materializeColor);
    }

    /// <summary>
    /// Collect the weapon and add it to the players weapons list
    /// </summary>
    private void CollectWeaponItem()
    {
        //Check item exists and has been materialized
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        //If the player doesn't already have the weapon, then add it to the player's inventory
        if (!GameManager.Instance.GetPlayer().isWeaponHeldByPlayer(weaponDetails))
        {
            //Add weapon to player
            GameManager.Instance.GetPlayer().AddWeaponToPlayer(weaponDetails);

            //Player pickup sound effect
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.weaponPickupSoundEffect);
        }
        else
        {
            //display message saying the player already has that weapon
            StartCoroutine(DisplayMessage("WEAPON ALREADY \n EQUIPPED", 5f));
        }
        weaponDetails = null;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }

    /// <summary>
    /// Display message above the chest
    /// </summary>
    private IEnumerator DisplayMessage(string messageText, float messageDisplayTime)
    {
        messageTextTMP.text = messageText;

        yield return new WaitForSeconds(messageDisplayTime);

        messageTextTMP.text = "";
    }
}

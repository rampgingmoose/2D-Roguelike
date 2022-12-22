using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region Header Dungeon
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region ToolTip
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
    #endregion

    public RoomNodeTypeListSO roomNodeTypeList;

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion Header PLAYER
    #region Tooltip
    [Tooltip("The current player scriptable object - this is used to reference the current player between scenes")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region Header MUSIC
    [Space(10)]
    [Header("MUSIC")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with main menu music scriptable object")]
    #endregion
    public MusicTrackSO mainMenuMusic;
    #region Tooltip
    [Tooltip("Populate with the music master mixer group")]
    #endregion
    public AudioMixerGroup musicMasterMixerGroup;
    #region Tooltip
    [Tooltip("music on full snapshot")]
    #endregion
    public AudioMixerSnapshot musicFullOnSnapshot;
    #region Tooltip
    [Tooltip("music on low snapshot")]
    #endregion
    public AudioMixerSnapshot musicLowOnSnapshot;
    #region Tooltip
    [Tooltip("music off snapshot")]
    #endregion
    public AudioMixerSnapshot musicOffSnapshot;

    #region Header SOUNDS
    [Space(10)]
    [Header("SOUNDS")]
    #endregion
    #region
    [Tooltip("Populate with the sounds master mixer group")]
    #endregion
    public AudioMixerGroup soundsMasterMixerGroup;
    #region
    [Tooltip("Door open and close sound effect")]
    #endregion
    public SoundEffectSO doorOpenSoundEffect;
    #region Tooltip
    [Tooltip("Populate with the table flip sound effect")]
    #endregion
    public SoundEffectSO tableFlipSoundEffect;
    #region Tooltip
    [Tooltip("Populate with the chest open sound effect")]
    #endregion
    public SoundEffectSO chestOpenSoundEffect;
    #region Tooltip
    [Tooltip("Populate with the health pickup sound effect")]
    #endregion
    public SoundEffectSO healthPickUpSoundEffect;
    #region Tooltip
    [Tooltip("Populate with the ammo pickup sound effect")]
    #endregion
    public SoundEffectSO ammoPickupSoundEffect;
    #region Tooltip
    [Tooltip("Populate with the weapon pickup sound effect")]
    #endregion
    public SoundEffectSO weaponPickupSoundEffect;

    #region Header MATERIALS
    [Space(10)]
    [Header("Materials")]
    #endregion
    #region Header Tooltip
    [Tooltip("Dimmed Material")]
    #endregion
    public Material dimmedMaterial;

    #region Tooltip
    [Tooltip("Sprite-Lit-Default Material")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("Populate with the Variable Lite Shader")]
    #endregion
    public Shader variableLitShader;
    #region Tooltip
    [Tooltip("Populate with the materialize shader")]
    #endregion
    public Shader materializeShader;

    #region Header SPECIAL TILEMAP TILES
    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]
    #endregion
    #region Tooltip
    [Tooltip("Collision tiles that the enemies can navigate to")]
    #endregion
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    #region Tooltip
    [Tooltip("Preferred path tile for enemy navigation")]
    #endregion
    public TileBase preferredEnemyPathTiles;

    #region Header UI
    [Space(10)]
    [Header("UI")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with ammoArray icon prefab")]
    #endregion
    public GameObject ammoIconPrefab;
    #region
    [Tooltip("Populate with heart image prefab")]
    #endregion
    public GameObject heartImage;

    #region
    [Tooltip("Populate with half heart image prefab")]
    #endregion
    public GameObject halfHeartImage;

    #region Header CHESTS
    [Space(10)]
    [Header("CHESTS")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with chest Item prefab")]
    #endregion
    public GameObject chestItemPrefab;
    #region Tooltip
    [Tooltip("Populate with heart icon sprite")]
    #endregion
    public Sprite heartIcon;
    #region Tooltip
    [Tooltip("Populate with bullet icon sprite")]
    #endregion
    public Sprite bulletIcon;

    #region Header MINIMAP
    [Space(10)]
    [Header("MINIMAP")]
    #endregion
    #region
    [Tooltip("Populate with boss minimap Prefab")]
    #endregion
    public GameObject bossMinimapPrefab;

    #region Validation
#if UNITY_EDITOR
    //Validate the scriptable object details entered
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTiles), preferredEnemyPathTiles);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenSoundEffect), doorOpenSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(heartImage), heartImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(tableFlipSoundEffect), tableFlipSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestOpenSoundEffect), chestOpenSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(healthPickUpSoundEffect), healthPickUpSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoPickupSoundEffect), ammoPickupSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPickupSoundEffect), weaponPickupSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(materializeShader), materializeShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestItemPrefab), chestItemPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(heartIcon), heartIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(bulletIcon), bulletIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(bossMinimapPrefab), bossMinimapPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicFullOnSnapshot), musicFullOnSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicLowOnSnapshot), musicLowOnSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOffSnapshot), musicOffSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(mainMenuMusic), mainMenuMusic);
    }

#endif
    #endregion
}

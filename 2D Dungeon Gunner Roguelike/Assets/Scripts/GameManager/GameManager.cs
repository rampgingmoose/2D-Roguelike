using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonoBehavior<GameManager>
{
    #region Header GAMEOBJECT REFRENCES
    [Space(10)]
    [Header("GAMEOBJECT REFERENCES")]
    #endregion Header
    #region Tooltip
    [Tooltip("Populate with the MessageText textMeshPro component in the FadeScreenUI")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI messageTextUI;
    #region Tooltip
    [Tooltip("Populate with the FadeImage canvasGroup component in the FadeScreenUI")]
    #endregion Tooltip
    [SerializeField] private CanvasGroup fadeScreenImageCanvasGroup;

    #region Header DUNGEON LEVELS
    [Space(10)]
    [Header("DUNGEON LEVELS")]
    #endregion Header DUNGEON LEVELS
    #region Tooltip
    [Tooltip("Populate with the dungeon level scriptable objects")]
    #endregion Tooltip

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip
    [Tooltip("Populate with the starting dungeon level for testing, first level = 0")]
    #endregion Tooltip
    [SerializeField] private int currentDungeonLevelListIndex = 0;
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;
    private InstantiatedRoom bossRoom;
    [SerializeField] public BossHealthBarUI bossHealthBarUI;

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;

    private long playerScore;
    private int scoreMultiplier;

    protected override void Awake()
    {
        //Call base class
        base.Awake();

        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        InstantiatePlayer();
    }

    private void OnEnable()
    {
        StaticEventsHandler.OnRoomChanged += StaticEventsHandler_OnRoomChanged;
        StaticEventsHandler.OnPointsScored += StaticEventsHandler_OnPointsScored;
        StaticEventsHandler.OnMultiplier += StaticEventsHandler_OnMultiplier;
        StaticEventsHandler.OnRoomEnemiesDefeated += StaticEventsHandler_OnRoomEnemiesDefeated;
        player.destroyedEvent.OnDestroy += Player_OnDestroy;
    }

    private void OnDisable()
    {
        StaticEventsHandler.OnRoomChanged -= StaticEventsHandler_OnRoomChanged;
        StaticEventsHandler.OnPointsScored -= StaticEventsHandler_OnPointsScored;
        StaticEventsHandler.OnMultiplier -= StaticEventsHandler_OnMultiplier;
        StaticEventsHandler.OnRoomEnemiesDefeated -= StaticEventsHandler_OnRoomEnemiesDefeated;
        player.destroyedEvent.OnDestroy -= Player_OnDestroy;
    }

    private void StaticEventsHandler_OnMultiplier(MultiplierArgs multiplierArgs)
    {
        if (multiplierArgs.multiplier)
        {
            scoreMultiplier++;
        }
        else
        {
            scoreMultiplier--;
        }

        //Clamp between 1 and 10
        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 10);

        StaticEventsHandler.CallScoreChangedEvent(playerScore, scoreMultiplier);
    }

    private void StaticEventsHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
    {
        //Increase player score
        playerScore += pointsScoredArgs.points * scoreMultiplier;

        //Call score changed event
        StaticEventsHandler.CallScoreChangedEvent(playerScore, scoreMultiplier);
    }

    /// <summary>
    /// Handle room changed event
    /// </summary>
    private void StaticEventsHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    private void StaticEventsHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }
    
    private void Player_OnDestroy(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        previousGameState = gameState;
        gameState = GameState.gameLost;
    }

    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;

        //Set score as 0 to start
        playerScore = 0;

        //Set score Multiplier as 1
        scoreMultiplier = 1;

        //Set screen to black
        StartCoroutine(FadeScreen(0f, 1f, 0f, Color.black));
    }

    private void Update()
    {
        HandleGameState();

        //For testing
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    gameState = GameState.gameStarted;
        //}
    }

    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        player = playerGameObject.GetComponent<Player>();

        player.Initailize(playerDetails);
    }

    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.gameStarted:

                //Play first level
                PlayDungeonLevel(currentDungeonLevelListIndex);

                gameState = GameState.playingLevel;

                //Trigger room enemies defeated since we start in the entrance where there are no enemies (just in case of a level with only boss rooms)
                RoomEnemiesDefeated();

                break;

            case GameState.levelCompleted:

                StartCoroutine(LevelCompleted());

                break;

            //handle the game being won (only trigger this once - test the previous game state to do this)
            case GameState.gameWon:

                if (previousGameState != GameState.gameWon)
                    StartCoroutine(GameWon());

                break;

            //handle the game being lost (only trigger this once - test the previous game state to do this)
            case GameState.gameLost:

                if (previousGameState != GameState.gameLost)
                {
                    StopAllCoroutines(); //prevent messages if you clear the level just as the player is killed
                    StartCoroutine(GameLost());
                }

                break;

            case GameState.restartGame:

                RestartGame();

                break;
        }
    }

    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;

        ////Debug
        //Debug.Log(room.prefab.name.ToString());
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        //Build dungeon for level
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccessfully)
        {
            Debug.Log("Couldn't build dungeon from specified rooms and node graphs");
        }

        StaticEventsHandler.CallRoomChangedEvent(currentRoom);

        //Set player roughly in the middle of the room
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, 
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        //Get nearest spawn point in room nearest to player
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);

        if (bossHealthBarUI != null)
        {
            bossHealthBarUI.DisableBossHealthBar();
        }

        StartCoroutine(DisplayDungeonLevelText());
    }

    /// <summary>
    /// Room enemies defeated - test if all dungeon rooms have been cleared of enemies - if so load
    /// next dungeon game level
    /// </summary>
    private void RoomEnemiesDefeated()
    {
        //Initialize dungeon as being cleared - but then test each room
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;

        foreach(KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }
            if (!keyValuePair.Value.isClearedOfEnemies)
            {
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }

        //Set game state
        //If dungeon level is completely cleared (i.e. dungeon cleared apart from boss and there is no boss room OR dungeon cleared apart from boss 
        //and boss room is also cleared
        if((isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
        {
            //Are there more dungeon levels then
            if (currentDungeonLevelListIndex < dungeonLevelList.Count - 1)
            {
                gameState = GameState.levelCompleted;
            }
            else
            {
                gameState = GameState.gameWon;
            }
        }
        else if (isDungeonClearOfRegularEnemies)
        {
            gameState = GameState.bossStage;

            StartCoroutine(BossStage());
        }
    }

    /// <summary>
    /// Enter Boss Stage
    /// </summary>
    private IEnumerator BossStage()
    {
        //Activate boss room
        bossRoom.gameObject.SetActive(true);

        //Unlock doors
        bossRoom.UnlockDoors(0f);

        yield return new WaitForSeconds(2f);

        //Fade canvas to a slightly transparant color to display text message
        yield return StartCoroutine(FadeScreen(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        //Display boss message
        yield return StartCoroutine(DisplayMessageRoutine("KILLING EVERYONE WON'T CLEAR YOUR DEBT" + "\n\n TO THE SMILEY GANG "
            + GameResources.Instance.currentPlayer.playerName + "!" + "\n\n IF YOU COME TO THE BOSS ROOM" + "\n\n I'M SURE WE CAN MAKE A DEAL!",
            Color.white, 5f));

        //Fade canvas back to normal
        yield return StartCoroutine(FadeScreen(1f, 0f, 2f, new Color(0f, 0f, 0f, 0f)));
    }

    /// <summary>
    /// Show level as being completed - load next level
    /// </summary>
    private IEnumerator LevelCompleted()
    {
        gameState = GameState.playingLevel;

        if (bossHealthBarUI != null)
        {
            bossHealthBarUI.DisableBossHealthBar();
        }

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(FadeScreen(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        yield return StartCoroutine(DisplayMessageRoutine("THERE ARE STILL OTHERS \n\n YOU OWE A DEBT TO "
            + GameResources.Instance.currentPlayer.playerName + "!\n\n" + " FIND THEM ON THE LOWER FLOORS", Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("COLLECT ANY ITEMS YOU NEED, THEN PRESS ENTER \n\n" +
            "TO DESCEND FURTHER INTO THE HIDEOUT", Color.white, 5f));


        //When player presses the return key proceed to the next level
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null; //to avoid enter being detected twice

        //Increase index to next level
        currentDungeonLevelListIndex++;

        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    private IEnumerator FadeScreen(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundGroundColor)
    {
        Image image = fadeScreenImageCanvasGroup.GetComponent<Image>();
        image.color = backgroundGroundColor;

        float time = 0f;

        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            fadeScreenImageCanvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }
    }

    private IEnumerator DisplayDungeonLevelText()
    {
        //Set screen to black
        StartCoroutine(FadeScreen(0f, 1f, 0f, Color.black));

        GetPlayer().playerControl.DisablePlayer();

        string messageText = "Floor " + (currentDungeonLevelListIndex + 1).ToString() + ("\n\n") + dungeonLevelList[currentDungeonLevelListIndex].levelName;

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        GetPlayer().playerControl.EnablePlayer();

        //Fade in
        StartCoroutine(FadeScreen(1f, 0f, 2f, Color.black));
    }

    /// <summary>
    /// Display the message text for displaySeconds - if displaySeconds = 0 then the message is displayed until the return key is pressed
    /// </summary>
    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        //Set text
        messageTextUI.SetText(text);
        messageTextUI.color = textColor;

        //Display the message for the given time
        if (displaySeconds > 0)
        {
            float timer = displaySeconds;

            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        else
        //else display the message until the return key is pressed
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;

        //Clear Text
        messageTextUI.SetText("");
    }

    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;

        GetPlayer().playerControl.DisablePlayer();

        //Fade Out
        yield return StartCoroutine(FadeScreen(0f, 1f, 2f, Color.black));

        //Display Game Won
        yield return StartCoroutine(DisplayMessageRoutine("FINALLY YOUR DEBT IS CLEARED "
            + GameResources.Instance.currentPlayer.playerName + "! YOU'VE LEARNED YOUR LESSON ABOUT \n\n BORROWING MONEY FOR THE SODA MACHINE",
            Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("YOUR FINAL SCORE "
            + playerScore.ToString("###,###0"), Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("THOUGH NOW THAT YOU THINK ABOUT IT \n\n " +
            "YOU MIGHT NEED TO BORROW SOME MONEY FOR THE SNACK MACHINE \n\n " +
            "PRESS ENTER TO RESTART THE GAME", Color.white, 5f));

        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        GetPlayer().playerControl.DisablePlayer();

        yield return new WaitForSeconds(1f);

        //Fade Out
        yield return StartCoroutine(FadeScreen(0f, 1f, 2f, Color.black));

        //disable enemies (FindObjects of type is resource hungry - but will suffice in this instance since the game has ended)
        Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();

        foreach(Enemy enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }

        yield return StartCoroutine(DisplayMessageRoutine("YOUR DEBT IS STILL OUTSTANDING "
            + GameResources.Instance.currentPlayer.playerName + "! \n\n YOUR FAMILY WILL HAVE TO SETTLE IT NOW", Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("YOUR FINAL SCORE "
            + playerScore.ToString("###,###0"), Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS ENTER TO RESTART THE GAME", Color.white, 5f));

        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public Sprite GetPlayerMiniMapIcon()
    {
        return playerDetails.playerMiniMapIcon;
    }

    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevelListIndex];
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(messageTextUI), messageTextUI);
        HelperUtilities.ValidateCheckNullValue(this, nameof(fadeScreenImageCanvasGroup), fadeScreenImageCanvasGroup);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion Validation
}

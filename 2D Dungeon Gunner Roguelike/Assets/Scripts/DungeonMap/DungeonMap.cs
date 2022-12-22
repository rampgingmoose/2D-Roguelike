using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DungeonMap : SingletonMonoBehavior<DungeonMap>
{
    #region Header GameObject References
    [Space(10)]
    [Header("GameObject References")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the MinimapUI gameObject")]
    #endregion
    [SerializeField] private GameObject minimapUI;
    private Camera dungeonMapCamera;
    private Camera cameraMain;

    private void Start()
    {
        //cache main camera
        cameraMain = Camera.main;

        //Get playera transform
        Transform playerTransform = GameManager.Instance.GetPlayer().transform;

        //Populate player as cinemachine camera target
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        //Get dungeonmap Camera
        dungeonMapCamera = GetComponentInChildren<Camera>();
        dungeonMapCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        //If the left mouse button is pressed and the gameState is dungeonOverview map then get the room clicked
        if (Input.GetMouseButton(0) && GameManager.Instance.gameState == GameState.dungeonOverviewMap)
        {
            GetRoomClicked();
        }
    }

    /// <summary>
    /// Get the room clicked on the map
    /// </summary>
    private void GetRoomClicked()
    {
        //Convert screen position to world position
        Vector3 worldPosition = dungeonMapCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

        //Check for collisions at cursor point
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(new Vector2(worldPosition.x, worldPosition.y), 1f);

        //Check if any of the colliders are a room
        foreach(Collider2D collider2D in collider2DArray)
        {
            if (collider2D.GetComponent<InstantiatedRoom>() != null)
            {
                InstantiatedRoom instantiatedRoom = collider2D.GetComponent<InstantiatedRoom>();

                //If clicked room is clear of enemies and the room has been previously visited then move the player to that room
                if (instantiatedRoom.room.isClearedOfEnemies && instantiatedRoom.room.isPreviouslyVisited)
                {
                    //Move player to room
                    StartCoroutine(MovePlayerToRoom(worldPosition, instantiatedRoom.room));
                }
            }
        }
    }

    /// <summary>
    /// Move Player to selected room
    /// </summary>
    private IEnumerator MovePlayerToRoom(Vector3 worldPosition, Room room)
    {
        //Call room changed event
        StaticEventsHandler.CallRoomChangedEvent(room);

        ClearDungeonOverviewMap();

        //Fade out screen to black immediately
        yield return StartCoroutine(GameManager.Instance.FadeScreen(0f, 1f, 0f, Color.black));

        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        //Get nearest spawn point in room nearest to player
        Vector3 spawnPosition = HelperUtilities.GetSpawnPositionNearestToPlayer(worldPosition);

        //Move player to new location - spawn them at the closest spawn point
        GameManager.Instance.GetPlayer().transform.position = spawnPosition;

        //Fade screen back in
        yield return StartCoroutine(GameManager.Instance.FadeScreen(1f, 0f, 1f, Color.black));

        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();
    }

    /// <summary>
    /// Display dungeon overview map UI
    /// </summary>
    public void DisplayDungeonOverviewMap()
    {
        //Set game state
        GameManager.Instance.previousGameState = GameManager.Instance.gameState;
        GameManager.Instance.gameState = GameState.dungeonOverviewMap;

        //Disable player
        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        //Disable main camera and enable dungeonoverview map camera
        cameraMain.gameObject.SetActive(false);
        dungeonMapCamera.gameObject.SetActive(true);

        //Ensure all rooms are active so they can be displayed
        ActivateRoomsForDisplay();

        //Disable small minimap ui
        minimapUI.SetActive(false);
    }

    /// <summary>
    /// Clear the dungeon overview map UI
    /// </summary>
    public void ClearDungeonOverviewMap()
    {
        //Set gameState
        GameManager.Instance.gameState = GameManager.Instance.previousGameState;
        GameManager.Instance.previousGameState = GameState.dungeonOverviewMap;

        //Enable player
        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();

        //Disable dungeonoverview map camera and enable main camera
        cameraMain.gameObject.SetActive(true);
        dungeonMapCamera.gameObject.SetActive(false);

        minimapUI.SetActive(true);
    }

    /// <summary>
    /// Ensure all rooms are active so they can be displayed
    /// </summary>
    private void ActivateRoomsForDisplay()
    {
        foreach(KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            room.instantiatedRoom.gameObject.SetActive(true);
        }
    }
}

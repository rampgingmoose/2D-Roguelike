using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarTest : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;
    private Grid grid;
    private Tilemap frontTilemap;
    private Tilemap pathTilemap;
    private Vector3Int startGridPosition;
    private Vector3Int endGridPosition;
    private TileBase startPathTile;
    private TileBase finishPathTile;

    //Vector3Int is a non-nullable type so we populate it with this high value in place of a no value
    private Vector3Int noValue = new Vector3Int(9999, 9999, 9999);
    private Stack<Vector3> pathStack;

    private void OnEnable()
    {
        StaticEventsHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventsHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void Start()
    {
        startPathTile = GameResources.Instance.preferredEnemyPathTiles;
        finishPathTile = GameResources.Instance.enemyUnwalkableCollisionTilesArray[0];
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        pathStack = null;
        instantiatedRoom = roomChangedEventArgs.room.instantiatedRoom;
        frontTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front").GetComponent<Tilemap>();
        grid = instantiatedRoom.transform.GetComponentInChildren<Grid>();
        startGridPosition = noValue;
        endGridPosition = noValue;

        SetUpPathTilemap();
    }

    /// <summary>
    /// Use a clone of the Front Tilemap for the path tilemap. If not created then creat one, else use the existing one.
    /// </summary>
    private void SetUpPathTilemap()
    {
        Transform tilemapCloneTransform = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)");

        //If the front tilemap hasn't been cloned then clone it
        if(tilemapCloneTransform == null)
        {
            pathTilemap = Instantiate(frontTilemap, grid.transform);
            pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
            pathTilemap.gameObject.tag = "Untagged";
        }
        //else use it
        else
        {
            pathTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)").GetComponent<Tilemap>();
            pathTilemap.ClearAllTiles();
        }
    }

    private void Update()
    {
        if (instantiatedRoom == null || startPathTile == null || finishPathTile == null || grid == null || pathTilemap == null)
            return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            ClearPath();
            SetStartPosition();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();
            SetEndPosition();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPath();
        }
    }

    private void ClearPath()
    {
        if (pathStack == null) return;

        foreach(Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), null);
        }

        pathStack = null;

        //Clear start and finish squares
        endGridPosition = noValue;
        startGridPosition = noValue;
    }

    private void SetStartPosition()
    {
        if (startGridPosition == noValue)
        {
            startGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            if (!IsPositionWithinBounds(startGridPosition))
            {
                startGridPosition = noValue;
                return;
            }

            pathTilemap.SetTile(startGridPosition, startPathTile);
        }
        else
        {
            pathTilemap.SetTile(startGridPosition, null);
            startGridPosition = noValue;
        }
    }

    private void SetEndPosition()
    {
        if (endGridPosition == noValue)
        {
            endGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            if (!IsPositionWithinBounds(endGridPosition))
            {
                endGridPosition = noValue;
                return;
            }

            pathTilemap.SetTile(endGridPosition, finishPathTile);
        }
        else
        {
            pathTilemap.SetTile(endGridPosition, null);
            endGridPosition = noValue;
        }
    }

    private bool IsPositionWithinBounds(Vector3Int position)
    {
        if (position.x < instantiatedRoom.room.templateLowerBounds.x || position.x > instantiatedRoom.room.templateUpperBounds.x ||
            position.y < instantiatedRoom.room.templateLowerBounds.y || position.y > instantiatedRoom.room.templateUpperBounds.y)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void DisplayPath()
    {
        if (startGridPosition == noValue || endGridPosition == noValue) return;

        pathStack = AStar.BuildPath(instantiatedRoom.room, startGridPosition, endGridPosition);

        if (pathStack == null) return;

        foreach(Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), startPathTile);
        }
    }
}

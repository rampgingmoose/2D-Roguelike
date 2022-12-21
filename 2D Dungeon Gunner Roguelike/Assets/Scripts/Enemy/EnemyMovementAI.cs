using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]
    #endregion
    [SerializeField] private MovementDetailsSO movementDetails;
    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    public float moveSpeed;
    private bool chasePlayer = false;
    [HideInInspector] public int updateFrameNumber = 1; //default value. This is set by the EnemySpawner.
    private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        //Create waitforfixed update to use in coroutine
        waitForFixedUpdate = new WaitForFixedUpdate();

        //reset player reference position
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }


    private void MoveEnemy()
    {
        //Movement cooldown Timer
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        //Check distance to player to see if enemy should start chasing
        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition())
            < enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }

        //If not close enough to chase player
        if (!chasePlayer)
            return;

        //Only process A Star path rebuild on certain frames to spread the load between enemies
        if (Time.frameCount % Settings.targetFramRateToSpreadPathfindingOver != updateFrameNumber) return;

        //If the movement cooldown timer reached or player has moved more than required distance
        //then rebuild the enemy path and move the enemy
        if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition,
            GameManager.Instance.GetPlayer().GetPlayerPosition()) > Settings.playerMoveDistanceToRebuildPath))
        {
            //reset path rebuild cooldown timer
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldownTimer;

            //Reset player reference position
            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            //Move the enemy using AStar pathfinding - Trigger rebuild of path to player
            CreatePath();

            //If a path has been found move the enemy
            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                //Move enemy along the path using a coroutine
                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }

    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            //while not very close continue to move - when close move to next step
            while(Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                //Trigger movement event
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, moveSpeed,
                    (nextPosition - transform.position).normalized);

                yield return waitForFixedUpdate; //moving the enemy using 2d physics so until the next fixed update
            }

            yield return waitForFixedUpdate;
        }

        enemy.idleEvent.CallIdleEvent();
    }

    /// <summary>
    /// Use the AStar static class to create a path for the enemy
    /// </summary>
    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        //Get player position on the grid
        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

        //Get the enemy position on the grid
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        //Build a path for the enemy to move on
        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        //Take off first step on path - this is the grid square the enemy is already on
        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
        else
        {
            //Trigger idle event - not path
            enemy.idleEvent.CallIdleEvent();
        }
    }

    /// <summary>
    /// Set the frame number that the enemy path will be recaluculated on - to avoid performance spikes
    /// </summary>
    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }

    /// <summary>
    /// Get the nearest position to the player that isn't on an obstacle (This is because player's are able to occupy a 
    /// square that is also marked obstacle e.g the 'half collision' tilemap squares
    /// </summary>
    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        Vector3Int playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x,
            playerCellPosition.y - currentRoom.templateLowerBounds.y);

        int obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y],
            currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y]);

        //If the player isn't a cell square marked as an obstacle then return the position
        if (obstacle != 0)
        {
            return playerCellPosition;
        }
        //find a surrounding cell that isn't an obstacle - required because with the 'half collision' tiles
        //and moveable obstacles the player can be on  a grid square that is marked as an obstacle
        else
        {
            //Empty surrounding position list
            surroundingPositionList.Clear();

            //Populate surround position list - this will hold the 8 possible vector locations surrounding a (0,0) grid square
            for(int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) continue;

                    surroundingPositionList.Add(new Vector2Int(i, j));
                }
            }

            //Loop through all positions
            for (int l = 0; l < 8; l++)
            {
                //Generate a random index for the list
                int index = Random.Range(0, surroundingPositionList.Count);

                //See if there is an obstacle i nthe seloected surrounding position
                try
                {
                    obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x +
                        surroundingPositionList[index].x, adjustedPlayerCellPosition.y + surroundingPositionList[index].y],
                        currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPosition.x + surroundingPositionList[index].x,
                        adjustedPlayerCellPosition.y + surroundingPositionList[index].y]);
                }
                //Catch errors where the surround position is outside the grid
                catch
                {

                }

                //Remove the surrounding position with the obstacle so we can try again
                surroundingPositionList.RemoveAt(index);
            }

            //If no non-obstacle cells found surrounding the play - send the enemy in the direction of an enemy spawn position
            return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];
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

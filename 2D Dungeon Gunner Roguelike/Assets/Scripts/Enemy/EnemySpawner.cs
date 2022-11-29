using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonoBehavior<EnemySpawner>
{
    private int enemiesToSpawn;
    private int currentEnemyCount;
    private int enemiesSpawnedSoFar;
    private int enemyMaxConcurrentSpawnNumber;
    private Room currentRoom;
    private RoomEnemySpawnParameters roomEnemySpawnParameters;

    private void OnEnable()
    {
        StaticEventsHandler.OnRoomChanged += StaticEventsHandler_OnRoomChanged;
        
    }

    private void OnDisable()
    {
        StaticEventsHandler.OnRoomChanged -= StaticEventsHandler_OnRoomChanged;
    }

    private void StaticEventsHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        //If the room is a corridor or the entrance then return
        if(currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
        {
            return;
        }

        //If the room has already been cleared
        if (currentRoom.isClearedofEnemies)
            return;

        //Get random number of enemies to spawn
        enemiesToSpawn = currentRoom.GetNumberOfEnemiestoSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        //Get room enemy spawn parameters
        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

        if (enemiesToSpawn == 0)
        {
            //Mark the room as cleared
            currentRoom.isClearedofEnemies = true;
            return;
        }

        //Get concurrent number of enemies
        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        //Lock Doors
        currentRoom.instantiatedRoom.LockDoors();

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (GameManager.Instance.gameState == GameState.playingLevel)
        {
            GameManager.Instance.previousGameState = GameState.playingLevel;
            GameManager.Instance.gameState = GameState.engagingEnemies;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        Grid grid = currentRoom.instantiatedRoom.grid;

        //Create an instance of the helper class used to select a random enemy
        RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);
        
        //Check we have somewhere to spawn the enemies
        if (currentRoom.spawnPositionArray.Length > 0)
        {
            for(int i = 0; i < enemiesToSpawn; i++)
            {
                //wait until current enemy count is less than max concurrent enemies
                while(currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
                {
                    yield return null;
                }

                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    /// <summary>
    /// Get a random spawn interval between the min and max values
    /// </summary>
    private float GetEnemySpawnInterval()
    {
        return (Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval));
    }

    /// <summary>
    /// Get a random number of concurrent enemies between the min and max values
    /// </summary>
    private int GetConcurrentEnemies()
    {
        return (Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies));
    }

    private void CreateEnemy(EnemyDetailsSO enemyDetailsSO, Vector3 position)
    {
        //keep track of the number of enemies spawned so far
        enemiesSpawnedSoFar++;

        //Add one to the current enemy count - this is reduced when an enemy is detroyed
        currentEnemyCount++;

        DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        //Instantiate enemy
        GameObject enemy = Instantiate(enemyDetailsSO.enemyPrefab, position, Quaternion.identity, transform);

        //Initialize enemy
        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetailsSO, enemiesSpawnedSoFar, dungeonLevel);

        //subscribe to enemy destroyed event
        enemy.GetComponent<DestroyedEvent>().OnDestroy += Enemy_OnDestroyed;
    }

    /// <summary>
    /// Process enemy destroyed
    /// </summary>
    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        //Unsubscribe from the event
        destroyedEvent.OnDestroy -= Enemy_OnDestroyed;

        //reduce current enemy count
        currentEnemyCount--;

        if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
        {
            currentRoom.isClearedofEnemies = true;

            //Set gamestate
            if (GameManager.Instance.gameState == GameState.engagingEnemies)
            {
                GameManager.Instance.gameState = GameState.playingLevel;
                GameManager.Instance.previousGameState = GameState.engagingEnemies;
            }
            else if (GameManager.Instance.gameState == GameState.engagingBoss)
            {
                GameManager.Instance.gameState = GameState.bossStage;
                GameManager.Instance.gameState = GameState.engagingBoss;
            }

            //unlock doors
            currentRoom.instantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);

            //Trigger room enemies defeated event
            StaticEventsHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
        }
    }
}

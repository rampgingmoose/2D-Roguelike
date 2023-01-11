using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    private List<SpawnableObjectByLevel<EnemyDetailsSO>> testLevelSpawnList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass;
    private List<GameObject> instantiatedEnemyList = new List<GameObject>();

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
        //Destroy any spawned enemies
        if (instantiatedEnemyList != null && instantiatedEnemyList.Count > 0)
        {
            foreach (GameObject enemy in instantiatedEnemyList)
            {
                Destroy(enemy);
            }
        }

        RoomTemplateSO roomTemplateSO = DungeonBuilder.Instance.GetRoomTemplate(roomChangedEventArgs.room.templateID);

        if (roomTemplateSO != null)
        {
            testLevelSpawnList = roomTemplateSO.enemiesByLevelList;

            //Create RandomSpawnableObject helper class
            randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            EnemyDetailsSO enemyDetailsSO = randomEnemyHelperClass.GetItem();
            
            if(enemyDetailsSO != null)
            {
                instantiatedEnemyList.Add (Instantiate(enemyDetailsSO.enemyPrefab, 
                    HelperUtilities.GetSpawnPositionNearestToPlayer(HelperUtilities.GetMouseWorldPosition()), Quaternion.identity));
            }
        }
    }
}

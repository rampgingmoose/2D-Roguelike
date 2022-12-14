using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region UNITS
    public const float pixelsPerUnit = 16f;
    public const float tilesSizePixels = 16f;
    #endregion

    #region DUNGEON BUILD SETTINGS
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion

    #region ROOM SETTINGS

    //Max number of child corridors leading from a room. - maximum should be 3 although this is
    //not recommended since it can cause the dungeon building to fail since the rooms are more likely to not fit together.
    public const int maxChildCorridors = 3;

    //time to fade in room
    public const float fadeInTime = 0.5f;

    public const float doorUnlockDelay = 1f;

    #endregion

    #region ANIMATOR PARAMETERS
    //Animator parameters - Player
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static float baseSpeedForPlayerAnimations = 8f;

    //Animator parameters for enemy
    public static float baseSpeedForEnemyAnimations = 3f;

    //Animator parameters - door
    public static int open = Animator.StringToHash("open");
    #endregion

    //Animator parameters - Destroyable Environment Objects
    public static int destroy = Animator.StringToHash("destroy");
    public static string stateDestroyed = "Destroyed";

    #region GAMEOBJECT TAGS
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    #endregion

    #region FIRING CONTROL
    //If the target distance is less than this then the aim angle will be used (calculated from player),
    //else the weapon aim angle will be used (calculated from the weapon pivot point)
    public const float useAimAngleDistance = 3.5f;
    #endregion

    #region ASTAR PATHFINDING PARAMETERS
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldownTimer = 2f;
    public const int targetFramRateToSpreadPathfindingOver = 60;
    #endregion

    #region ENEMY HEALTH PARAMETERS
    public const int defaultEnemyHealth = 20;
    #endregion

    #region UI Parameter
    public const float uiAmmoIconSpacing = 4f;
    public const float uiHeartSpacing = 16f;
    #endregion

    #region CONTACT DAMAGE PARAMETERS
    public const float contactDamageCollisionResetDelay = 0.5f;
    #endregion
}

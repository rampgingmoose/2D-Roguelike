using System.Collections;
using UnityEngine;

public static class HelperUtilities 
{
    public static Camera mainCamera;
    //<summary>
    //Get the mouse world position
    //</summary>
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        //Clamp mouse position to screen size
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }

    /// <summary>
    /// Get the camera viewport lower and upper bounds
    /// </summary>
    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLowerBounds, out Vector2Int cameraWorldPositionUpperBounds,
        Camera camera)
    {
        Vector3 worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewportUpperRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        cameraWorldPositionLowerBounds = new Vector2Int((int)worldPositionViewportBottomLeft.x, (int)worldPositionViewportBottomLeft.y);
        cameraWorldPositionUpperBounds = new Vector2Int((int)worldPositionViewportUpperRight.x, (int)worldPositionViewportUpperRight.y);
    }

    //<summary>
    //Get the angle in degrees from a direction vector
    //<summary>
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float angleInDegrees = radians * Mathf.Rad2Deg;

        return angleInDegrees;
    }

    /// <summary>
    /// Get the direction vector from an angle in degrees
    /// </summary>
    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection;

        //UpRight
        if (angleDegrees >= 22f && angleDegrees <= 67f)
        {
            aimDirection = AimDirection.UpRight;
        }
        //Up
        else if (angleDegrees > 67f && angleDegrees <= 112f)
        {
            aimDirection = AimDirection.Up;
        }
        //UpLeft
        else if (angleDegrees > 112f && angleDegrees <= 158f)
        {
            aimDirection = AimDirection.UpLeft;
        }
        //Left
        else if ((angleDegrees <= 180f && angleDegrees > 158f) || (angleDegrees > -180f && angleDegrees <= -135f))
        {
            aimDirection = AimDirection.Left;
        }
        //Down
        else if (angleDegrees > -135f && angleDegrees <= -45f)
        {
            aimDirection = AimDirection.Down;
        }
        //Right
        else if ((angleDegrees > -45f && angleDegrees <= 0f) || (angleDegrees > 0 && angleDegrees < 22f))
        {
            aimDirection = AimDirection.Right;
        }
        else
        {
            aimDirection = AimDirection.Right;
        }

        return aimDirection;
    }

    public static float LinearToDecibels(int linear)
    {
        float linearScaleRange = 20f;

        //formula to convert from the linear scale to logarithmic decibel scale
        //The '20' value is an adjustment fact when converting a linear value to a logarithmic value and the decibel scale is an
        //amptitude scale
        return Mathf.Log10((float)linear / linearScaleRange) * 20f;
    }

    //<summary>
    // Empty string Debug check
    //<summary>
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + "is empty and must contain a value in object" + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    public static bool ValidateCheckNullValue(Object thisObject, string fieldName, Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fieldName + " is null and must contain a value in object" + thisObject.name.ToString());
        }

        return false;
    }

    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + "is null in object" + thisObject.name.ToString());
            return true;
        }

        foreach(var item in enumerableObjectToCheck)
        {
            if(item == null)
            {
                Debug.Log(fieldName + " has null values in object " + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }           
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " has no values in object " + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    //<summary>
    //positive value debug check - if zero is allowed set isZeroAllowed to true. Returns true if there is an error
    //<summary>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive vale or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    //<summary>
    //positive range debug check - set isZerAllowed to true if the min and max range values can be both zero. Return true if there is an error
    //<summary>
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float valueToCheckMinimum, string fieldNameMaximum,
        float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;

        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to " + fieldNameMaximum + " in object " + thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;

        return error;
    }

    //<summary>
    //positive range debug check - set isZerAllowed to true if the min and max range values can be both zero. Return true if there is an error
    //<summary>
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, int valueToCheckMinimum, string fieldNameMaximum,
        int valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;

        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to " + fieldNameMaximum + " in object " + thisObject.name.ToString());
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;

        return error;
    }

    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(10000f, 10000f, 0f);

        //Loop through room spawn positions
        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            //Convert the spawn grid positions to world positions
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if (Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                nearestSpawnPosition = spawnPositionWorld;
            }
        }

        return nearestSpawnPosition;
    }
}

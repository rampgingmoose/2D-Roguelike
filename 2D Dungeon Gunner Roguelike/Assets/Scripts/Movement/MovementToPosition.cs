using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {
        //subscribe to movement to position event
        movementToPositionEvent.OnMovementToPosition += MovementToPosition_OnMovementToPosition;
    }

    private void OnDisable()
    {
        //unsubscribe to movement to position event
        movementToPositionEvent.OnMovementToPosition += MovementToPosition_OnMovementToPosition;
    }

    private void MovementToPosition_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        MoveRigidBody(movementToPositionArgs.movePosition, movementToPositionArgs.currentPosition, movementToPositionArgs.moveSpeed);
    }

    private void MoveRigidBody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed)
    {
        Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);

        rigidBody.MovePosition(rigidBody.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
    }
}

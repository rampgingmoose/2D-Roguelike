using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private MovementByVelocityEvent movementByVelocityEvent;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        //Subscribe to movement event
        movementByVelocityEvent.OnMovementByVelocity += MovementByVelocity_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        //Unsubscribe to movement event
        movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocity_OnMovementByVelocity;
    }

    private void MovementByVelocity_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
    {
        MoveRigidBody(movementByVelocityArgs.moveDirection, movementByVelocityArgs.moveSpeed);
    }

    //<summary>
    //move the rigidbody
    //<summary>
    private void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
    {
        //ensure the rigidbody collision is set to continuous
        rigidBody.velocity = moveDirection * moveSpeed;
    }
}

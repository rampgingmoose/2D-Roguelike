using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private IdleEvent idleEvent;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisbale()
    {
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidBody();
    }

    //<summary>
    //Move the rigidBody component
    //</summary>
    private void MoveRigidBody()
    {
        //ensure the rb collision detection is set to continuous
        rigidBody.velocity = Vector2.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("Object References")]
    #endregion

    #region
    [Tooltip("Populate this with the BoxCollider2D component on the DoorCollider gameobject")]
    #endregion
    [SerializeField] private BoxCollider2D doorCollider;

    public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator animator;

    private void Awake()
    {
        //disable door collider by default
        doorCollider.enabled = false;

        //Load components
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
        {
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        //When the parent GameObject is disabled (when the player moves far enough away from the room)
        //the animator state gets reset. Therefore we need to restore the animator state.
        animator.SetBool(Settings.open, isOpen);
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            animator.SetBool(Settings.open, true);
        }
    }

    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        //Set open to false to close door
        animator.SetBool(Settings.open, false);
    }

    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if(previouslyOpened == true)
        {
            isOpen = false;
            OpenDoor();
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
    #endregion
}

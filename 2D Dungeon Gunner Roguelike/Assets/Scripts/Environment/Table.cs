using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Table : MonoBehaviour, IUseable
{
    #region Tooltip
    [Tooltip("The mass of the table to ocntrol the speed that it moves when pushed")]
    #endregion
    [SerializeField] private float itemMass;
    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private bool itemUsed = false;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void UseItem()
    {
        if (!itemUsed)
        {
            //Get item collider bounds
            Bounds bounds = boxCollider2D.bounds;

            //Calculate closest point to player on collider bounds
            Vector3 closestPointToPlayer = bounds.ClosestPoint(GameManager.Instance.GetPlayer().GetPlayerPosition());

            //If player is to the right of the table then flip left
            if (closestPointToPlayer.x == bounds.max.x)
            {
                animator.SetBool(Settings.flipLeft, true);
            }
            //If player is to the left of the table then flip right
            else if (closestPointToPlayer.x == bounds.min.x)
            {
                animator.SetBool(Settings.flipRight, true);
            }
            //If player is above the table then flip the table down
            else if (closestPointToPlayer.y == bounds.min.y)
            {
                animator.SetBool(Settings.flipUp, true);
            }
            //if none of the above is true then the player must be below the table so flip the table up
            else 
            {
                animator.SetBool(Settings.flipDown, true);
            }

            //Set the layer to environment - bullets will now collide with the table
            gameObject.layer = LayerMask.NameToLayer("Environment");

            //Set the mass of the objects to the specified amount so that the player can move the item
            rigidBody2D.mass = itemMass;

            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.tableFlipSoundEffect);

            itemUsed = true;
        }
    }

    #region
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(itemMass), itemMass, false);
    }
#endif
    #endregion
}

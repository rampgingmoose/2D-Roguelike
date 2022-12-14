using System.Collections;
using UnityEngine;

//Don't add require directives ass we're going to destroy the components when the item is destroyed
public class DestroyableItem : MonoBehaviour
{
    #region Header HEALTH
    [Header("HEALTH")]
    #endregion
    #region Tooltip
    [Tooltip("What the starting health for this destroyable item should be")]
    #endregion
    [SerializeField] private int startingHealthAmount = 1;
    #region Tooltip
    [Tooltip("The sound effect when this item is destroyed")]
    #endregion
    [SerializeField] private SoundEffectSO destroySoundEffect;
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private HealthEvent healthEvent;
    private Health health;
    private RecieveContactDamage recieveContactDamage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        health.SetStartingHealth(startingHealthAmount);
        recieveContactDamage = GetComponent<RecieveContactDamage>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }

    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0f)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        //Destroy trigger collider
        Destroy(boxCollider2D);

        //Play sound effect
        if (destroySoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(destroySoundEffect);
        }

        //Trigger the destroy animation
        animator.SetBool(Settings.destroy, true);

        // Let the animation play through
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.stateDestroyed))
        {
            yield return null;
        }

        //Destroy all components other than the Sprite Renderer to just display the final sprite in the animation
        Destroy(animator);
        Destroy(recieveContactDamage);
        Destroy(health);
        Destroy(healthEvent);
        Destroy(this);
    }
}

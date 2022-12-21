using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startingHealth;
    public int currentHealth;
    private HealthEvent healthEvent;
    public Player player;
    public Enemy enemy;

    private Coroutine immunityCoroutine;
    private bool isImmuneAfterHit = false;
    private float immunityTime = 0f;
    private SpriteRenderer spriteRenderer = null;
    private const float spriteFlashInterval = 0.2f;
    private WaitForSeconds WaitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);

    public bool isDamageable = true;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        //Trigger a health event for UI update
        CallHealthEvent(0);

        //Attempt to load enemy / player components
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>(); 

        //Get hit immunity details
        if (player != null)
        {
            if (player.playerDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = player.playerDetails.hitImmunityTime;
                spriteRenderer = player.spriteRenderer;
            }
        }
        else if (enemy != null)
        {
            if (enemy.enemyDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = enemy.enemyDetails.hitImmunityTime;
                spriteRenderer = enemy.spriteRendererArray[0];
            }
        }
    }

    private void CallHealthEvent(int damageAmount)
    {
        //Trigger health event
        healthEvent.CallHealthChangedEvent(((float)currentHealth / (float)startingHealth), currentHealth, damageAmount);
    }

    //public method called when damage is taken
    public void TakeDamage(int damageAmount)
    {
        bool isRolling = false;

        if (player != null)
        {
            isRolling = player.playerControl.isPlayerRolling;
        }

        if (isDamageable && !isRolling)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);

            PostHitImmunity();

            //Set health bar as the percentage of health remaining
            if (enemy != null && GameManager.Instance.bossHealthBarUI != null && enemy.enemyDetails.displayBossHealthBar == true)
            {
                GameManager.Instance.bossHealthBarUI.InitializeBossHealthBar((float)currentHealth / (float)startingHealth);
            }
        }
    }

    private void PostHitImmunity()
    {
        //Check if gameobject is active - if not return
        if (gameObject.activeSelf == false)
            return;

        //If there is post hit immunity
        if (isImmuneAfterHit)
        {
            if (immunityCoroutine != null)
            {
                StopCoroutine(immunityCoroutine);
            }

            //flash red and give period of immunity
            immunityCoroutine = StartCoroutine(PostHitImmunityCoroutine(immunityTime, spriteRenderer));
        }
    }

    private IEnumerator PostHitImmunityCoroutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);

        isDamageable = false;

        while(iterations > 0)
        {
            spriteRenderer.color = Color.red;

            yield return WaitForSecondsSpriteFlashInterval;

            spriteRenderer.color = Color.white;

            yield return WaitForSecondsSpriteFlashInterval;

            iterations--;

            yield return null;
        }

        isDamageable = true;
    }

    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    public int GetStartingHealth()
    {
        return startingHealth;
    }

    /// <summary>
    /// Increase health by specified percent
    /// </summary>
    public void AddHealth(int healthPercent)
    {
        int healthIncrease = Mathf.RoundToInt((startingHealth * healthPercent) / 100f);

        int totalHealth = currentHealth + healthIncrease;

        if (totalHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }
        else
        {
            currentHealth = totalHealth;
        }

        CallHealthEvent(0);
    }
}

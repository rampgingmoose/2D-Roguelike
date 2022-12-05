using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBarUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Header("OBJECT REFERENCES")]
    #endregion

    #region Tooltip
    [Tooltip("Populate with Boss HealthBar Slider")]
    #endregion
    [SerializeField] private Slider healthBarSlider;

    #region Tooltip
    [Tooltip("Populate with Boss Name TMP Component")]
    #endregion
    [SerializeField] private TextMeshProUGUI bossName;
    private Enemy enemy;

    private void Awake()
    {
        enemy = GameManager.Instance.GetComponent<Enemy>();

        healthBarSlider = GetComponentInChildren<Slider>();
        bossName = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        if(enemy != null && enemy.enemyDetails.displayBossHealthBar == true)
        {
            bossName.text = enemy.enemyDetails.name;
        }
    }

    public void EnableBossHealthBar()
    {
        gameObject.SetActive(true);
        healthBarSlider.value = 1f;
    }

    public void DisableBossHealthBar()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Set health bar value with health percent between 0 and 1
    /// </summary>
    public void InitializeBossHealthBar(float healthPercentage)
    {
        healthBarSlider.value = healthPercentage;
    }
}

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HeathUI : MonoBehaviour
{
    private List<GameObject> healthHeartList = new List<GameObject>();

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        SetHealthBar(healthEventArgs);
    }

    private void ClearHealthBar()
    {
        foreach(GameObject heartIcon in healthHeartList)
        {
            Destroy(heartIcon);
        }

        healthHeartList.Clear();
    }

    private void SetHealthBar(HealthEventArgs healthEventArgs)
    {
        ClearHealthBar();

        //Instantiate heart image prefabs
        int healthHearts = Mathf.CeilToInt(healthEventArgs.healhPercent * 100f / 20f);

        for (int i = 0; i < healthHearts; i++)
        {
            //Instantiate Heart prefabs
            GameObject heart = Instantiate(GameResources.Instance.heartImage, transform);

            //Position
            heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiHeartSpacing * i, 0f);

            healthHeartList.Add(heart);

            if (GameManager.Instance.GetPlayer().health.currentHealth % 10 != 0)
            {
                healthHeartList[i - 1] = Instantiate(GameResources.Instance.halfHeartImage, transform);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent]
public class LightFlicker : MonoBehaviour
{
    private Light2D light2D;
    [SerializeField] private float lightIntensityMinimum;
    [SerializeField] private float lightIntensityMaximum;
    [SerializeField] private float lightFlickerTimeMin;
    [SerializeField] private float lightFlickerTimeMax;
    private float lightFlickerTimer;

    private void Awake()
    {
        light2D = GetComponentInChildren<Light2D>();
    }

    private void Start()
    {
        lightFlickerTimer = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax);
    }

    private void Update()
    {
        if (light2D == null)
            return;

        lightFlickerTimer -= Time.deltaTime;

        if (lightFlickerTimer <= 0f)
        {
            lightFlickerTimer = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax);

            RandomizeLightIntensity();
        }
    }

    private void RandomizeLightIntensity()
    {
        light2D.intensity = Random.Range(lightIntensityMinimum, lightIntensityMaximum);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(lightIntensityMinimum), lightIntensityMinimum, 
            nameof(lightIntensityMaximum), lightIntensityMaximum, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(lightFlickerTimeMin), lightFlickerTimeMin, 
            nameof(lightFlickerTimeMax), lightFlickerTimeMax, false);
    }
#endif
    #endregion
}

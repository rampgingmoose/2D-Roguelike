using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem shootEffectParticleSystem;

    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;

    private void Awake()
    {
        shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Set the Shoot Effect from the WeaponShootEffectSO and aimAngle
    /// </summary>
    public void SetShootEffect(WeaponShootEffectSO weaponShootEffectSO, float aimAngle)
    {
        SetShootEffectColorGradient(weaponShootEffectSO.colorGradient);

        SetShootEffectParticleStartingValues(weaponShootEffectSO.particleDuration, weaponShootEffectSO.startParticleSize,
            weaponShootEffectSO.startParticleSpeed, weaponShootEffectSO.startParticletLifetime, weaponShootEffectSO.particleGravity,
            weaponShootEffectSO.maxParticleNumber);

        SetShootEffectParticleEmission(weaponShootEffectSO.emissionRate, weaponShootEffectSO.burstParticleNumber);

        SetEmitterRotation(aimAngle);

        SetShootEffectParticleSprite(weaponShootEffectSO.particleSprite);

        SetShootEffectVelocityOverLifetime(weaponShootEffectSO.velocityOverLifetimeMin, weaponShootEffectSO.velocityOverLifetimeMax);
    }

    /// <summary>
    /// Set the shoot effect particle system color gradient
    /// </summary>
    private void SetShootEffectColorGradient(Gradient gradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// <summary>
    /// Set shoot effect particle system starting values
    /// </summary>
    private void SetShootEffectParticleStartingValues(float particleDuration, float startParticleSize,
            float startParticleSpeed, float startParticletLifetime, float particleGravity,int maxParticleNumber)
    {
        ParticleSystem.MainModule mainModule = shootEffectParticleSystem.main;

        mainModule.duration = particleDuration;
        mainModule.startSize = startParticleSize;
        mainModule.startSpeed = startParticleSpeed;
        mainModule.startLifetime = startParticletLifetime;
        mainModule.gravityModifier = particleGravity;
        mainModule.maxParticles = maxParticleNumber;
    }

    /// <summary>
    /// Set shoot effect particle system particl burst particle number
    /// </summary>
    private void SetShootEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = shootEffectParticleSystem.emission;

        //Set particle Burst number
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        //Set particle emission rate
        emissionModule.rateOverTime = emissionRate;
    }

    private void SetShootEffectParticleSprite(Sprite particleSprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = shootEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, particleSprite);
    }

    private void SetEmitterRotation(float aimAngle)
    {
        

        fireDirectionAngle = aimAngle;

        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    private void SetShootEffectVelocityOverLifetime(Vector3 velocityOverLifetimeMin, Vector3 velocityOverLifetimeMax)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = shootEffectParticleSystem.velocityOverLifetime;

        //Define min and max X velocity
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = velocityOverLifetimeMin.x;
        minMaxCurveX.constantMax = velocityOverLifetimeMax.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        //Define min and max Y velocity
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = velocityOverLifetimeMin.y;
        minMaxCurveY.constantMax = velocityOverLifetimeMax.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        //Defnie min and max Z velocity
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = velocityOverLifetimeMin.z;
        minMaxCurveZ.constantMax = velocityOverLifetimeMax.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }
}

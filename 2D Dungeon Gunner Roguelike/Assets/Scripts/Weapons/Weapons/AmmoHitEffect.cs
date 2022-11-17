using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Set the Shoot Effect from the WeaponShootEffectSO and aimAngle
    /// </summary>
    public void SetHitEffect(AmmoHitEffectSO ammoHitEffectSO)
    {
        SetHitEffectColorGradient(ammoHitEffectSO.colorGradient);

        SetHitEffectParticleStartingValues(ammoHitEffectSO.particleDuration, ammoHitEffectSO.startParticleSize,
            ammoHitEffectSO.startParticleSpeed, ammoHitEffectSO.startParticletLifetime, ammoHitEffectSO.particleGravity,
            ammoHitEffectSO.maxParticleNumber);

        SetHitEffectParticleEmission(ammoHitEffectSO.emissionRate, ammoHitEffectSO.burstParticleNumber);

        SetHitEffectParticleSprite(ammoHitEffectSO.particleSprite);

        SetHitEffectVelocityOverLifetime(ammoHitEffectSO.velocityOverLifetimeMin, ammoHitEffectSO.velocityOverLifetimeMax);
    }

    /// <summary>
    /// Set the shoot effect particle system color gradient
    /// </summary>
    private void SetHitEffectColorGradient(Gradient gradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// <summary>
    /// Set shoot effect particle system starting values
    /// </summary>
    private void SetHitEffectParticleStartingValues(float particleDuration, float startParticleSize,
            float startParticleSpeed, float startParticletLifetime, float particleGravity, int maxParticleNumber)
    {
        ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;

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
    private void SetHitEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticleSystem.emission;

        //Set particle Burst number
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        //Set particle emission rate
        emissionModule.rateOverTime = emissionRate;
    }

    private void SetHitEffectParticleSprite(Sprite particleSprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, particleSprite);
    }

    private void SetHitEffectVelocityOverLifetime(Vector3 velocityOverLifetimeMin, Vector3 velocityOverLifetimeMax)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

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

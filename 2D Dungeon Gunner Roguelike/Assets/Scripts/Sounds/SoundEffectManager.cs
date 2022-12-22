using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonoBehavior<SoundEffectManager>
{
    public int soundsVolume = 8;

    private void Start()
    {
        //Check if sound effects volume has been saved in Player Prefs - if so retrieve and set them
        if (PlayerPrefs.HasKey("soundsVolume"))
        {
            soundsVolume = PlayerPrefs.GetInt("soundsVolume");
        }

        SetSoundVolume(soundsVolume);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt("soundsVolume", soundsVolume);
    }

    public void PlaySoundEffect(SoundEffectSO soundEffectSO)
    {
        //Play sound using a sound gameobject and component from the object pool
        SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffectSO.soundEffectPrefab, Vector3.zero,
            Quaternion.identity);
        sound.SetSound(soundEffectSO);
        sound.gameObject.SetActive(true);

        StartCoroutine(DisableSound(sound, soundEffectSO.soundEffectClip.length));
    }

    private IEnumerator DisableSound(SoundEffect sound, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        sound.gameObject.SetActive(false);
    }

    public void IncreaseSoundVolume()
    {
        int maxSoundsVolume = 10;

        if (soundsVolume >= maxSoundsVolume) return;

        soundsVolume++;

        SetSoundVolume(soundsVolume);
    }

    public void DecreaseSoundVolume()
    {
        if (soundsVolume == 0) return;

        soundsVolume--;

        SetSoundVolume(soundsVolume);
    }

    private void SetSoundVolume(int soundsVolume)
    {
        float muteDecibels = -80f;

        if (soundsVolume == 0)
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", 
                HelperUtilities.LinearToDecibels(soundsVolume));
        }
    }
}

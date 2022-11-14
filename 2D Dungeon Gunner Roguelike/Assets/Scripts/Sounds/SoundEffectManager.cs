using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonoBehavior<SoundEffectManager>
{
    public int soundsVolume = 8;

    private void Start()
    {
        SetSoundVolume(soundsVolume);
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

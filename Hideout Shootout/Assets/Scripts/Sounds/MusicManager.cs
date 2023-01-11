using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MusicManager : SingletonMonoBehavior<MusicManager>
{
    private AudioSource musicAudioSource = null;
    private AudioClip currentAudioClip = null;
    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInMusicCoroutine;
    public int musicVolume = 10;

    protected override void Awake()
    {
        base.Awake();

        musicAudioSource = GetComponent<AudioSource>();

        //Start with music off
        GameResources.Instance.musicOffSnapshot.TransitionTo(0f);
    }

    private void Start()
    {
        //Check if volume levels have been saved in player prefs - if so retrieve and set them
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume = PlayerPrefs.GetInt("musicVolume");
        }

        SetMusicVolume(musicVolume);
    }

    private void OnDisable()
    {
        //Save volume settings in playerprefs
        PlayerPrefs.SetInt("musicVolume", musicVolume);
    }

    public void PlayMusic(MusicTrackSO musicTrackSO, float fadeOutTime = Settings.musicFadeOutTime, float fadeInTime = Settings.musicFadeInTime)
    {
        //Play music track
        StartCoroutine(PlayMusicRoutine(musicTrackSO, fadeOutTime, fadeInTime));
    }

    /// <summary>
    /// Play music for room routine
    /// </summary>
    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrackSO, float fadeOutTime, float fadeInTime)
    {
        //if fade out routine already running then stop it
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }

        //if fade in routine already running then stop it
        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        if (musicTrackSO.musicClip != currentAudioClip)
        {
            currentAudioClip = musicTrackSO.musicClip;

            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusicRoutine(fadeOutTime));

            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusicRoutine(musicTrackSO, fadeInTime));
        }

        yield return null;
    }

    private IEnumerator FadeOutMusicRoutine(float fadeOutTime)
    {
        GameResources.Instance.musicLowOnSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }

    private IEnumerator FadeInMusicRoutine(MusicTrackSO musicTrackSO, float fadeInTime)
    {
        //Set clip & play
        musicAudioSource.clip = musicTrackSO.musicClip;
        musicAudioSource.volume = musicTrackSO.musicVolume;
        musicAudioSource.Play();

        GameResources.Instance.musicFullOnSnapshot.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }

    public void IncreaseMusicVolume()
    {
        int maxMusicVolume = 10;

        if (musicVolume >= maxMusicVolume) return;

        musicVolume++;

        SetMusicVolume(musicVolume);
    }

    public void DecreaseMusicVolume()
    {
        if (musicVolume == 0) return;

        musicVolume--;

        SetMusicVolume(musicVolume);
    }

    private void SetMusicVolume(int musicVolume)
    {
        float muteDecibels = -80f;

        if (GameResources.Instance.musicMasterMixerGroup != null)
        {
            if (musicVolume == 0)
            {
                GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", muteDecibels);
            }
            else
            {
                GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", HelperUtilities.LinearToDecibels(musicVolume));
            }
        }       
    }
}

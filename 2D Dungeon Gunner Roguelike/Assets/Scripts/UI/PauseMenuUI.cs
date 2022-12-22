using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with the music volume level")]
    #endregion
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    #region Tooltip
    [Tooltip("Populate with the sound effects volume level")]
    #endregion
    [SerializeField] private TextMeshProUGUI soundVolumeText;

    private void Start()
    {
        //Initially hide the game object
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Initialize the UI Text
    /// </summary>
    private IEnumerator InitializeUI()
    {
        //Wait a frame to ensure the previous music and sounds levels have been set
        yield return null;

        //Initialize the UI Text
        soundVolumeText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
        musicVolumeText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
        StartCoroutine(InitializeUI());
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Quit game and load main menu - linked to pause menu UI button
    /// </summary>
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary>
    /// Increase music volume - linked to music volume increase button in UI
    /// </summary>
    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        musicVolumeText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    /// <summary>
    /// Decrease music volume - linked to music volume decrease button in UI
    /// </summary>
    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseMusicVolume();
        musicVolumeText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    /// <summary>
    /// Increase the sound effect volume - linked to sound volume increase button in UI
    /// </summary>
    public void IncreaseSoundVolume()
    {
        SoundEffectManager.Instance.IncreaseSoundVolume();
        soundVolumeText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }
    
    /// <summary>
    /// Decrease the sound effect volume - linked to sound volume decrease button in UI
    /// </summary>
    public void DecreaseSoundVolume()
    {
        SoundEffectManager.Instance.DecreaseSoundVolume();
        soundVolumeText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicVolumeText), musicVolumeText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundVolumeText), soundVolumeText);
    }
#endif
    #endregion
}

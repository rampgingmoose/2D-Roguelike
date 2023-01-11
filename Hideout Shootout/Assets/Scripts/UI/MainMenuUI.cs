using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the enter the hideout button gameobject")]
    #endregion
    [SerializeField] private GameObject playButton;
    #region Tooltip
    [Tooltip("Populate with the quit button gameObject")]
    #endregion
    [SerializeField] private GameObject quitButton;
    #region Tooltip
    [Tooltip("Populate with the controls button gameObject")]
    #endregion
    [SerializeField] private GameObject controlsButton;
    #region Tooltip
    [Tooltip("Populate with the high scores button gameobject")]
    #endregion 
    [SerializeField] private GameObject highScoreButton;
    #region Tooltip
    [Tooltip("Populate with the return to main menu button gameobject")]
    #endregion
    [SerializeField] private GameObject returnToMainMenuButton;

    private bool isHighScoresSceneLoaded = false;
    private bool isControlsSceneLoaded = false;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);

        //Load Character Selector Scene additively
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);

        returnToMainMenuButton.SetActive(false);
    }

    /// <summary>
    /// Called from the Play Game / Enter the Dungeon Button
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    /// <summary>
    /// Called from the high scores button
    /// </summary>
    public void LoadHighScores()
    {
        playButton.SetActive(false);
        highScoreButton.SetActive(false);
        quitButton.SetActive(false);
        controlsButton.SetActive(false);
        returnToMainMenuButton.SetActive(true);
        isHighScoresSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        //Load High Score scene additively
        SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// Called from the Return to Main Menu Button
    /// </summary>
    public void LoadMainMenu()
    {
        returnToMainMenuButton.SetActive(false);

        if (isHighScoresSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("HighScoreScene");
            isHighScoresSceneLoaded = false;
        }
        else if (isControlsSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("ControlsScene");
            isControlsSceneLoaded = false;
        }

        playButton.SetActive(true);
        quitButton.SetActive(true);
        highScoreButton.SetActive(true);
        controlsButton.SetActive(true);

        //Load character selector screne additively
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// Called from the Controls Button
    /// </summary>
    public void LoadControls()
    {
        playButton.SetActive(false);
        highScoreButton.SetActive(false);
        quitButton.SetActive(false);
        controlsButton.SetActive(false);
        isControlsSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("ControlsScene", LoadSceneMode.Additive);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playButton), playButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(highScoreButton), highScoreButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(quitButton), quitButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(controlsButton), controlsButton);
    }
#endif
    #endregion
}

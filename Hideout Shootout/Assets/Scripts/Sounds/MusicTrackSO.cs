using UnityEngine;

[CreateAssetMenu(fileName = "Music Track", menuName = "Scriptable Objects/Sounds/Music Track")]
public class MusicTrackSO : ScriptableObject
{
    #region Header MUSIC TRACK DETAILS
    [Space(10)]
    [Header("MUSIC TRACK DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("The name for the music track")]
    #endregion
    public string trackName;

    #region Tooltip
    [Tooltip("The audio Clip for the music track")]
    #endregion
    public AudioClip musicClip;

    #region Tooltip
    [Tooltip("The volume for the music")]
    #endregion
    public float musicVolume = 1f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(trackName), trackName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicClip), musicClip);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(musicVolume), musicVolume, true);
    }
#endif
    #endregion
}

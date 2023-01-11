using UnityEngine;

public class DisplayHighScoresUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the child Content gameObject Transform component")]
    #endregion
    [SerializeField] private Transform contentAnchorTransform;

    private void Start()
    {
        DisplayScores();
    }

    private void DisplayScores()
    {
        HighScore highScore = HighScoreManager.Instance.GetHighScores();
        GameObject scoreGameObject;

        int rank = 0;

        foreach(Score score in highScore.scoreList)
        {
            rank++;

            //Instantiate scores gameObject
            scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);

            ScorePrefab scorePrefab = scoreGameObject.GetComponent<ScorePrefab>();

            scorePrefab.rankTMP.text = rank.ToString();
            scorePrefab.nameTMP.text = score.playerName;
            scorePrefab.levelTMP.text = score.levelDescription;
            scorePrefab.scoreTMP.text = score.playerScore.ToString("###,###0");
        }

        //Add blank line
        //Instantiate score gameobject
        scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);
    }
}

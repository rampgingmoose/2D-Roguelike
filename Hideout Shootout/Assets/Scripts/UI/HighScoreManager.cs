using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class HighScoreManager : SingletonMonoBehavior<HighScoreManager>
{
    private HighScore highScore = new HighScore();


    protected override void Awake()
    {
        base.Awake();

        LoadScores();
    }

    /// <summary>
    /// Load Scores from disk
    /// </summary>
    private void LoadScores()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/HideoutShootoutHighScores.dat"))
        {
            ClearScoreList();

            FileStream file = File.OpenRead(Application.persistentDataPath + "/HideoutShootoutHighScores.dat");

            highScore = (HighScore)bf.Deserialize(file);

            file.Close();
        }
    }

    /// <summary>
    /// Clear all scores
    /// </summary>
    private void ClearScoreList()
    {
        highScore.scoreList.Clear();
    }

    /// <summary>
    /// Add score to high scores list
    /// </summary>
    public void AddScore(Score score, int rank)
    {
        highScore.scoreList.Insert(rank - 1, score);

        if(highScore.scoreList.Count >= Settings.numberOfHighScoresToSave)
        {
            highScore.scoreList.RemoveAt(Settings.numberOfHighScoresToSave);
        }

        SaveScores();
    }

    /// <summary>
    /// Save Scores To Disk
    /// </summary>
    private void SaveScores()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/HideoutShootoutHighScores.dat");

        bf.Serialize(file, highScore);

        file.Close();
    }

    public HighScore GetHighScores()
    {
        return highScore;
    }

    public int GetRank(long playScore)
    {
        //If there are no scores currently in the list - then this score must be ranked 1 - then return
        if (highScore.scoreList.Count == 0) return 1;

        int index = 0;

        //Loop through scores in list to find the rank of this score
        for (int i = 0; i < highScore.scoreList.Count; i++)
        {
            index++;
            
            if(playScore >= highScore.scoreList[i].playerScore)
            {
                return index;
            }
        }

        if (highScore.scoreList.Count < Settings.numberOfHighScoresToSave)
            return (index + 1);

        return 0;
    }
}

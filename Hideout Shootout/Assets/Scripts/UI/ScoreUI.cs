using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI scoreTextTMP;

    private void Awake()
    {
        scoreTextTMP = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StaticEventsHandler.OnScoreChanged += StaticEventsHandler_OnScoreChanged;
    }

    private void OnDisable()
    {
        StaticEventsHandler.OnScoreChanged -= StaticEventsHandler_OnScoreChanged;
    }

    private void StaticEventsHandler_OnScoreChanged(ScoreChangedArgs scoreChangedArgs)
    {
        //Update UI
        scoreTextTMP.text = "SCORE: " + scoreChangedArgs.score.ToString("###,###0") + "\nMULTIPLIER: x" + scoreChangedArgs.multiplier;
    }
}

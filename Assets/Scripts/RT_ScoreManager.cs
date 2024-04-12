using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RT_ScoreManager : MonoBehaviour
{
    private static int score = 0;
    private static int scoreGainDefault = 10;

    [Header("Prototype UI")]
    [SerializeField] private TMP_Text text;

    public static void IncrementScore(int scoreGain)
    {
        if(scoreGain <= 0)
        {
            score += scoreGainDefault; 
        } else
        {
            score += scoreGain;
        }
        Instance.UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        text.text = "Score: " + score;
    }

    private static RT_ScoreManager instance;
    public static RT_ScoreManager Instance
    { 
        get
        {
            instance = FindObjectOfType<RT_ScoreManager>();
            return instance;
        }

    }

}

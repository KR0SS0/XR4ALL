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
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;

    private AudioSource source;
    [SerializeField] private AudioClip scoreClip;

    private bool gotHighscore = false;

    private void Start()
    {
        UpdateScoreUI();
        source = GetComponent<AudioSource>();
        source.clip = scoreClip;
    }

    public void IncrementScore(int scoreGain)
    {
        if(scoreGain <= 0)
        {
            score += scoreGainDefault; 
        } else
        {
            score += scoreGain;
        }
        UpdateScoreUI();

        source.PlayOneShot(source.clip);
    }

    private void UpdateScoreUI()
    {
        if(score >= PlayerPrefs.GetInt("Highscore"))
        {
            PlayerPrefs.SetInt("Highscore", score);
            gotHighscore = true;
        }
        scoreText.text = "Score: " + score;
        highscoreText.text = "Highscore: " + PlayerPrefs.GetInt("Highscore");
    }

    public void ResetScore()
    {
        gotHighscore = false;
        score = 0;
        UpdateScoreUI();
    }

    public bool GotHighscore()
    {
        return gotHighscore;
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

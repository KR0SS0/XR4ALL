using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SD_GameAndScoreManager : MonoBehaviour
{
    private static int score = 0;

    public float gameTime = 60f; // Total game time in seconds
    private float currentTime;

    [Header("Audio")]
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip gameOverHighscoreClip;
    [SerializeField] private AudioClip gameStartClip;
    [SerializeField] private AudioClip scoreClip;
    private AudioSource source;

    [Header("Prototype UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;
    public TMP_Text timerText;

    private bool gotHighscore = false;
    private bool isGameActive = false;

    private Coroutine coroutine;

    private void Start()
    {
        currentTime = gameTime;
        UpdateScoreUI();
        source = GetComponent<AudioSource>();
        source.clip = scoreClip;
    }

    public void IncrementScore()
    {
        Debug.Log("increment score");
        score++;

        UpdateScoreUI();

        source.PlayOneShot(scoreClip);
    }

    private void UpdateScoreUI()
    {
        if (score >= PlayerPrefs.GetInt("SD_Highscore"))
        {
            PlayerPrefs.SetInt("SD_Highscore", score);
            gotHighscore = true;
        }
        scoreText.text = "Score: " + score;
        highscoreText.text = "Highscore: " + PlayerPrefs.GetInt("SD_Highscore");
    }

    private IEnumerator Countdown()
    {
        while (currentTime > 0)
        {
            currentTime -= 1f;
            UpdateTimerDisplay();
            yield return new WaitForSeconds(1f);
        }
        ResetGame();
    }

    private void UpdateTimerDisplay()
    {
        timerText.text = "Time remaining: " + currentTime.ToString("F0"); // "F0" formats the time to display without decimal points
    }

    public void ResetGame()
    {
        if(gotHighscore)
        {
            source.PlayOneShot(gameOverHighscoreClip);
        } else
        {
            source.PlayOneShot(gameOverClip);
        }

        gotHighscore = false;
        isGameActive = false;
        score = 0;
        currentTime = gameTime;

        StopCoroutine(coroutine);

        UpdateScoreUI();
    }

    public void OnButtonPress()
    {
        if(!isGameActive)
        {
            isGameActive = true;
            coroutine = StartCoroutine(Countdown());
        }
    }

    public bool IsGameActive() 
    {  
        return isGameActive; 
    } 


}

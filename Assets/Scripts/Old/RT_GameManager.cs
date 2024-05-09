using System.Collections;
using TMPro;
using UnityEngine;

public class RT_GameManager : MonoBehaviour
{
    public TMP_Text timerText;
    public float gameTime = 60f; // Total game time in seconds
    private float currentTime;

    public RT_RingSpawner ringSpawner;

    private AudioSource source;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip gameOverHighscoreClip;
    [SerializeField] private AudioClip gameStartClip;

    private bool isGameActive = false;

    private Coroutine countdownCoroutine;

    private void Start()
    {
        currentTime = gameTime;
        source = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        // Start the countdown coroutine
        if(isGameActive) return;

        source.PlayOneShot(gameStartClip);
        ringSpawner.Reset();
        countdownCoroutine = StartCoroutine(Countdown());
    }

    public void ResetGame()
    {
        currentTime = gameTime;
        StopCoroutine(countdownCoroutine);
        // Time's up, handle game over logic here
        foreach (RT_Ring ring in FindObjectsOfType<RT_Ring>())
        {
            ring.gameObject.GetComponent<Anim_Scale>().ScaleDownAndDestroy(ring.gameObject);
        }
        ringSpawner.Reset();

        isGameActive = false;

        if (RT_ScoreManager.Instance.GotHighscore())
        {
            source.PlayOneShot(gameOverHighscoreClip);
        } else
        {
            source.PlayOneShot(gameOverClip);
        }

        // Reset score
        RT_ScoreManager.Instance.ResetScore();

        // Reset the timer
        UpdateTimerDisplay();
    }

    private IEnumerator Countdown()
    {
        // Loop until time runs out
        isGameActive = true;

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
        // Update the text to display remaining time
        timerText.text = "Time remaining: " + currentTime.ToString("F0"); // "F0" formats the time to display without decimal points
    }
}

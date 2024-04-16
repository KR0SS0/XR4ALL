using System.Collections;
using TMPro;
using UnityEngine;

public class RT_GameManager : MonoBehaviour
{
    public TMP_Text timerText;
    public float gameTime = 60f; // Total game time in seconds
    private float currentTime;

    private AudioSource source;
    [SerializeField] private AudioClip gameOverClip;

    private void Start()
    {
        currentTime = gameTime;
        source = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        // Start the countdown coroutine
        StartCoroutine(Countdown());
    }

    public void ResetGame()
    {
        // Reset score
        RT_ScoreManager.Instance.ResetScore();

        // Reset the timer
        currentTime = gameTime;
        UpdateTimerDisplay();
    }

    private IEnumerator Countdown()
    {
        // Loop until time runs out
        while (currentTime > 0)
        {
            currentTime -= 1f;
            UpdateTimerDisplay();
            yield return new WaitForSeconds(1f);
        }

        source.PlayOneShot(gameOverClip);

        // Time's up, handle game over logic here
        foreach (RT_Ring ring in FindObjectsOfType<RT_Ring>())
        {
            ring.gameObject.GetComponent<Anim_Scale>().ScaleDownAndDestroy(ring.gameObject);
        }
    }

    private void UpdateTimerDisplay()
    {
        // Update the text to display remaining time
        timerText.text = "Time remaining: " + currentTime.ToString("F0"); // "F0" formats the time to display without decimal points
    }
}

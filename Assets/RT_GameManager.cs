using System.Collections;
using TMPro;
using UnityEngine;

public class RT_GameManager : MonoBehaviour
{
    public TMP_Text timerText;
    public float gameTime = 60f; // Total game time in seconds
    private float currentTime;

    private void Start()
    {
        currentTime = gameTime;
        StartGame();
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
        // Reset ring positions
        RT_Ring[] rings = FindObjectsOfType<RT_Ring>();
        foreach (RT_Ring ring in rings)
        {
            // Reset ring positions here
        }

        // Reset the timer
        currentTime = gameTime;
        UpdateTimerDisplay();
    }

    private IEnumerator Countdown()
    {
        // Loop until time runs out
        while (currentTime > 0)
        {
            // Decrease time by 1 second
            currentTime -= 1f;
            // Update the timer display
            UpdateTimerDisplay();
            // Wait for 1 second
            yield return new WaitForSeconds(1f);
        }
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

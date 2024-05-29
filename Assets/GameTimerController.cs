using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameTimerController : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    private string initialTimerText;
    private float timer;

    private void Start()
    {
        initialTimerText = timerText.text;
    }

    private void Update()
    {
        if(GameManager.Instance == null)
        {
            return;
        }

        if(GameManager.Instance.CurrentState == GameManager.GameState.WaitingToStart ) 
        {
            timer = 0;
            timerText.enabled = false;
        } 
        else if (GameManager.Instance.CurrentState == GameManager.GameState.Active) 
        {
            timer += Time.deltaTime;
            timerText.enabled = true;
        } 
        else if (GameManager.Instance.CurrentState == GameManager.GameState.GameOver)
        {
            //
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt((timer % 3600) / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        int tenths = Mathf.FloorToInt((timer * 10) % 10);

        if (hours > 0)
        {
            timerText.text = initialTimerText + string.Format("{0}h {1}m {2}.{3}s", hours, minutes, seconds, tenths);
        } 
        else if (minutes > 0)
        {
            timerText.text = initialTimerText + string.Format("{0}m {1}.{2}s", minutes, seconds, tenths);
        } 
        else
        {
            timerText.text = initialTimerText + string.Format("{0}.{1}s", seconds, tenths);
        }
    }
}


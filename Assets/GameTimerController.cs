using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimerController : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    private string initialTimerText;
    private float timer;

    private static GameTimerController instance;
    public static GameTimerController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameTimerController>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameTimerController).ToString());
                    instance = singleton.AddComponent<GameTimerController>();
                    DontDestroyOnLoad(singleton);

                    // This ensures that the timerText is not null if we create the singleton at runtime.
                    instance.timerText = singleton.AddComponent<TMP_Text>();
                    instance.timerText.enabled = false;  // Ensure the timerText is initially hidden.
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject.transform.parent);
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        initialTimerText = timerText != null ? timerText.text : "";
    }

    private void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.CurrentState == GameManager.GameState.WaitingToStart)
        {
            timer = 0;
            if (timerText != null)
            {
                timerText.enabled = false;
            }
        } else if (GameManager.Instance.CurrentState == GameManager.GameState.Active)
        {
            timer += Time.deltaTime;
            if (timerText != null)
            {
                timerText.enabled = true;
            }
        } else if (GameManager.Instance.CurrentState == GameManager.GameState.GameOver)
        {
            // Handle GameOver state if necessary
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        timerText.text = initialTimerText + GetTimerFormatText();
    }

    public string GetTimerFormatText()
    {
        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt((timer % 3600) / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        int tenths = Mathf.FloorToInt((timer * 10) % 10);

        if (hours > 0)
        {
            return string.Format("{0}h {1}m {2}.{3}s", hours, minutes, seconds, tenths);
        } 
        else if (minutes > 0)
        {
            return string.Format("{0}m {1}.{2}s", minutes, seconds, tenths);
        } 
        else
        {
            return string.Format("{0}.{1}s", seconds, tenths);
        }
    }

    public float GetTimer()
    {
        return timer;
    }
}

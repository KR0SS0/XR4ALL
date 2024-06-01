using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameObject gameoverScreen;

    [SerializeField] private TMP_Text timeSurvivedText;
    private string initialTimeSurvivedText;

    [SerializeField] private TMP_Text bestTimeSurvivedText;
    private string initialBestTimeSurvivedText;

    private static GameOverController instance;
    public static GameOverController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameOverController>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameOverController).ToString());
                    instance = singleton.AddComponent<GameOverController>();
                    DontDestroyOnLoad(singleton);
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
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        initialTimeSurvivedText = timeSurvivedText != null ? timeSurvivedText.text : "";
        initialBestTimeSurvivedText = bestTimeSurvivedText != null ? bestTimeSurvivedText.text : "";
        HideGameOverScreen();
    }

    public void ShowGameOverScreen()
    {
        if (gameoverScreen != null)
        {
            gameoverScreen.SetActive(true);
            timeSurvivedText.text = initialTimeSurvivedText + GameTimerController.Instance.GetTimerFormatText();

            float timer = GameTimerController.Instance.GetTimer();
            if (timer > PlayerPrefs.GetFloat("bestTimeSurvived")) {
                PlayerPrefs.SetFloat("bestTimeSurvived", timer);
                bestTimeSurvivedText.text = initialBestTimeSurvivedText + GameTimerController.Instance.GetTimerFormatText();
            }
        }
    }

    public void HideGameOverScreen()
    {
        if (gameoverScreen != null)
        {
            gameoverScreen.SetActive(false);
        }
    }
}

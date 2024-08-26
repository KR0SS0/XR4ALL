using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] private AudioClip normalHitSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject[] healthUI;

    private PlayerItemSwitcher playerItemSwitcher;
    private InputAction anyInputAction;
    private TutorialManager tutorial;

    private void Start()
    {
        playerItemSwitcher = FindObjectOfType<PlayerItemSwitcher>();
        tutorial = FindFirstObjectByType<TutorialManager>();
    }

    [ContextMenu("Hit()")]
    public void Hit()
    {
        if (playerItemSwitcher.IsShieldActive())
        {
            playerItemSwitcher.PlayShieldBlockSound();
            if(tutorial != null) 
            {
                if (tutorial.OngoingTutorial)
                {
                    Debug.Log("Tutorial Success block");
                    tutorial.OnPlayerSuccessBlock();
                }
            }

        } else
        {
            health--;
            audioSource.PlayOneShot(normalHitSound);

            if (tutorial != null)
            {
                if (tutorial.OngoingTutorial && tutorial.currentState != TutorialManager.TutorialState.TestWave)
                {
                    Debug.Log("Tutorial unsuccess block");
                    tutorial.OnPlayerGetHit();
                }
            }


            if (healthUI[health].gameObject != null)
            {
                healthUI[health].SetActive(false);
            }

            if (health <= 0)
            {
                if(tutorial.OngoingTutorial && tutorial.currentState == TutorialManager.TutorialState.TestWave)
                {
                    tutorial.OnFailureTestWave();
                }

                else
                {
                    Debug.Log("Player Controller Game Over");
                    GameOver();
                }
            }
        }
    }

    private void GameOver()
    {
        GameManager.Instance.EndGame();
    }

    public void StartGame()
    {
        foreach(var item in healthUI)
        {
            item.SetActive(true);
        }
        health = 3;
    }

    public void WaitingToStart() {
        foreach (var item in healthUI)
        {
            item.SetActive(false);
        }
        health = 3;
    }
}

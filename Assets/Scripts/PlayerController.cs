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

    private void Start()
    {
        playerItemSwitcher = FindObjectOfType<PlayerItemSwitcher>();
    }

    [ContextMenu("Hit()")]
    public void Hit()
    {
        if (playerItemSwitcher.IsShieldActive())
        {
            playerItemSwitcher.PlayShieldBlockSound();
        } else
        {
            health--;
            audioSource.PlayOneShot(normalHitSound);

            if (healthUI[health].gameObject != null)
            {
                healthUI[health].SetActive(false);
            }

            if (health <= 0)
            {
                GameOver();
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
    }

    public void WaitingToStart() {
        foreach (var item in healthUI)
        {
            item.SetActive(false);
        }
    }
}

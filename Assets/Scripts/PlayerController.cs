using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] private AudioClip normalHitSound;
    [SerializeField] private AudioSource audioSource;

    private PlayerItemSwitcher playerItemSwitcher;

    private void Start()
    {
        playerItemSwitcher = FindObjectOfType<PlayerItemSwitcher>();
    }

    public void Hit()
    {
        if (playerItemSwitcher.IsShieldActive())
        {
            playerItemSwitcher.PlayShieldBlockSound();
        } else
        {
            health--;
            audioSource.PlayOneShot(normalHitSound);

            if (health < 0)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        GameManager.Instance.EndGame();
    }
}

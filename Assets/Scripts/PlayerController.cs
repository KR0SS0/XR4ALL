using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int health = 3;

    public void Hit()
    {
        health--;
        if (health < 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        GameManager.Instance.EndGame();
    }
}

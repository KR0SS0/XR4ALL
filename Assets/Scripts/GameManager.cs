using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public TMP_Text waitingToStartText;

    // Enum for the game states
    public enum GameState
    {
        WaitingToStart,
        Active,
        GameOver
    }

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameManager).ToString());
                    instance = singleton.AddComponent<GameManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return instance;
        }
    }

    private GameState currentState;

    public GameState CurrentState
    {
        get { return currentState; }
    }

    private InputAction anyInputAction;

    void Start()
    {
        SetGameState(GameState.WaitingToStart);

        anyInputAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/anyKey");
        anyInputAction.AddBinding("<Mouse>/press");
        anyInputAction.AddBinding("<Gamepad>/buttonSouth");
        anyInputAction.AddBinding("<XRController>/triggerPressed");
        anyInputAction.performed += ctx => OnAnyInput();
        anyInputAction.Enable();
    }

    public void SetGameState(GameState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case GameState.WaitingToStart:
            Debug.Log("Game is waiting to start");
            break;
            case GameState.Active:
            Debug.Log("Game is active");
            break;
            case GameState.GameOver:
            Debug.Log("Game is over");
            break;
        }
    }

    public void WaitToStartGame()
    {
        waitingToStartText.enabled = true;
    }

    // Method to start the game
    public void StartGame()
    {
        SetGameState(GameState.Active);
        waitingToStartText.enabled = false;
    }

    // Method to end the game
    public void EndGame()
    {
        SetGameState(GameState.GameOver);
    }

    private void OnAnyInput()
    {
        if (currentState == GameState.WaitingToStart)
        {
            StartGame();
        }
    }
}

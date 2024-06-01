using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    private InputAction debugCycleStateAction;

    void Start()
    {
        SetGameState(GameState.WaitingToStart);

        anyInputAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/anyKey");
        anyInputAction.AddBinding("<Mouse>/press");
        anyInputAction.AddBinding("<Gamepad>/buttonSouth");
        anyInputAction.AddBinding("<XRController>/triggerPressed");
        anyInputAction.AddBinding("<XRController>/gripPressed");
        anyInputAction.AddBinding("<XRController>/primaryButton");
        anyInputAction.AddBinding("<XRController>/secondaryButton");
        anyInputAction.performed += ctx => OnAnyInput();
        anyInputAction.Enable();

        debugCycleStateAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/d");
        debugCycleStateAction.performed += ctx => CycleGameState();
        debugCycleStateAction.Enable();
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

    private void CycleGameState()
    {
        switch (currentState)
        {
            case GameState.WaitingToStart:
            StartGame();
            break;
            case GameState.Active:
            EndGame();
            break;
            case GameState.GameOver:
            SetGameState(GameState.WaitingToStart);
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
        GameOverController.Instance.HideGameOverScreen();
    }

    // Method to end the game
    public void EndGame()
    {
        SetGameState(GameState.GameOver);
        GameOverController.Instance.ShowGameOverScreen();
    }

    private void OnAnyInput()
    {
        if (currentState == GameState.WaitingToStart)
        {
            StartGame();
        }
    }
}

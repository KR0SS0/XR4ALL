using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.CullingGroup;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnAnyInputOccurred;
    public event EventHandler OnStartInputOccurred;

    public GameObject startGame;

    [SerializeField] private TMP_Text inputText;

    [SerializeField] private GameObject VR_Lightsaber;
    [SerializeField] private GameObject Gamepad_Lightsaber;
    [SerializeField] private GameObject Joystick_Lightsaber;

    private AccessibilityController accessController;

    private const int LIGHTSABER_SCENE_INDEX = 1;
    private const int TUTORIAL_SCENE_INDEX = 2;

    // Enum for the game states
    public enum GameState
    {
        WaitingToStart,
        Active,
        GameOver
    }

    // Current Input Mode
    public enum AssistMode
    {
        VRController,
        VoiceCommand,
        JoystickButton,
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
    private InputAction startInputAction;
    private InputAction debugCycleStateAction;

    void Start()
    {
        SetGameState(GameState.WaitingToStart);

        anyInputAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/buttonSouth");
        anyInputAction.AddBinding("<XRController>/triggerPressed");
        anyInputAction.AddBinding("<XRController>/gripPressed");
        anyInputAction.AddBinding("<XRController>/primaryButton");
        anyInputAction.AddBinding("<XRController>/secondaryButton");
        anyInputAction.performed += ctx => OnAnyInput();
        anyInputAction.Enable();

        startInputAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/enter");
        startInputAction.performed += ctx => OnStartInput();
        startInputAction.Enable();

        //debugCycleStateAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/d");
        //debugCycleStateAction.performed += ctx => CycleGameState();
        //debugCycleStateAction.Enable();

        accessController = FindObjectOfType<AccessibilityController>();

        inputText.text = accessController.GetInputTypeString();
    }

    private void Update()
    {
        CheckActivationStatus();
        inputText.text = accessController.GetInputTypeString();
    }

    public void SetGameState(GameState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case GameState.WaitingToStart:
            WaitToStartGame();
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
            WaitToStartGame();
            break;
        }
    }

    public void WaitToStartGame()
    {
        startGame.SetActive(true);
        FindObjectOfType<PlayerController>().WaitingToStart();
    }

    // Method to start the game
    public void StartGame()
    {
        SetGameState(GameState.Active);
        FindObjectOfType<PlayerController>().StartGame();
        FindObjectOfType<DroneSpawner>().StartGame();
        if(startGame) {
            startGame.SetActive(false);
        }
        GameOverController.Instance.HideGameOverScreen();
    }

    private void StartTutorial()
    {
        SetGameState(GameState.Active);
        FindObjectOfType<TutorialManager>().StartTutorial();
        if(startGame) {
            startGame.SetActive(false);
        }
        GameOverController.Instance.HideGameOverScreen();
    }

    //
    public void RestartWave()
    {
        FindObjectOfType<PlayerController>().StartGame();
        FindObjectOfType<DroneSpawner>().RestartGame();
    }

    // Method to end the game
    public void EndGame()
    {
        SetGameState(GameState.GameOver);
        FindObjectOfType<DroneSpawner>().EndGame();
        GameOverController.Instance.ShowGameOverScreen();
    }

    private void OnAnyInput()
    {
        OnAnyInputOccurred?.Invoke(this, EventArgs.Empty);
    }

    private void OnStartInput()
    {
        if (currentState == GameState.WaitingToStart)
        {
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case LIGHTSABER_SCENE_INDEX:
                    StartGame();
                    break;

                case TUTORIAL_SCENE_INDEX:
                    StartTutorial();
                    break;
            }
            
        }
        OnStartInputOccurred?.Invoke(this, EventArgs.Empty);
    }

    private void CheckActivationStatus()
    {
        if (accessController.GetControllerPresetIndex() == 0)
        {
            VR_Lightsaber.gameObject.SetActive(true);
            Gamepad_Lightsaber.gameObject.SetActive(false);
            Joystick_Lightsaber.gameObject.SetActive(false);
        } 
        else if(accessController.GetControllerPresetIndex() == 1)
        {
            VR_Lightsaber.gameObject.SetActive(false);
            Gamepad_Lightsaber.gameObject.SetActive(true);
            Joystick_Lightsaber.gameObject.SetActive(false);
        } 
        else if (accessController.GetControllerPresetIndex() == 2)
        {
            VR_Lightsaber.gameObject.SetActive(false);
            Gamepad_Lightsaber.gameObject.SetActive(false);
            Joystick_Lightsaber.gameObject.SetActive(true);
        }
    }

    public AssistMode GetCurrentAssistMode()
    {
        int presetIndex = (int) accessController.GetControllerPresetIndex();

        switch (presetIndex)
        {
            case 0:
                return AssistMode.VRController;
            case 1:
                return AssistMode.VoiceCommand;
            case 2:
                return AssistMode.JoystickButton;
            default:
                return AssistMode.VRController;
        }
    }
}

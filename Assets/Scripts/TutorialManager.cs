
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static GameManager;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject oneHitDrone;
    [SerializeField] private GameObject twoHitsDrone;
    [SerializeField] private GameObject explosiveDrone;

    [SerializeField] private DroneSpawner droneSpawner;
    private GameManager gameManager;

    public TutorialState currentState = TutorialState.Standby;
    private AudioSource audioSource;
    [SerializeField] private AudioClip objetive;            //Objetive

    //VR controller
    [SerializeField] private AudioClip controlsVR_AttackIntro; //Controls Attack Intro
    [SerializeField] private AudioClip[] controlsVR_Attack;    //Controls Attack 
    [SerializeField] private AudioClip controlsVR_DefendIntro; //Controls Defend Intro
    [SerializeField] private AudioClip[] controlsVR_Defend;    //Controls Defend 

    //Voice commands
    [SerializeField] private AudioClip controlsVC_AttackIntro; //Controls Attack Intro
    [SerializeField] private AudioClip[] controlsVC_Attack;    //Controls Attack
    [SerializeField] private AudioClip controlsVC_DefendIntro; //Controls Defend Intro
    [SerializeField] private AudioClip[] controlsVC_Defend;    //Controls Defend

    //Joystick + Button
    [SerializeField] private AudioClip controlsJB_AttackIntro; //Controls Attack Intro
    [SerializeField] private AudioClip[] controlsJB_Attack;    //Controls Attack
    [SerializeField] private AudioClip controlsJB_DefendIntro; //Controls Defend Intro
    [SerializeField] private AudioClip[] controlsJB_Defend;    //Controls Defend

    private AudioClip controlsAttackIntro; //Controls Attack Intro
    private AudioClip[] controlsAttack;    //Controls Attack [3]
    private AudioClip controlsDefendIntro; //Controls Defend Intro
    private AudioClip[] controlsDefend;    //Controls Defend [5]

    [SerializeField] private AudioClip dronesIntro;         //Drones Intro
    [SerializeField] private AudioClip dronesRegularIntro;  //Regular Drone
    [SerializeField] private AudioClip[] dronesPart1;       //Regular Drone
    [SerializeField] private AudioClip dronesShieldedIntro; //Shielded Drone
    [SerializeField] private AudioClip[] dronesPart2;       //Shielded Drone
    [SerializeField] private AudioClip dronesExplosiveIntro;//Explosive Drone
    [SerializeField] private AudioClip[] dronesPart3;       //Explosive Drone
    [SerializeField] private AudioClip testWaveIntro;       //Test Wave Intro
    [SerializeField] private AudioClip[] testWaveClips;     //Test Wave Fail/Success
    [SerializeField] private AudioClip tutorialComplete;    //Completed Tutorial
    private Transform staticDroneSpawnLocation;
    private Transform movingDroneSpawnLocation;

    public enum TutorialState { Standby, Objetive, ControlsAttack, ControlsDefend, Drones, DronesRegular, DronesShielded, DronesExplosive,
        TestWave, Completed }
    public bool OngoingTutorial { get => ongoingTutorial; set => ongoingTutorial = value; }

    private bool ongoingTutorial = false;
    private DebugInput debugInputActions;

    private BaseDroneController currentActiveDrone;
    private float sourceVolume;

    private Coroutine instatiateDroneCoroutine;
    private Coroutine nextCoroutine;

    void Start()
    {
        transform.position = Vector3.zero;

        audioSource = GetComponent<AudioSource>();
        sourceVolume = audioSource.volume;

        debugInputActions = new DebugInput();
        debugInputActions.Debug.DebugAction.performed += OnDebugAction;
        debugInputActions.Enable();

        staticDroneSpawnLocation = transform.GetChild(0);
        movingDroneSpawnLocation = transform.GetChild(1);
        //Debug.Log("Spawn Location: " + staticDroneSpawnLocation.position);

        droneSpawner.SetTutorialManager(this);

        gameManager = FindFirstObjectByType<GameManager>();

 

    }

    private void OnDebugAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Space key pressed");
            if (!ongoingTutorial)
            {
                //StartTutorial();
            }
        }
    }

    //Change voice lines depending on current AssistMode
    public void SetVoicelineAssistMode()
    {
        AssistMode assistMode = gameManager.GetCurrentAssistMode();

        switch (assistMode)
        {
            case AssistMode.VRController:
                controlsAttackIntro = controlsVR_AttackIntro; 
                controlsAttack = controlsVR_Attack;    
                controlsDefendIntro = controlsVR_DefendIntro; 
                controlsDefend = controlsVR_Defend;
                break;


            case AssistMode.VoiceCommand:
                controlsAttackIntro = controlsVC_AttackIntro;
                controlsAttack = controlsVC_Attack;
                controlsDefendIntro = controlsVC_DefendIntro;
                controlsDefend = controlsVC_Defend;
                break;


            case AssistMode.JoystickButton:
                controlsAttackIntro = controlsJB_AttackIntro;
                controlsAttack = controlsJB_Attack;
                controlsDefendIntro = controlsJB_DefendIntro;
                controlsDefend = controlsJB_Defend;
                break;
        }
    }

    public void StartTutorial()
    {
        SetVoicelineAssistMode();
        ongoingTutorial = true;
        currentState = TutorialState.Objetive;
        if(nextCoroutine == null)
        {
            nextCoroutine = StartCoroutine(WaitNextState());
        }
    }

    private void PlaySound(AudioClip audioClip)
    {
        audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.volume = Random.Range(sourceVolume - 0.05f, sourceVolume + 0.05f);
        audioSource.pitch = Random.Range(0.98f, 1.02f);
        audioSource.Play();
    }

    private void PlaySound(AudioClip audioClip, bool playNext)
    {
        PlaySound(audioClip);

        if(playNext)
        {
            StartCoroutine(WaitAndPlay(audioClip.length / audioSource.pitch));
        }
    }

    private IEnumerator AwaitForIdle(AudioClip audioClip)
    {
        /*
        yield return new WaitForSeconds(0f);
        PlaySound(audioClip);
        StartCoroutine(AwaitForIdle(audioClip));
        */
        yield return null;
    }

    private IEnumerator WaitAndPlay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if(nextCoroutine == null)
        {
            currentState++;
            nextCoroutine = StartCoroutine(WaitNextState());
        }

        yield return null;
    }

    private IEnumerator WaitNextState()
    {
        Debug.Log("Hello");
        switch (currentState)
        {
            case TutorialState.Standby:
                break;

            case TutorialState.Objetive:
                PlaySound(objetive, true);
                break;

            case TutorialState.ControlsAttack:
                PlaySound(controlsAttackIntro);
                StartDroneCoroutine(controlsAttackIntro.length / 3f);
                break;

            case TutorialState.ControlsDefend:
                PlaySound(controlsDefendIntro);
                StartDroneCoroutine(controlsDefendIntro.length / 2f);
                break;

            case TutorialState.Drones:
                PlaySound(dronesIntro, true);
                StartCoroutine(ShowAndHideDrones(dronesIntro.length));
                break;

            case TutorialState.DronesRegular:
                PlaySound(dronesRegularIntro);
                StartDroneCoroutine(dronesRegularIntro.length);
                break;

            case TutorialState.DronesShielded:
                PlaySound(dronesShieldedIntro);
                StartDroneCoroutine(dronesShieldedIntro.length);
                break;

            case TutorialState.DronesExplosive:
                PlaySound(dronesExplosiveIntro);
                StartDroneCoroutine(dronesExplosiveIntro.length);
                break;

            case TutorialState.TestWave:
                PlaySound(testWaveIntro);
                StartDroneCoroutine(testWaveIntro.length);
                break;

            case TutorialState.Completed:
                PlaySound(tutorialComplete);
                StartCoroutine(EndTutorial());
                break;
        }
        yield return new WaitForSeconds(3);
        nextCoroutine = null;
    }

    private void StartDroneCoroutine(float waitTime)
    {
        if (instatiateDroneCoroutine == null)
        {
            instatiateDroneCoroutine = StartCoroutine(InstatiateDrone(waitTime));
        }

        else {
            Debug.Log("Tried to re-start coroutine");
        }
    }

    public void NotEnoughForceToDestroy()
    {
        switch (currentState)
        {
            case TutorialState.ControlsAttack:
                PlaySound(controlsAttack[1]);
                break;

            case TutorialState.ControlsDefend:
                break;

            case TutorialState.DronesRegular:
                PlaySound(controlsAttack[1]);
                break;

            case TutorialState.DronesShielded:
                PlaySound(controlsAttack[1]);
                break;

            default:
                break;

        }
    }

    public void OnDroneDestroyed()
    {
        switch (currentState)
        {
            case TutorialState.ControlsAttack:

                PlaySound(controlsAttack[2], true);
                break;

            case TutorialState.ControlsDefend:
                PlaySound(controlsDefend[3]);
                StartDroneCoroutine(controlsDefend[3].length);
                break;

            case TutorialState.DronesRegular:
                PlaySound(dronesPart1[1], true);
                break;

            case TutorialState.DronesShielded:
                PlaySound(dronesPart2[1], true);
                break;

            case TutorialState.DronesExplosive:
                PlaySound(dronesPart3[0]);
                StartDroneCoroutine(dronesPart3[0].length);
                break;

            default: 
                break;
        }
    }

    public void OnPlayerGetHit()
    {
        switch (currentState)
        {
            case TutorialState.ControlsDefend:
                PlaySound(controlsDefend[1]);
                currentActiveDrone.ForceDestroy();
                StartDroneCoroutine(controlsDefend[1].length);
                break;

            case TutorialState.DronesRegular:
                currentActiveDrone.ForceDestroy();
                PlaySound(dronesPart1[0]);
                StartDroneCoroutine(dronesPart1[0].length);
                break;

            case TutorialState.DronesShielded:
                currentActiveDrone.ForceDestroy();
                PlaySound(dronesPart2[0]);
                StartDroneCoroutine(dronesPart2[0].length);
                break;

            case TutorialState.DronesExplosive:
                StartCoroutine(DelayedPlaySound(controlsDefend[2], 1.0f , false));
                StartDroneCoroutine(controlsDefend[2].length + 1f);
                break;

            default:
                break;
        }
    }

    public void OnPlayerSuccessBlock()
    {
        switch (currentState)
        {
            case TutorialState.ControlsDefend:
                PlaySound(controlsDefend[4], true);
                currentActiveDrone.ForceDestroy();
                break;

            case TutorialState.DronesExplosive:
                StartCoroutine(DelayedPlaySound(dronesPart3[1], 1.0f, true));
                break;

            default:
                break;
        }
    }

    private IEnumerator DelayedPlaySound(AudioClip clip, float delay, bool playNext)
    {
        Debug.Log("Playing: " + clip.name);
        yield return new WaitForSeconds(delay);
        PlaySound(clip, playNext);
    }


    public void OnFailureTestWave()
    {
        PlaySound(testWaveClips[0]);
        droneSpawner.DestroyAllDrones();
        StartCoroutine(RestartTestWave());
    }

    public void OnSuccessTestWave()
    {
        PlaySound(testWaveClips[1], true);
    }

    private IEnumerator RestartTestWave()
    {
        yield return new WaitForSeconds(testWaveClips[0].length);
        gameManager.RestartWave();
        yield return null;
    }

    private IEnumerator EndTutorial()
    {
        yield return new WaitForSeconds(tutorialComplete.length);
        gameManager.EndGame();
        yield return null;
    }

    private IEnumerator InstatiateDrone(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        GameObject newDrone;
        switch (currentState)
        {
            case TutorialState.ControlsAttack:
                newDrone = Instantiate(oneHitDrone, staticDroneSpawnLocation.position, staticDroneSpawnLocation.rotation);
                currentActiveDrone = newDrone.GetComponent<BaseDroneController>();
                currentActiveDrone.StaticDrone = true;
                currentActiveDrone.SetSpawner(droneSpawner);
                newDrone.GetComponent<ITutorial>().SetManager(this);
                droneSpawner.AddDrone(newDrone);
                break;

            case TutorialState.ControlsDefend:
                newDrone = Instantiate(oneHitDrone, staticDroneSpawnLocation.position, staticDroneSpawnLocation.rotation);
                currentActiveDrone = newDrone.GetComponent<BaseDroneController>();
                currentActiveDrone.StaticDrone = true;
                currentActiveDrone.SetSpawner(droneSpawner);
                SwitchDroneState(controlsDefendIntro.length / 2f);
                newDrone.GetComponent<ITutorial>().SetManager(this);
                droneSpawner.AddDrone(newDrone);
                break;

            case TutorialState.DronesRegular:
                newDrone = Instantiate(oneHitDrone, movingDroneSpawnLocation.position, movingDroneSpawnLocation.rotation);
                newDrone.GetComponent<ITutorial>().SetManager(this);
                currentActiveDrone = newDrone.GetComponent<BaseDroneController>();
                currentActiveDrone.SetSpawner(droneSpawner);
                droneSpawner.AddDrone(newDrone);
                break;

            case TutorialState.DronesShielded:
                newDrone = Instantiate(twoHitsDrone, movingDroneSpawnLocation.position, movingDroneSpawnLocation.rotation);
                newDrone.GetComponent<ITutorial>().SetManager(this);
                currentActiveDrone = newDrone.GetComponent<BaseDroneController>();
                currentActiveDrone.SetSpawner(droneSpawner);
                droneSpawner.AddDrone(newDrone);
                break;

            case TutorialState.DronesExplosive:
                newDrone = Instantiate(explosiveDrone, movingDroneSpawnLocation.position, movingDroneSpawnLocation.rotation);
                newDrone.GetComponent<ITutorial>().SetManager(this);
                currentActiveDrone = newDrone.GetComponent<BaseDroneController>();
                currentActiveDrone.SetSpawner(droneSpawner);
                droneSpawner.AddDrone(newDrone);
                break;

            default:
                gameManager.StartGame();
                currentActiveDrone = null;
                break;
        }
        instatiateDroneCoroutine = null;
        yield return null;
    }


    private IEnumerator ShowAndHideDrones(float introTime)
    {
        GameObject[] exampleDrones = new GameObject[transform.GetChild(2).childCount]; 
        
        for (int i = 0; i < transform.GetChild(2).childCount; i++)
        {
            exampleDrones[i] = transform.GetChild(2).GetChild(i).gameObject;
        }

        float halftime = introTime / 2;
        yield return new WaitForSeconds(halftime - 0.2f);
        exampleDrones[0].SetActive(true);

        yield return new WaitForSeconds(1.25f);
        exampleDrones[1].SetActive(true);

        yield return new WaitForSeconds(1.25f);
        exampleDrones[2].SetActive(true);

        yield return new WaitForSeconds(4f);

        for (int i = 1; i < transform.GetChild(2).childCount; i++)
        {
            exampleDrones[i].SetActive(false);
        }

        yield return new WaitForSeconds(8f);

        exampleDrones[0].SetActive(false);

        yield return null;
    }

    private void SwitchDroneState(float waitTime)
    {
        StartCoroutine(DroneWaitTime(waitTime));
    }

    private IEnumerator DroneWaitTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        currentActiveDrone.SwitchToAttackState();
    }
}

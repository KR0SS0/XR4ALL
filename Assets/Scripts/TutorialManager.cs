
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject oneHitDrone;
    [SerializeField] private GameObject twoHitsDrone;
    [SerializeField] private GameObject explosiveDrone;

    public TutorialState currentState = TutorialState.Standby;
    private AudioSource audioSource;
    [SerializeField] private AudioClip objetive;            //Objetive
    [SerializeField] private AudioClip controlsAttackIntro;  //Controls Intro
    [SerializeField] private AudioClip[] controlsAttack;    //Controls Attack
    [SerializeField] private AudioClip controlsDefendIntro;  //Controls Intro
    [SerializeField] private AudioClip[] controlsDefend;    //Controls Defend
    [SerializeField] private AudioClip dronesIntro;         //Drones Intro
    [SerializeField] private AudioClip dronesRegularIntro;  //Regular Drone
    [SerializeField] private AudioClip[] dronesPart1;       //Regular Drone
    [SerializeField] private AudioClip dronesShieldedIntro; //Shielded Drone
    [SerializeField] private AudioClip[] dronesPart2;       //Shielded Drone
    [SerializeField] private AudioClip dronesExplosiveIntro;//Explosive Drone
    [SerializeField] private AudioClip[] dronesPart3;       //Explosive Drone
    [SerializeField] private AudioClip tutorialComplete;    //Completed Tutorial
    public Transform staticDroneSpawnLocation;
    public Transform movingDroneSpawnLocation;

    public enum TutorialState { Standby, Objetive, ControlsAttack, ControlsDefend, Drones, DronesRegular, DronesShielded, DronesExplosive, Completed }

    private bool ongoingTutorial = false;
    private DebugInput debugInputActions;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        debugInputActions = new DebugInput();
        debugInputActions.Debug.DebugAction.performed += OnDebugAction;
        debugInputActions.Enable();

        staticDroneSpawnLocation = transform.GetChild(0);
        movingDroneSpawnLocation = transform.GetChild(1);
        Debug.Log("Spawn Location: " + staticDroneSpawnLocation.position);
    }

    private void OnDebugAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Space key pressed");
            if (!ongoingTutorial)
            {
                StartTutorial();
            }
        }
    }

    private void StartTutorial()
    {
        ongoingTutorial = true;
        currentState = TutorialState.Objetive;
        NextState();
    }

    private void PlaySound(AudioClip audioClip)
    {
        audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private void PlaySound(AudioClip audioClip, bool playNext)
    {

        audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.Play();

        if(playNext)
        {
            StartCoroutine(WaitAndPlay(audioClip.length));
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
        currentState++;
        NextState();
        yield return null;
    }

    private void NextState()
    {
        switch (currentState)
        {
            case TutorialState.Standby:
                break;

            case TutorialState.Objetive:
                PlaySound(objetive, true);
                break;

            case TutorialState.ControlsAttack:
                PlaySound(controlsAttackIntro);
                StartCoroutine(InstatiateDrone(controlsAttackIntro.length / 3f));
                break;

            case TutorialState.ControlsDefend:
                PlaySound(controlsDefendIntro);
                StartCoroutine(InstatiateDrone(controlsDefendIntro.length / 2f));
                break;

            case TutorialState.Drones:
                PlaySound(dronesIntro, true);
                break;

            case TutorialState.DronesRegular:
                PlaySound(dronesRegularIntro);
                StartCoroutine(InstatiateDrone(dronesRegularIntro.length));
                break;

            case TutorialState.DronesShielded:
                PlaySound(dronesShieldedIntro);
                StartCoroutine(InstatiateDrone(dronesShieldedIntro.length));
                break;

            case TutorialState.DronesExplosive:
                PlaySound(dronesExplosiveIntro);
                StartCoroutine(InstatiateDrone(dronesExplosiveIntro.length));
                break;

            case TutorialState.Completed:
                PlaySound(tutorialComplete);
                break;
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

            case TutorialState.DronesExplosive:               
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
                break;

            case TutorialState.DronesRegular:
                PlaySound(dronesPart1[1], true);
                break;

            case TutorialState.DronesShielded:
                PlaySound(dronesPart2[1], true);
                break;

            case TutorialState.DronesExplosive:
                PlaySound(dronesPart3[0]);
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
                break;
            case TutorialState.DronesRegular:
                PlaySound(dronesPart1[0]);
                break;
            case TutorialState.DronesShielded:
                PlaySound(dronesPart2[0]);
                break;
            case TutorialState.DronesExplosive:
                PlaySound(controlsDefend[2]);
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
                break;

            case TutorialState.DronesExplosive:
                PlaySound(dronesPart3[1], true);
                break;

            default:
                break;
        }
    }

    private IEnumerator InstatiateDrone(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);

        GameObject newDrone;
        switch (currentState)
        {
            case TutorialState.ControlsAttack:
                newDrone = Instantiate(oneHitDrone, staticDroneSpawnLocation.position, staticDroneSpawnLocation.rotation);
                newDrone.GetComponent<BaseDroneController>().StaticDrone = true;
                newDrone.GetComponent<ITutorial>().SetManager(this);
                //newDrone.GetComponent<ITutorial>().Test();
                break;

            case TutorialState.ControlsDefend:
                newDrone = Instantiate(oneHitDrone, staticDroneSpawnLocation.position, staticDroneSpawnLocation.rotation);
                newDrone.GetComponent<ITutorial>().SetManager(this);
                break;

            case TutorialState.DronesRegular:
                newDrone = Instantiate(oneHitDrone, movingDroneSpawnLocation.position, movingDroneSpawnLocation.rotation);
                newDrone.GetComponent<ITutorial>().SetManager(this);
                break;

            case TutorialState.DronesShielded:
                newDrone = Instantiate(twoHitsDrone, movingDroneSpawnLocation.position, movingDroneSpawnLocation.rotation);
                newDrone.GetComponent<ITutorial>().SetManager(this);
                break;

            case TutorialState.DronesExplosive:
                newDrone = Instantiate(explosiveDrone, movingDroneSpawnLocation.position, movingDroneSpawnLocation.rotation);
                newDrone.GetComponent<ITutorial>().SetManager(this);
                break;

            default:
                break;
        }

        yield return null;
    }

}

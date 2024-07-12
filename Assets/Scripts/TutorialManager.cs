
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
    [SerializeField] private AudioClip controlsAttackIntro; //Controls Intro
    [SerializeField] private AudioClip[] controlsAttack;    //Controls Attack
    [SerializeField] private AudioClip controlsDefendIntro; //Controls Intro
    [SerializeField] private AudioClip[] controlsDefend;    //Controls Defend
    [SerializeField] private AudioClip dronesIntro;         //Drones Intro
    [SerializeField] private AudioClip dronesRegularIntro;  //Regular Drone
    [SerializeField] private AudioClip[] dronesPart1;       //Regular Drone
    [SerializeField] private AudioClip dronesShieldedIntro; //Shielded Drone
    [SerializeField] private AudioClip[] dronesPart2;       //Shielded Drone
    [SerializeField] private AudioClip dronesExplosiveIntro;//Explosive Drone
    [SerializeField] private AudioClip[] dronesPart3;       //Explosive Drone
    [SerializeField] private AudioClip tutorialComplete;    //Completed Tutorial
    private Transform staticDroneSpawnLocation;
    private Transform movingDroneSpawnLocation;

    public enum TutorialState { Standby, Objetive, ControlsAttack, ControlsDefend, Drones, DronesRegular, DronesShielded, DronesExplosive, Completed }
    public bool OngoingTutorial { get => ongoingTutorial; set => ongoingTutorial = value; }

    private bool ongoingTutorial = false;
    private DebugInput debugInputActions;

    private BaseDroneController currentActiveDrone;

    void Start()
    {
        transform.position = Vector3.zero;

        audioSource = GetComponent<AudioSource>();

        debugInputActions = new DebugInput();
        debugInputActions.Debug.DebugAction.performed += OnDebugAction;
        debugInputActions.Enable();

        staticDroneSpawnLocation = transform.GetChild(0);
        movingDroneSpawnLocation = transform.GetChild(1);
        //Debug.Log("Spawn Location: " + staticDroneSpawnLocation.position);
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
                StartCoroutine(ShowAndHideDrones(dronesIntro.length));
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
                StartCoroutine(InstatiateDrone(controlsDefend[3].length));
                break;

            case TutorialState.DronesRegular:
                PlaySound(dronesPart1[1], true);
                break;

            case TutorialState.DronesShielded:
                PlaySound(dronesPart2[1], true);
                break;

            case TutorialState.DronesExplosive:
                PlaySound(dronesPart3[0]);
                StartCoroutine(InstatiateDrone(dronesPart3[0].length));
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
                StartCoroutine(InstatiateDrone(controlsDefend[1].length));
                break;
            case TutorialState.DronesRegular:
                currentActiveDrone.ForceDestroy();
                PlaySound(dronesPart1[0]);
                StartCoroutine(InstatiateDrone(dronesPart1[0].length));
                break;
            case TutorialState.DronesShielded:
                currentActiveDrone.ForceDestroy();
                PlaySound(dronesPart2[0]);
                StartCoroutine(InstatiateDrone(dronesPart2[0].length));
                break;
            case TutorialState.DronesExplosive:
                PlaySound(controlsDefend[2]);
                StartCoroutine(InstatiateDrone(controlsDefend[2].length));
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
                newDrone = new();
                break;
        }

        currentActiveDrone = newDrone.GetComponent<BaseDroneController>();
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
}

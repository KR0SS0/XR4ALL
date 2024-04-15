using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RT_RingSpawner : MonoBehaviour
{
    public int ringCount = 10;
    public float timeToSpawn = 2f;
    private float timer = 0f;
    private bool isReadyToSpawn;
    public GameObject ringPrefab;
    private GameObject currentRing;
    private Rigidbody ringRigidbody;
    private bool isSpawnBlocked = false;

    private XRGrabInteractable grabInteractable;

    public
        TMP_Text ringText;

    // Start is called before the first frame update
    void Start()
    {
        ringText.text = "Rings remaining: " + ringCount;
        SpawnNewRing();
    }

    private void Update()
    {
        if(isReadyToSpawn)
        {
            timer += Time.deltaTime;
            if (timer > timeToSpawn)
            {
                if(isSpawnBlocked) return;
                SpawnNewRing();
                isReadyToSpawn = false;
                timer = 0f;
            } 
        }
    }

    private void SpawnNewRing()
    {
        if(ringCount > 0)
        {
            if(grabInteractable != null)
            {
                grabInteractable.onSelectEntered.RemoveAllListeners();
                grabInteractable.onSelectExited.RemoveAllListeners();
            } 

            currentRing = Instantiate(ringPrefab, gameObject.transform.position, Quaternion.identity);

            ringRigidbody = currentRing.GetComponent<Rigidbody>();

            grabInteractable = currentRing.GetComponent<XRGrabInteractable>();
            grabInteractable.onSelectEntered.AddListener(OnGrab);
            grabInteractable.onSelectExited.AddListener(OnRelease);

            ringCount--;
            ringText.text = "Rings remaining: " + ringCount;
        }
    }

    void OnGrab(XRBaseInteractor interactor)
    {
        Debug.Log("Object grabbed");
        // You can perform any actions you want when the object is grabbed
        isReadyToSpawn = true;

    }

    // Called when the object is released
    void OnRelease(XRBaseInteractor interactor)
    {
        Debug.Log("Object released");
        // You can perform any actions you want when the object is released
    }

    bool IsGrabbed()
    {
        return grabInteractable.isSelected;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("RT_Ring"))
        {
            isSpawnBlocked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RT_Ring"))
        {
            isSpawnBlocked = false;
        }
    }
}

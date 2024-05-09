using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Member;

public class SD_Shotgun : MonoBehaviour
{

    private const string duckTag = "SD_Duck";
    [SerializeField] private Transform bulletSpawnLocation;
    [SerializeField] private GameObject bullet;

    [SerializeField] private AudioClip bulletClip;
    [SerializeField] private AudioClip reloadClip;
    [SerializeField] private AudioSource source;
    
    [SerializeField] private float timeToReload = 2f;
    private float reloadTimer = 0;
    private bool hasReloaded;

    private Transform startTransform;

    [SerializeField] private SD_GameAndScoreManager game;

    private void Start()
    {
        startTransform = transform;
    }

    // Start is called before the first frame update

    private void Update()
    {
        if(!game.IsGameActive())
        {
            hasReloaded = false;
        } else
        {
            reloadTimer += Time.deltaTime;

            if (reloadTimer > timeToReload)
            {
                hasReloaded = true;
            } else
            {
                hasReloaded = false;
            }
        }
    }


    public void ShootBullet()
    {
        Debug.Log("Shoot action triggered!");
        if(hasReloaded) {
            reloadTimer = 0f;
            ShootRaycast();
            StartCoroutine(PlaySoundCoroutine());
        } 
    }


    private void ShootRaycast()
    {

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        ShootBullet(ray.direction);

        if (Physics.Raycast(ray, out hit, 10f))
        {          
            if (hit.collider.CompareTag(duckTag))
            {
                // Duck found
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                Debug.Log("Hit a duck!");
            }

            else
            {
                // No duck
                Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
                Debug.Log("No duck");
            }
        }   
        
        else
        {
            // No duck
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
            Debug.Log("No duck");
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(2f);
        ShootRaycast();
        yield return null;
    }

    private void ShootBullet(Vector3 direction)
    {
        Instantiate(bullet, bulletSpawnLocation.position, bulletSpawnLocation.rotation);
    }

    private IEnumerator PlaySoundCoroutine()
    {
        source.PlayOneShot(bulletClip);
        yield return new WaitForSeconds(0.2f);
        source.PlayOneShot(reloadClip);
    }

    public void OnStartGrabbing()
    {
    }

    public void OnStopGrabbing()
    {
        transform.SetPositionAndRotation(startTransform.position, startTransform.rotation);
    }
}

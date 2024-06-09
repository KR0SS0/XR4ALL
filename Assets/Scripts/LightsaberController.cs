using UnityEngine;
using UnityEngine.XR;

public class LightsaberController : MonoBehaviour
{
    [Header("Audio")]
    public float requiredSpeed = 1.0f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float replayThreshold = 0.6f;
    public AudioClip[] swingClips;
    public AudioClip[] strikeClips;
    public AudioSource source;

    public void PlaySwingSound()
    {
        float randomPitch = Random.Range(minPitch, maxPitch);
        source.pitch = randomPitch;

        AudioClip clip = swingClips[Random.Range(0, swingClips.Length)];
        source.clip = clip;
        source.PlayOneShot(clip);
    }

    public void PlayStrikeSound()
    {
        float randomPitch = Random.Range(minPitch, maxPitch);
        source.pitch = randomPitch;

        AudioClip clip = strikeClips[Random.Range(0, strikeClips.Length)];
        source.clip = clip;
        source.PlayOneShot(clip);
    }
    public BaseDroneController FindNearestDrone()
    {
        BaseDroneController[] drones = FindObjectsOfType<BaseDroneController>();
        BaseDroneController nearestDrone = null;
        float nearestDistance = Mathf.Infinity;

        foreach (BaseDroneController drone in drones)
        {
            float distance = Vector3.Distance(transform.position, drone.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestDrone = drone;
            }
        }

        return nearestDrone;
    }
}

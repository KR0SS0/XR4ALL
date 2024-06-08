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
    public AudioSource source;

    public void PlaySwingSound()
    {
        float randomPitch = Random.Range(minPitch, maxPitch);
        source.pitch = randomPitch;

        AudioClip clip = swingClips[Random.Range(0, swingClips.Length)];
        source.clip = clip;
        source.PlayOneShot(clip);
    }
}

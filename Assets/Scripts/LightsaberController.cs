using UnityEngine;
using UnityEngine.XR;

public class LightsaberController : MonoBehaviour
{
    [Header("Audio")]
    public float requiredSpeed = 1.0f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float replayThreshold = 0.6f;
    [SerializeField] private AudioClip[] swingClips;
    [SerializeField] private AudioSource source;

    public void PlaySwingSound()
    {
        float randomPitch = Random.Range(minPitch, maxPitch);
        source.pitch = randomPitch;

        AudioClip clip = swingClips[Random.Range(0, swingClips.Length)];
        source.clip = clip;
        source.PlayOneShot(clip);
    }

    public void PlayVRSwingSound(Vector3 velocity)
    {
        if (source.isPlaying)
        {
            if (source.clip != null)
            {
                float playbackPosition = source.time / source.clip.length;

                if (playbackPosition < replayThreshold)
                {
                    return;
                }
            }
        }
        float swingSpeed = velocity.magnitude;
        if (swingSpeed > requiredSpeed)
        {
            PlaySwingSound();
        }
    }
}

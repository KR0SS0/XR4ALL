using UnityEngine;
using UnityEngine.XR;

public class LightsaberController : MonoBehaviour
{
    public XRNode controllerNode;
    private Vector3 previousPosition;
    private Vector3 currentVelocity;

    private InputDevice controller;

    [Header("Audio")]
    public float requiredSpeed = 1.0f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float replayThreshold = 0.6f;
    [SerializeField] private AudioClip[] swingClips;
    [SerializeField] private AudioSource source;

    void Start()
    {
        controller = InputDevices.GetDeviceAtXRNode(controllerNode);
        controller.TryGetFeatureValue(CommonUsages.devicePosition, out previousPosition);
    }

    void Update()
    {
        Vector3 currentPosition;

        currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        if (controller.TryGetFeatureValue(CommonUsages.devicePosition, out currentPosition))
        {
        }
        PlaySwingSound();
    }

    public void PlaySwingSound()
    {
        if (source.isPlaying)
        {
            float playbackPosition = source.time / source.clip.length;

            if (playbackPosition < replayThreshold)
            {
                return;
            }
        }
        float swingSpeed = GetVelocity().magnitude;
        if (swingSpeed > requiredSpeed)
        {
            float randomPitch = Random.Range(minPitch, maxPitch);
            source.pitch = randomPitch;

            AudioClip clip = swingClips[Random.Range(0, swingClips.Length)];
            source.clip = clip;
            source.PlayOneShot(clip);
        }
    }

    public Vector3 GetVelocity()
    {
        return currentVelocity;
    }

    public Vector3 GetSwingDirection()
    {
        // Normalize the velocity to get the direction
        return currentVelocity.normalized;
    }
}

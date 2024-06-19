using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] hitClips;
    [SerializeField] private AudioClip[] spawnClips;
    [SerializeField] private AudioClip[] destroyClips;
    [SerializeField] private AudioClip[] stunClips;
    [SerializeField] private AudioClip[] chargeBulletAttackClips;
    [SerializeField] private AudioClip[] chargeExplosionAttackClips;
    [SerializeField] private AudioClip[] bulletAttackClips;
    [SerializeField] private AudioClip[] electricShotClips;
    [SerializeField] private AudioClip[] explosionClips;
    [SerializeField] private AudioClip[] movingClips;

    private Dictionary<PriorityLevel, float[]> volumeLevels;

    private AudioSource oneShotSource;
    private AudioSource loopSource;

    //one shots
    private float lowVolumeOS = 0.2f;
    private float mediumVolumeOS = 0.4f;
    private float highVolumeOS = 0.6f;

    //loops
    private float lowVolumeLoop = 0.15f;
    private float mediumVolumeLoop = 0.3f;
    private float highVolumeLoop = 0.4f;

    private float[] startVolumes;
    private float[] endVolumes;
    private float lerpDuration = 0.5f;
    private float elapsedTime = 0f;
    private bool isLerping = false;

    // Start is called before the first frame update
    void Start()
    {
        oneShotSource = GetComponents<AudioSource>()[0];
        loopSource = GetComponents<AudioSource>()[1];
        loopSource.clip = RandomClip(movingClips);
        loopSource.loop = true;

        volumeLevels = new Dictionary<PriorityLevel, float[]>
        {
            {PriorityLevel.low, new float [2] {lowVolumeOS, lowVolumeLoop} },
            {PriorityLevel.medium, new float [2] {mediumVolumeOS, mediumVolumeLoop} },
            {PriorityLevel.high, new float[2] { highVolumeOS, highVolumeLoop} },

        };

        PlaySpawnSound();

    }

    private void FixedUpdate()
    {
        if(isLerping)
        {
            LerpVolume();
        }
    }

    public void PlayHitSound()
    {
        oneShotSource.pitch = RandomPitch();
        oneShotSource.PlayOneShot(RandomClip(hitClips));
    }

    public void PlaySpawnSound()
    {
        oneShotSource.pitch = RandomPitch();
        oneShotSource.PlayOneShot(RandomClip(spawnClips));
    }

    public void PlayStunSound()
    {
        oneShotSource.pitch = RandomPitch();
        oneShotSource.PlayOneShot(RandomClip(stunClips));
    }

    public void PlayDestroySound()
    {
        oneShotSource.pitch = RandomPitch();
        oneShotSource.PlayOneShot(RandomClip(destroyClips));
    }

    public void PlayChargeAttackSound(DroneType droneType)
    {
        oneShotSource.pitch = RandomPitch();

        switch (droneType)
        {
            case DroneType.Explosive:
                oneShotSource.PlayOneShot(RandomClip(chargeExplosionAttackClips));
                break;
            default:
                oneShotSource.PlayOneShot(RandomClip(chargeBulletAttackClips));
                break;

        }
    }

    public void PlayAttackSound()
    {
        oneShotSource.pitch = RandomPitch();
        oneShotSource.PlayOneShot(RandomClip(bulletAttackClips));
        oneShotSource.PlayOneShot(RandomClip(electricShotClips));
    }

    public void StartMovingSound()
    {
        loopSource.pitch = RandomPitch();
        loopSource.Play();
    }

    public void PlayExplosionSound()
    {
        oneShotSource.pitch = RandomPitch();
        oneShotSource.PlayOneShot(RandomClip(explosionClips));
    }

    private float RandomPitch()
    {
        return Random.Range(0.9f, 1.1f);
    }

    private AudioClip RandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }

    public void SwitchLevel(PriorityLevel oldLevel, PriorityLevel newLevel)
    {
        if (oldLevel == newLevel) return;

        startVolumes = volumeLevels[oldLevel];
        endVolumes = volumeLevels[newLevel];
        elapsedTime = 0.0f;
        isLerping = true;

    }

    private void LerpVolume()
    {
        elapsedTime += Time.fixedDeltaTime;

        float t = elapsedTime / lerpDuration;

        oneShotSource.volume = Mathf.Lerp(startVolumes[0], endVolumes[0], t);
        loopSource.volume = Mathf.Lerp(startVolumes[1], endVolumes[1], t);

        if (elapsedTime >= lerpDuration)
        {
            isLerping = false;
            oneShotSource.volume = endVolumes[0];
            loopSource.volume = endVolumes[1];
        }
    }
}

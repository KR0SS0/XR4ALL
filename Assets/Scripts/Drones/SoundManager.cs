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

    private AudioSource audioSource;
    private AudioSource loopSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponents<AudioSource>()[0];
        loopSource = GetComponents<AudioSource>()[1];
        loopSource.clip = RandomClip(movingClips);

        PlaySpawnSound();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayHitSound()
    {
        audioSource.pitch = RandomPitch();
        audioSource.PlayOneShot(RandomClip(hitClips));
    }

    public void PlaySpawnSound()
    {
        audioSource.pitch = RandomPitch();
        audioSource.PlayOneShot(RandomClip(spawnClips));
    }

    public void PlayStunSound()
    {
        audioSource.pitch = RandomPitch();
        audioSource.PlayOneShot(RandomClip(stunClips));
    }

    public void PlayDestroySound()
    {
        loopSource.Stop();
        audioSource.pitch = RandomPitch();
        audioSource.PlayOneShot(RandomClip(destroyClips));
    }

    public void PlayChargeAttackSound(DroneType droneType)
    {
        loopSource.Stop();
        audioSource.pitch = RandomPitch();

        switch (droneType)
        {
            case DroneType.Explosive:
                audioSource.PlayOneShot(RandomClip(chargeExplosionAttackClips));
                break;
            default:
                audioSource.PlayOneShot(RandomClip(chargeBulletAttackClips));
                break;

        }
    }

    public void PlayAttackSound()
    {
        loopSource.Stop();
        audioSource.pitch = RandomPitch();
        audioSource.PlayOneShot(RandomClip(bulletAttackClips));
        audioSource.PlayOneShot(RandomClip(electricShotClips));
    }

    public void StartMovingSound()
    {
        loopSource.pitch = RandomPitch();
        loopSource.Play();
    }

    public void PlayExplosionSound()
    {
        loopSource.Stop();
        audioSource.pitch = RandomPitch();
        audioSource.PlayOneShot(RandomClip(explosionClips));
    }

    private float RandomPitch()
    {
        return Random.Range(0.9f, 1.1f);
    }

    private AudioClip RandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length - 1)];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{

    private ParticleSystem _particleSystem;

    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void StartTowerVFX()
    {
        _particleSystem.Play();
    }

    public void StopTowerVFX()
    {
        _particleSystem?.Stop();
    }

}

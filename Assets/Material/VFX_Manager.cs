using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VFX_Type { Spawn, Destroy, Idle }

public class VFX_Manager : MonoBehaviour
{
    private float spawnEffectTime = 2f;
    public AnimationCurve fadeIn;

    [SerializeField] private Renderer _renderer;
    [SerializeField] private GameObject[] VFX_objs;
    private ParticleSystem[] particleSystems;
    private float timer = 0;
    private int shaderProperty;
    private bool isPlayingVFX;
    private VFX_Type currentType = VFX_Type.Idle;

    void Start()
    {
        shaderProperty = Shader.PropertyToID("_Cutoff");
        isPlayingVFX = false;
    }

    private void GetParticleSystem(int type)
    {
        if(type < VFX_objs.Length)
        {
            particleSystems = VFX_objs[type].GetComponentsInChildren<ParticleSystem>();
            particleSystems[0].Play();
        }
    }

    void Update()
    {
        if (timer < spawnEffectTime && isPlayingVFX)
        {
            switch (currentType)
            {
                case VFX_Type.Spawn:
                    SpawnVFX();
                    break;

                case VFX_Type.Destroy:
                    DestroyVFX();
                    break;

                default: break;
            }

            timer += Time.deltaTime;
       
        }   
        
        else if (timer > spawnEffectTime)
        {
            isPlayingVFX = false;
            currentType = VFX_Type.Idle;
        }

        //if (currentType == VFX_Type.Idle) Debug.Log("Now Idle");
    }

    public void PlayVFX(VFX_Type type)
    {
        timer = 0;
        isPlayingVFX = true;
        GetParticleSystem((int) type);
        currentType = type;
    }

    private void SpawnVFX()
    {
        float value = fadeIn.Evaluate(1 - Mathf.InverseLerp(0, spawnEffectTime, timer));
        SetFloatValueToMaterial(value);
    }

    private void DestroyVFX()
    {  
        float value = fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer));
        SetFloatValueToMaterial(value);
    }

    private void SetFloatValueToMaterial(float value)
    {
        foreach (Material material in _renderer.materials)
        {
            material.SetFloat(shaderProperty, value);
        }
    }

    public void InterruptParticleSystems()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }

        SetFloatValueToMaterial(1f);
    }

    public bool IsParticleSystemDone
    {
        get
        {
            if (particleSystems == null || particleSystems.Length == 0)
            {
                return true;
            }

            foreach (ParticleSystem ps in particleSystems)
            {
                if (!ps.isStopped)
                {
                    return false; // Return false if any Particle System is still playing
                }
            }

            return true; 
        }
    }
}

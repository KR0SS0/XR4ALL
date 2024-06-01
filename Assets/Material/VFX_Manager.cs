using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VFX_Type { Spawn, Destroy, ChargeAttack, Idle }

public class VFX_Manager : MonoBehaviour
{
    private float effectTime;
    public AnimationCurve fadeIn;

    private Renderer _drone_renderer;
    private Renderer _ball_renderer;
    [SerializeField] private GameObject[] VFX_objs;
    private ParticleSystem[] particleSystems;
    private float timer = 0;
    private int shaderProperty;
    private bool isPlayingVFX;
    private VFX_Type currentType = VFX_Type.Idle;
    private const string DRONE_PATH = "SM_CYBER_Drone";

    [SerializeField] private List<Material> deathMaterials;

    void Start()
    {
        _drone_renderer = GetComponentInParent<Renderer>();
        _ball_renderer = VFX_objs[2].GetComponentInChildren<Renderer>();
        shaderProperty = Shader.PropertyToID("_Cutoff");
        isPlayingVFX = false;
        effectTime = 2f;
        DeactivateAllVFX();

    }

    private void GetParticleSystem(int type)
    {
        if(type < VFX_objs.Length)
        {
            VFX_objs[type].SetActive(true);
            particleSystems = VFX_objs[type].GetComponentsInChildren<ParticleSystem>();
            particleSystems[0].Play();
        }
    }

    private void DeactivateAllVFX()
    {
        foreach(GameObject obj in VFX_objs)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        if (timer < effectTime && isPlayingVFX)
        {
            switch (currentType)
            {
                case VFX_Type.Spawn:
                    SpawnVFX();
                    break;

                case VFX_Type.Destroy:
                    DestroyVFX();
                    break;

                case VFX_Type.ChargeAttack:
                    ChargeVFX();
                    break;

                default: break;
            }

            timer += Time.deltaTime;
       
        }   
        
        else if (timer > effectTime)
        {
            isPlayingVFX = false;
            currentType = VFX_Type.Idle;
            DeactivateAllVFX();
        }

        //if (currentType == VFX_Type.Idle) Debug.Log("Now Idle");
    }

    public void PlayVFX(VFX_Type type)
    {
        timer = 0;
        isPlayingVFX = true;
        GetParticleSystem((int) type);

        if (type == VFX_Type.Destroy) ChangeMaterials();

        currentType = type;
    }

    private void SpawnVFX()
    {
        float value = fadeIn.Evaluate(1 - Mathf.InverseLerp(0, effectTime, timer));
        SetFloatValueToMaterial(value);
    }

    private void DestroyVFX()
    {  
        float value = fadeIn.Evaluate(Mathf.InverseLerp(0, effectTime, timer));
        SetFloatValueToMaterial(value);
    }

    private void ChargeVFX()
    {
        float value = fadeIn.Evaluate(1 - Mathf.InverseLerp(0, effectTime, timer));
        _ball_renderer.material.SetFloat("Anim", value);
    }

    private void SetFloatValueToMaterial(float value)
    {
        foreach (Material material in _drone_renderer.materials)
        {
            material.SetFloat(shaderProperty, value);
        }
    }

    private void ChangeMaterials()
    {
        _drone_renderer.SetMaterials(deathMaterials);
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

    public Transform GetBulletSpawnLocation()
    {
        return VFX_objs[2].transform;
    }
}

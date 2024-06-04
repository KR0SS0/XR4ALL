using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum VFX_Type { Spawn, Destroy, ChargeAttack, Stunned, Idle }

public class VFX_Manager : MonoBehaviour
{
    private float effectTime = 2f;
    public AnimationCurve fadeIn;
    [SerializeField] private GameObject[] VFX_objs;
    private ParticleSystem[] particleSystems;
    private float timer = 0;
    private int droneShaderProperty;
    private int ballShaderProperty;
    private int sphereShaderProperty;
    private bool isPlayingVFX;
    private VFX_Type currentType = VFX_Type.Idle;
    private DroneType droneType;
    private Material[] droneMaterials;
    private Material ballMaterial;
    private Material sphereMaterial;
    private Renderer _droneRenderer;
    private AnimationCurve stunnedAnimation;
    private bool isSphereDisabled = false;

    [SerializeField] private Material[] deathMaterials;

    void Awake()
    {

        DeactivateAllVFX(false);
        //Debug.Log(droneType.ToString());    

        isPlayingVFX = false;
    }

    private void Start()
    {

        droneShaderProperty = Shader.PropertyToID("_Cutoff");
        ballShaderProperty = Shader.PropertyToID("Anim");
        sphereShaderProperty = Shader.PropertyToID("_AlphaIntensity");

        _droneRenderer = GetComponentInParent<Renderer>();
        droneMaterials = _droneRenderer.materials;
        ballMaterial = VFX_objs[2].GetComponentInChildren<Renderer>().material;

        if (VFX_objs.Length > 3)
        {
            sphereMaterial = VFX_objs[3].GetComponent<Renderer>().material;
            //Debug.Log(_sphere_shield_renderer.name);
        }


    }

    private void GetParticleSystem(int type)
    {
        switch (type)
        {
            case 0:
            case 1:
            case 2:
                VFX_objs[type].SetActive(true);

                Debug.Log("Setting active: " + VFX_objs[type].name + " with length: " + VFX_objs.Length);

                particleSystems = VFX_objs[type].GetComponentsInChildren<ParticleSystem>();

                particleSystems[0].Play();
                break;
        }
    }

    private void DeactivateAllVFX(bool deactivateSphere)
    {
        foreach(GameObject obj in VFX_objs)
        {
            if (obj.name != "SphereShield") 
            { 
                obj.SetActive(false);
            }
        }

        if (deactivateSphere)
        {
            VFX_objs[3].SetActive(false);
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

                case VFX_Type.Stunned:
                    StunVFX();
                    break;

                default: break;
            }

            timer += Time.deltaTime;
       
        }   
        
        else if (timer > effectTime && isPlayingVFX)
        {
            currentType = VFX_Type.Idle;
            Debug.Log("Deactivate vfx called in update");
            DeactivateAllVFX(isSphereDisabled);
            isPlayingVFX = false;
        }

        //if (currentType == VFX_Type.Idle) Debug.Log("Now Idle");
    }

    public void PlayVFX(VFX_Type type)
    {
        timer = 0;
        isPlayingVFX = true;
        GetParticleSystem((int) type);

        switch (type)
        {
            case VFX_Type.Destroy:
                effectTime = 2f;
                ChangeMaterials();
                break;

            case VFX_Type.Stunned:
                effectTime = 0.5f;
                isSphereDisabled = true;
                break;

            default :
                effectTime = 2f;
                break;

        }

        currentType = type;
    }

    private void StunVFX()
    {
        float value = stunnedAnimation.Evaluate(Mathf.InverseLerp(0, effectTime, timer));
        Debug.Log("Stun value: " + value);
        sphereMaterial.SetFloat(sphereShaderProperty, value);
    }

    private void SpawnVFX()
    {
        float value = fadeIn.Evaluate(1 - Mathf.InverseLerp(0, effectTime, timer));
        SetFloatValueToDroneMaterials(value);

        if(droneType == DroneType.TwoHits && sphereMaterial != null)
        {
            //Debug.Log(sphereMaterial.name + sphereMaterial.GetFloat(sphereShaderProperty));
            sphereMaterial.SetFloat(sphereShaderProperty, 1 - value);
        }
    }

    private void DestroyVFX()
    {  
        float value = fadeIn.Evaluate(Mathf.InverseLerp(0, effectTime, timer));
        SetFloatValueToDroneMaterials(value);
    }

    private void ChargeVFX()
    {
        float value = fadeIn.Evaluate(1 - Mathf.InverseLerp(0, effectTime, timer));
        ballMaterial.SetFloat(ballShaderProperty, value);
    }

    private void SetFloatValueToDroneMaterials(float value)
    {
        foreach (Material material in droneMaterials)
        {
            material.SetFloat(droneShaderProperty, value);
        }
    }

    private void ChangeMaterials()
    {
        _droneRenderer.materials = deathMaterials;
        droneMaterials = deathMaterials;
    }

    public void InterruptParticleSystems()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }

        SetFloatValueToDroneMaterials(0f);
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

    public DroneType DroneType { set => droneType = value; }
    public AnimationCurve StunnedAnimation { get => stunnedAnimation; set => stunnedAnimation = value; }

    public Transform GetBulletSpawnLocation()
    {
        return VFX_objs[2].transform;
    }
}

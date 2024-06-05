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
    [SerializeField] private Texture2D noiseMap;

    //[SerializeField] private Material[] deathMaterials;

    void Awake()
    {

        DeactivateAllVFX();
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

        if (VFX_objs.Length > 3 && droneType == DroneType.TwoHits)
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

                //Debug.Log("Setting active: " + VFX_objs[type].name + " with length: " + VFX_objs.Length);

                particleSystems = VFX_objs[type].GetComponentsInChildren<ParticleSystem>();

                particleSystems[0].Play();
                break;
        }
    }

    private void DeactivateAllVFX()
    {
        foreach(GameObject obj in VFX_objs)
        {
            if (obj.name != "SphereShield" && obj.name != "BombEmbers") 
            { 
                Debug.Log(obj.name + " setting inactive");
                obj.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (timer <= effectTime && isPlayingVFX)
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

       
        }   
        
        else if (timer > effectTime && isPlayingVFX)
        {
            currentType = VFX_Type.Idle;
            //Debug.Log("Deactivate vfx called in update");
            DeactivateAllVFX();
            isPlayingVFX = false;
        }

        timer += Time.deltaTime;
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
                ChangeNoiseMap();
                if(droneType == DroneType.Explosive)
                {
                    VFX_objs[3].SetActive(false);
                }

                break;

            case VFX_Type.Stunned:
                effectTime = 0.5f;
                VFX_objs[3].SetActive(false);
                break;

            default :
                effectTime = 2f;
                break;

        }

        currentType = type;
    }

    private void StunVFX()
    {
        float value = RoundEvaluation(stunnedAnimation.Evaluate(Mathf.InverseLerp(0, effectTime, timer)));
        Debug.Log("Stun value: " + value);
        sphereMaterial.SetFloat(sphereShaderProperty, value);
    }

    private void SpawnVFX()
    {
        float value = RoundEvaluation(fadeIn.Evaluate(1 - Mathf.InverseLerp(0, effectTime, timer)));
        SetFloatValueToDroneMaterials(value);

        if(droneType == DroneType.TwoHits && sphereMaterial != null)
        {
            //Debug.Log(sphereMaterial.name + sphereMaterial.GetFloat(sphereShaderProperty));
            sphereMaterial.SetFloat(sphereShaderProperty, 1 - value);
        }
    }

    private void DestroyVFX()
    {  
        float value = RoundEvaluation(fadeIn.Evaluate(Mathf.InverseLerp(0, effectTime, timer)));
        SetFloatValueToDroneMaterials(value);
    }

    private void ChargeVFX()
    {
        float value = RoundEvaluation(fadeIn.Evaluate(1 - Mathf.InverseLerp(0, effectTime, timer)));
        ballMaterial.SetFloat(ballShaderProperty, value);
    }

    private void SetFloatValueToDroneMaterials(float value)
    {
        foreach (Material material in droneMaterials)
        {
            material.SetFloat(droneShaderProperty, value);
        }
    }

    private void ChangeNoiseMap()
    {
        foreach (Material material in _droneRenderer.materials)
        {
            material.SetTexture("_NoiseTex", noiseMap);
        }
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

    private float RoundEvaluation(float value)
    {
        if (Mathf.Abs(value) < 0.001f)
        {
            return 0f;
        }
        else if (Mathf.Abs(value - 1f) < 0.001f)
        {
            return 1f;
        }

        else return value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VFX_Type { Spawn, Destroy, ChargeAttack, Stunned, Idle }

public class VFX_Manager : MonoBehaviour
{
    private float effectTime = 2.2f;
    public AnimationCurve fadeIn;
    [SerializeField] private GameObject[] VFX_objs;
    private ParticleSystem[] particleSystems;
    private float timer = 0;
    private int droneShaderProperty;
    private int ballShaderProperty;
    private int sphereShaderProperty;
    private int emissiveShaderProperty;
    private bool isPlayingVFX;
    private VFX_Type currentType = VFX_Type.Idle;
    private DroneType droneType;
    private Material[] droneMaterials;
    private Material ballMaterial;
    private Material sphereMaterial;
    private Renderer _droneRenderer;
    private AnimationCurve stunnedAnimation;
    [SerializeField] private Texture2D noiseMap;
    private float startExplosiveLightIntensity;
    private float endExplosiveLightIntensity;
    private float spawnTime;
    private float deathTime;
    private float chargeTime;
    private float stunTime;

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
        ballShaderProperty = Shader.PropertyToID("_Anim");
        sphereShaderProperty = Shader.PropertyToID("_AlphaIntensity");
        emissiveShaderProperty = Shader.PropertyToID("_EmissiveIntensity");

        _droneRenderer = GetComponentInParent<Renderer>();
        droneMaterials = _droneRenderer.materials;
        ballMaterial = VFX_objs[2].GetComponentInChildren<Renderer>().material;

        if (VFX_objs.Length > 3 && droneType == DroneType.TwoHits)
        {
            sphereMaterial = VFX_objs[3].GetComponent<Renderer>().material;
            //Debug.Log(_sphere_shield_renderer.name);
        }

        if (droneType == DroneType.Explosive)
        {
            startExplosiveLightIntensity = droneMaterials[2].GetFloat(emissiveShaderProperty);
            endExplosiveLightIntensity = startExplosiveLightIntensity * 2f; 
        }
    }

    public void SetTimers(float[] timers)
    {
        spawnTime = timers[0];
        deathTime = timers[1];
        chargeTime = timers[2];
        stunTime = timers[3];
    }

    private void ActivateParticleSystem(int type)
    {

        VFX_objs[type].SetActive(true);
                 
    }

    private void DeactivateAllVFX()
    {
        foreach(GameObject obj in VFX_objs)
        {
            if (obj.name != "SphereShield" && obj.name != "BombEmbers") 
            { 
                //Debug.Log(obj.name + " setting inactive");
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
        //Debug.Log(spawnTime + " " + deathTime + " " + chargeTime + " " + stunTime);

        timer = 0;
        isPlayingVFX = true;

        switch (type)
        {
            case VFX_Type.Spawn:

                effectTime = spawnTime;
                ActivateParticleSystem((int)type);
                break;

            case VFX_Type.Destroy:
                effectTime = deathTime;
                ActivateParticleSystem((int)type);
                ChangeNoiseMap();
                if(droneType == DroneType.Explosive)
                {
                    VFX_objs[2].SetActive(false);
                }
                break;

            case VFX_Type.ChargeAttack:
                if (droneType != DroneType.Explosive)
                {
                    effectTime = chargeTime;
                    ActivateParticleSystem((int)type);                  
                }

                else
                {
                    effectTime = GetComponentInParent<ExplosiveDrone>().ChargeExplosionDuration;
                    droneMaterials[0].SetFloat("_UseEmissive", 1f);
                }

                break;

            case VFX_Type.Stunned:
                effectTime = stunTime;
                VFX_objs[2].SetActive(false);
                break;

            default :
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
        switch (droneType)
        {
            case DroneType.Explosive:
                ChargeExplosiveLightVFX();
                break;

            default:
                //ChargeBallVFX();
                break;
        }
    }

    private void ChargeBallVFX()
    {
        float value = RoundEvaluation(fadeIn.Evaluate(Mathf.InverseLerp(0, effectTime, timer)));
        ballMaterial.SetFloat(ballShaderProperty, value);
    }

    private void ChargeExplosiveLightVFX()
    {
        float normalizedTime = Mathf.Clamp01(timer / effectTime);
        float value1 = Mathf.Lerp(startExplosiveLightIntensity, endExplosiveLightIntensity, normalizedTime);
        float value2 = RoundEvaluation(fadeIn.Evaluate(Mathf.InverseLerp(0, effectTime, timer))) * startExplosiveLightIntensity;

        droneMaterials[2].SetFloat(emissiveShaderProperty, value1);

        droneMaterials[0].SetFloat(emissiveShaderProperty, value2);
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

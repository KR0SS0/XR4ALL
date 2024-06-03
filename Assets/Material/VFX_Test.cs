using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_Test : MonoBehaviour
{
    public int sphereShaderProperty;
    public Material sphereMaterial;
    public float effectTime = 2f;
    public AnimationCurve fadeIn;
    public float timer = 0;
    public float delay = 1f;
    public float value = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        sphereShaderProperty = Shader.PropertyToID("_AlphaIntensity");
        sphereMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > delay)
        {
            value = 1 - fadeIn.Evaluate(1 - Mathf.InverseLerp(0, effectTime, timer));
            sphereMaterial.SetFloat(sphereShaderProperty, value);
            timer += Time.deltaTime;
        }
    }
}

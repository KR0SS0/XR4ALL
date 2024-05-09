using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_Striker_Bell : MonoBehaviour
{

    private AudioSource audioSource;
    [SerializeField] private AudioClip bellSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bellSound;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HitBell()
    {
        audioSource.Play();
    }
}

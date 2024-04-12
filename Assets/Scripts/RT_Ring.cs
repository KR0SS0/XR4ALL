using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RT_Ring : MonoBehaviour
{

    [SerializeField] private ColliderVisualizer visualizer;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxVelocityAllowedToScore = 0.03f;
    [Tooltip("0 = gains default score defined in RT_ScoreManager")]
    [SerializeField] private int scoreGain = 0;
    [SerializeField] private bool hasScored = false;

    [Header("Debug Values")]
    [SerializeField] private float velocity;


    // Start is called before the first frame update
    void Start()
    {
        if(rb == null) Debug.LogWarning("Ingen rb i " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        //For debugging
        velocity = rb.velocity.magnitude;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("RingPost"))
        {
            visualizer.solidColor = Color.blue;

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("RingPost"))
        {
            if (Mathf.Abs(rb.velocity.magnitude) < maxVelocityAllowedToScore)
            {
                visualizer.solidColor = Color.green;
                if(!hasScored)
                {
                    RT_ScoreManager.IncrementScore(scoreGain);
                    hasScored = true;
                }

            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RingPost"))
        {
            visualizer.solidColor = Color.red;
        }
    }
}

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
        velocity = rb.velocity.magnitude;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<RT_RingPost>(out _))
        {
            visualizer.solidColor = Color.blue;
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if(other.TryGetComponent<RT_RingPost>(out RT_RingPost ringPost))
        {
            if (Mathf.Abs(rb.velocity.magnitude) < maxVelocityAllowedToScore)
            {
                visualizer.solidColor = Color.green;
                if (!hasScored)
                {
                    RT_ScoreManager.Instance.IncrementScore(ringPost.GetScoreGain());
                    hasScored = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<RT_RingPost>(out _))
        {
            visualizer.solidColor = Color.red;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_Striker_HitCollider : MonoBehaviour
{

    private const string hammerTag = "HS_Hammer";
    [SerializeField] private HS_Striker_ScoreBushing scoreBushing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(hammerTag))
        {
            float inVelocity = other.gameObject.GetComponent<HS_Hammer>().CurrentVelocity;
            //Debug.Log("Hammer in with: " + inVelocity);
            scoreBushing.StartBounce(inVelocity);
        }
    }
}

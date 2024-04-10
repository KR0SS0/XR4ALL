using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class HS_Striker_ScoreBushing : MonoBehaviour
{

    private Rigidbody rb;
    //[SerializeField] private float upperForce;
    private bool loop = true;
    private float upperBound = 3.415f;

    [SerializeField] private HS_Striker_Bell striker_Bell;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
             
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckLowerBound();
        CheckUpperBound();

        /*
        if (loop)
        {
            StartCoroutine(Test());
        }
        */
    }

    IEnumerator Test()
    {
        loop = false;
        StartBounce(300f);
        yield return new WaitForSeconds(2f);
        loop = true;
    
    }

    private void CheckUpperBound()
    {
        if (gameObject.transform.localPosition.y >= upperBound)
        {
            float oldVelocity = rb.velocity.y;
            rb.velocity = new Vector3(0f, -oldVelocity, 0f);
            striker_Bell.HitBell();
        }
    }

    private void CheckLowerBound()
    {
        if(gameObject.transform.localPosition.y <= 0.1f)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
    }

    public void StartBounce(float incomingVelocity)
    {
        float upperForce = incomingVelocity * 75f;
        //float rand = Random.Range(0.5f, 1.2f);
        rb.useGravity = true;
        rb.AddForce(new Vector3(0.0f, upperForce, 0.0f));
    }
}

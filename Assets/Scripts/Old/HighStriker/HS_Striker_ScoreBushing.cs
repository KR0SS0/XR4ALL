
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class HS_Striker_ScoreBushing : MonoBehaviour
{

    private Rigidbody rb;
    //[SerializeField] private float upperForce;
    private bool loop = true;
    private float upperBound = 3.415f;
    private bool setNewScore = true;

    [SerializeField] private HS_Striker_Bell striker_Bell;
    [SerializeField] private HS_Striker_ScoreScreen scoreScreen;
    //private float forceModifier = 3.5f;

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
        MaxHeightReached();

        
        if (loop)
        {
            //StartCoroutine(Test());
        }
        
    }

   
    
    IEnumerator Test()
    {
        loop = false;
        StartBounce(Random.Range(1.5f, 7f));
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
        if(gameObject.transform.localPosition.y < 0f)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    public void StartBounce(float incomingVelocity)
    {
        setNewScore = true;

        float interpolation = Mathf.InverseLerp(1f, 7f, incomingVelocity);
        float upperForce = Mathf.Lerp(3f, 8f, interpolation);
        rb.useGravity = true;
        rb.AddForce(new Vector3(0.0f, upperForce, 0.0f), ForceMode.Impulse);
        //Debug.Log("Applied force: " + upperForce);

    }

    private void MaxHeightReached()
    {
        if(rb.velocity.y < 0f && setNewScore)
        {
            float score = Mathf.InverseLerp(0f, upperBound, transform.localPosition.y);
            setNewScore = false;
            scoreScreen.SetScore(score);
            //Debug.Log("return at: " + score);
        }
    }
}

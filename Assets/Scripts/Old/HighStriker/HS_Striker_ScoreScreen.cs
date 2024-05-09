using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_Striker_ScoreScreen : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetScore(float score)
    {
        int newScore = Mathf.RoundToInt(score * 100f);
        Debug.Log("New score: " + newScore);
    }

}

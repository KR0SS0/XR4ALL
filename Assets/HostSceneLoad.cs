using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostSceneLoad : MonoBehaviour
{
    private static GameObject sampleInstance;
    private void Awake()
    {
        if (sampleInstance != null)
            Destroy(sampleInstance);

        sampleInstance = gameObject;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        } 
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            SceneManager.LoadScene(1);
        } 
        else if ( Input.GetKey(KeyCode.Alpha3))
        {
            SceneManager.LoadScene(2);
        } 
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    public int[] ScenesToLoadBuildIndex;

    private void Start()
    {
        foreach(int i in ScenesToLoadBuildIndex)
        {
            SceneManager.LoadScene(i, LoadSceneMode.Additive);
        }
    }
}

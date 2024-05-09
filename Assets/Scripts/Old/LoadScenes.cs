using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{

    public static LoadScenes Instance { get; private set; }

    public int[] ScenesToLoadBuildIndex;

    private void Awake()
    {
        Instance = this;
    }

    public void StartLoadScenes(GameObject destroy)
    {
        foreach (int i in ScenesToLoadBuildIndex)
        {
            SceneManager.LoadScene(i, LoadSceneMode.Additive);
        }
        Destroy(destroy);
    }
}

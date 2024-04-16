using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    private AudioSource source;
    private bool isPressed = false;

    public float pressDistance = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed)
        {
            Debug.Log("Button pressed");
            button.transform.position = new Vector3(button.transform.position.x, button.transform.position.y - pressDistance, button.transform.position.z);
            onPress.Invoke();
            source.PlayOneShot(source.clip);
            isPressed = true;
            StartCoroutine(Unpress());
        }
    }

    private IEnumerator Unpress()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("Button released");
        button.transform.position = new Vector3(button.transform.position.x, button.transform.position.y + pressDistance, button.transform.position.z);
        onRelease.Invoke();
        isPressed = false;
        
    }

}

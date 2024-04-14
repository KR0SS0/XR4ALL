using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_Duck : MonoBehaviour
{
    private Vector3 originalRotation = Vector3.zero;
    private Vector3 deadRotation = Vector3.forward * 90f;
    private bool killDuck = false;
    private bool respawnDuck = false;
    [SerializeField] private float rotationSpeed = 45f;
     

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (killDuck)
        {
            FlipDuck();
            Debug.Log("Rotation z: " + gameObject.transform.rotation.eulerAngles.z);
            if (gameObject.transform.rotation.eulerAngles.z >= 90f)
            {
                Debug.Log("Max rotation reached");
                StartCoroutine(DeadTimer());
            }
        }

        if (respawnDuck)
        {
            ResetDuck();
            if(gameObject.transform.rotation.eulerAngles.z <= 0f)
            {
                respawnDuck = false;
            }
        }
    }

    private void FlipDuck()
    {

        Quaternion targetRotation = Quaternion.Euler(deadRotation);

        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        gameObject.transform.rotation = newRotation;
    }

    IEnumerator DeadTimer()
    {
        killDuck = false;
        yield return new WaitForSeconds(3f);
        respawnDuck = true;
        

    }

    IEnumerator DeadTimer(float time)
    {
        yield return new WaitForSeconds(time);
    }

    private void ResetDuck()
    {
        Quaternion targetRotation = Quaternion.Euler(originalRotation);

        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        gameObject.transform.rotation = newRotation;
    }

    public void KillDuck()
    {
        killDuck = true;
    }


}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SD_Bullet : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float velocity;
    private const string duckTag = "SD_Duck";
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartMovement();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(duckTag))
        {
            Debug.Log("Duck bullet hit");
            collision.gameObject.GetComponent<SD_Duck>().KillDuck();
            StartCoroutine(DelayDestroy());
        }
    }

    private void StartMovement()
    {
        // Quaternion newDirection = gameObject.transform.rotation.normalized;
        // Debug.Log("Game Object: " + newDirection);
        direction = transform.forward;
        rb.velocity = direction * velocity;
        //Debug.Log("Direction vector: " + direction);

        
        
    }

    IEnumerator DelayDestroy()
    {
        rb.velocity = - direction * velocity * 0.4f;
        rb.useGravity = true;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}

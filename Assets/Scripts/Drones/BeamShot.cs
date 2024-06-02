using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamShot : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float velocity = 1f;
    private Vector3 direction;
    private const string playerTag = "Player";
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartMovement();
        StartCoroutine(LifeTime());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartMovement()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Set the object's forward direction to the direction towards the player
        transform.forward = directionToPlayer;
        //direction = transform.forward;
        rb.velocity = transform.forward * velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            Debug.Log("Bullet hit Player");
            Destroy(gameObject);
        }
    }

    private IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_Bullet : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float velocity;
    private const string duckTag = "SD_Duck";
    private Vector3 direction;

     private SD_GameAndScoreManager game;

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<SD_GameAndScoreManager>();
        rb = GetComponent<Rigidbody>();
        StartMovement();
        StartCoroutine(DelayGravity()); 
        StartCoroutine(DelayDestroy());
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
            game.IncrementScore();
            collision.gameObject.GetComponent<SD_Duck>().KillDuck();
            rb.velocity = -direction * velocity * 0.4f;
            rb.useGravity = true;
        }
    }

    private void StartMovement()
    {
        direction = transform.forward;
        rb.velocity = direction * velocity;       
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    IEnumerator DelayGravity()
    {
        yield return new WaitForSeconds(1.2f);
        rb.useGravity = true;
    }
}

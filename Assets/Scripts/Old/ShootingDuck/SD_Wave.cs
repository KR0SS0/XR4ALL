using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_Wave : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f; 
    [SerializeField] private float yAxisRadius = 0.15f; 
    [SerializeField] private float zAxisRadius = 0.5f;
    [SerializeField] private bool changeDirection = false;

    private float angle = 0f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float y, z;

        if(changeDirection)
        {
            y = startPosition.y + Mathf.Sin(angle) * yAxisRadius;
            z = startPosition.z + Mathf.Cos(angle) * zAxisRadius;
        }

        else
        {
            y = startPosition.y + Mathf.Sin(angle) * yAxisRadius * -1f;
            z = startPosition.z + Mathf.Cos(angle) * zAxisRadius * -1f;
        }

        // Update the position of the GameObject
        transform.localPosition = new Vector3(transform.localPosition.x, y, z);
        
        angle += Time.fixedDeltaTime * speed;       
        angle = Mathf.Repeat(angle, Mathf.PI * 2);
    }
}

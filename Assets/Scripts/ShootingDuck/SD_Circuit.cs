using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_Circuit : MonoBehaviour
{

    [SerializeField] private Transform leftBound;
    [SerializeField] private Transform rightBound;
    [SerializeField] private GameObject duck;
    //private SD_Duck[] ducks;
    [SerializeField] private int duckAmount;
    [SerializeField] private float stageHeight;
    [SerializeField] private float stageWidth;
    private int points;

    // Start is called before the first frame update
    void Start()
    {

        if(duckAmount == 0)
        {
            duckAmount = 6;
        }

        int firstRow = duckAmount / 2;
        int secondRow = firstRow + duckAmount % 2;

        IntantiateDucks(firstRow, secondRow);

    }

    private void IntantiateDucks(int firstRow, int secondRow)
    {

        for(int i = -1; i < firstRow - 1; i++)
        {

            float yTargetPosition = transform.localPosition.y - stageHeight / 3;
            float zTargetPosition = transform.localPosition.z + stageWidth / (firstRow + 1) * i;

            GameObject newDuck = Instantiate(duck, new Vector3(transform.position.x, yTargetPosition, zTargetPosition),
                transform.rotation, gameObject.transform);

            Vector3 localPosition = newDuck.transform.localPosition;
            newDuck.transform.localPosition = new Vector3(0.1f * i, localPosition.y, localPosition.z);

            newDuck.GetComponent<SD_Duck>().HorizontalPhase = RandomPhase();
        }


        for (int i = -1; i < secondRow - 1; i++)
        {
            float yTargetPosition = transform.localPosition.y + stageHeight / 3;
            float zTargetPosition = transform.localPosition.z + stageWidth / (firstRow + 1) * i;

            GameObject newDuck = Instantiate(duck, new Vector3(transform.position.x, yTargetPosition, zTargetPosition), 
                transform.rotation, gameObject.transform);

            Vector3 localPosition = newDuck.transform.localPosition;
             newDuck.transform.localPosition = new Vector3(0.1f * i - 0.2f, localPosition.y, localPosition.z);

            newDuck.GetComponent<SD_Duck>().HorizontalPhase = RandomPhase();
        }

    }

    private float RandomPhase()
    {
        return Random.Range(-Mathf.PI / 2, Mathf.PI / 2);
    }

    public void StartDeadTimer(GameObject duck, float respawnTime)
    {
        StartCoroutine(RespawnTimer(duck, respawnTime));
    }

    IEnumerator RespawnTimer(GameObject duck, float respawnTime)
    {
        Debug.Log("Dead courutine started");
        duck.SetActive(false);
        UpdateScore();
        yield return new WaitForSeconds(respawnTime);
        duck.SetActive(true);
        duck.GetComponent<SD_Duck>().Respawn();
    }

    private void UpdateScore()
    {
        points++;
        Debug.Log("Points: " + points);
    }
}

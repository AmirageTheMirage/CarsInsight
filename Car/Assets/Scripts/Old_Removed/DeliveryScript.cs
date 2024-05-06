using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryScript : MonoBehaviour
{
    public int DeliveriesMade =1;
    public GameObject DeliveryObject;
    public float MinX = 0;
    public float MinZ = 0;
    public float MaxX = 500;
    public float MaxZ = 500;
    public float LerpSpeed = 10f;
    void Start()
    {
        DeliveryObject.transform.position = new Vector3(Random.Range(MinX, MaxX), -0.48f, Random.Range(MinZ, MaxZ));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("CarObject"))
        {
            DeliveriesMade += 1;
            Vector3 newPosition = new Vector3(Random.Range(MinX, MaxX), -0.48f, Random.Range(MinZ, MaxZ));
            DeliveryObject.transform.position = Vector3.Lerp(DeliveryObject.transform.position, newPosition, LerpSpeed);
        }
    }
}

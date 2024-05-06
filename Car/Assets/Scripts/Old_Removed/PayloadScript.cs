using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayloadScript : MonoBehaviour
{
    public GameObject StrikeObject;
    public Rigidbody rb;
    public float ForceE = 100f;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        //Vector3 directionToTarget = StrikeObject.transform.position - transform.position;
        //transform.forward = directionToTarget.normalized;
        Vector3 randomDirection = Random.insideUnitSphere;
        if (randomDirection.y < 0)
        {
            randomDirection.y = Random.Range(0f, 90f);
        }
        rb.AddForce(randomDirection * ForceE, ForceMode.Impulse);
        //Destroy(gameObject, 1f);

    }

    
}

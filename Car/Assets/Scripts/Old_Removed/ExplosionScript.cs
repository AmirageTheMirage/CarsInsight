using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public GameObject ExplosionEffect;
  

    void OnCollisionEnter(Collision collision)
    {
        
            GameObject instance = Instantiate(ExplosionEffect, collision.contacts[0].point, Quaternion.identity);
            instance.SetActive(true);
            Destroy(instance, 0.1f);
            Destroy(gameObject);
        
    }
}

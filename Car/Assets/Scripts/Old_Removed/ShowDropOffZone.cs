using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDropOffZone : MonoBehaviour
{
    public GameObject DropOffZone;
    public GameObject Arrow;
    public float LerpSpeed = 10f;
    private Quaternion TarRot;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = DropOffZone.transform.position - Arrow.transform.position;
        direction.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(direction);
        TarRot = rotation;
        Arrow.transform.rotation = Quaternion.Lerp(Arrow.transform.rotation, TarRot, Time.deltaTime * LerpSpeed);
    }
}

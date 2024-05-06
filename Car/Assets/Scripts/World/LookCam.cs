using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCam : MonoBehaviour
{
    public GameObject LookAtObject;
    public bool Active = false;
    void Update()
    {
        if (LookAtObject != null && Active)
        {
            gameObject.transform.LookAt(LookAtObject.transform.position);
        }
    }
}

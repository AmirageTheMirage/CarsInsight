using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarAway : MonoBehaviour
{

    public GameObject Car;
    
    public CreateTerrainv2 TerrainScript;
    
    void Start()
    {
        InvokeRepeating("CheckForDestroy", 2f, 2f);
    }

    public void CheckForDestroy()
    {
        if (Vector3.Distance(Car.transform.position, gameObject.transform.position) >= 3000f) //3000f is Sinced with CreateTerrainv2.cs
        {
            //TerrainScript.ListX.Remove(MyIndex2);
            TerrainScript.ListX[int.Parse(gameObject.name) - 1] = 0f;
            TerrainScript.ListZ[int.Parse(gameObject.name) - 1] = 0f;
            //TerrainScript.ListZ.Remove(MyIndex2);
            Destroy(gameObject);
        }
    }
}

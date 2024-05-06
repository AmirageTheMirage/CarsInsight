using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawning : MonoBehaviour
{
    public GameObject CubeToSpawn;
    public GameObject CubeParent;
    public float Range = 5f;
    public float Cooldown = 0.1f;

    [Header("Can't be activated later!")]
    public bool Spawn = false;

    void Start()
    {
        if (Spawn)
        {
            InvokeRepeating("SpawnCube", 0f, Cooldown);
        }
    }

    // Update is called once per frame
    public void SpawnCube()
    {
        Vector3 MyVector3Random = new Vector3(transform.position.x + Random.Range(-1 * Range, Range), transform.position.y, transform.position.z + Random.Range(-1 * Range, Range));
        GameObject spawnedCube = Instantiate(CubeToSpawn, MyVector3Random, Quaternion.identity);
        spawnedCube.SetActive(true);
        spawnedCube.transform.parent = CubeParent.transform;
    }
}

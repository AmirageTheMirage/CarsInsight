using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACSpawner : MonoBehaviour
{
    // AC = Angry Cube
    public float Rate = 0f; //Per Second
    public GameObject TargetCube;
    public GameObject IndependentCubes;
    public int Amount = 10;
    public float MaxRange;
    public float MinRange;
    public GameObject Car;
    public float RangeCar = 500f; //We do not want the car to see the spawning
    private int Debug_CounterBatch;
    private int Debug_CounterCubes;
    [Space]
    public Vector3 SpawnLocation;
    [Space]
    public bool Spawn = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Rate != 0f)
        {
            float ran = Random.Range(0f, Rate / Time.fixedDeltaTime);
            if (ran <= 1f)
            {
                Spawn = true;
            }
        }
        if (Spawn)
        {
            SpawnCycle(1, Amount);

        }
    }

    public void SpawnC(int Am2)
    {
        
        for (int i = 0; i < Am2; i++)
        {
            GameObject New = Instantiate(TargetCube);
            New.transform.position = new Vector3(SpawnLocation.x + Random.Range(-5f, 5f), SpawnLocation.y, SpawnLocation.z + Random.Range(-5f, 5f));
            New.transform.parent = IndependentCubes.transform;
            New.SetActive(true);
            JumpingCubes JScript = New.GetComponent<JumpingCubes>();
            JScript.UseDestroyTimer = true;
            Debug_CounterCubes += 1;
        }
    }
    public void SpawnCycle(int OneBatchAmount, int CubesInOneBatch)
    {
        Debug_CounterBatch = 0;
        Debug_CounterCubes = 0;
        for (int i = 0; i < OneBatchAmount; i++)
        {
            Spawn = false;
            float ran = Random.Range(0f, 2f);
            //SpawnLocation = new Vector3(99999f, 99999f, 99999f);
            SpawnLocation.x = Random.Range(transform.position.x - MaxRange, transform.position.x + MaxRange);
            SpawnLocation.z = Random.Range(transform.position.z - MaxRange, transform.position.z + MaxRange);
            SpawnLocation.y = 90f;
            for (int e = 0; e < 50f; e++)
            {
                if (Vector3.Distance(SpawnLocation, transform.position) < MinRange)
                {
                    SpawnLocation.x = Random.Range(transform.position.x - MaxRange, transform.position.x + MaxRange);
                    SpawnLocation.z = Random.Range(transform.position.z - MaxRange, transform.position.z + MaxRange);
                    SpawnLocation.y = 90f;
                }
            }

            //while (Vector3.Distance(Car.transform.position, SpawnLocation) < RangeCar){
            //    SpawnLocation.x = Random.Range(transform.position.x - MaxRange, transform.position.x + MaxRange);
            //    SpawnLocation.z = Random.Range(transform.position.z - MaxRange, transform.position.z + MaxRange);
            //    SpawnLocation.y = 90f;
            //}
            SpawnC(CubesInOneBatch);
            Debug_CounterBatch += 1;
        }
        Debug.Log("Spawned " + Debug_CounterCubes + " AttackingCubes in " + Debug_CounterBatch + " batches.");
        
    }
}

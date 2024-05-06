using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public DayNightCycle DayScr;
    public ACSpawner ACScr;
    [Space]
    public int ACAmountMin;
    public int ACAmountMax;
    public int BatchesMin;
    public int BatchesMax;
    void Start()
    {
        
    }

    public void SpawnNightEnemies()
    {
        int Rand = Mathf.RoundToInt(Random.Range(ACAmountMin, ACAmountMax));
        int RandBatch = Mathf.RoundToInt(Random.Range(BatchesMin, BatchesMax));
        ACScr.SpawnCycle(RandBatch, Rand);
    }
}

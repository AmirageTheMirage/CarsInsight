using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Material DayMat;
    public Material NightMat;
    public GameObject AroundWalls;
    public Light DirLight;
    public SpawnEnemies SpawnerScr;
    public GracePeriod GrScr;
    [Space]
    public bool Day = true;
    public bool CycleAllowed = true;
    public float SecondsToChange;
    public float MinSecondsToChange;
    public float MaxSecondsToChange;
    public bool Change;
    private float DirIntens;

    void Start()
    {
        SecondsToChange = Random.Range(MinSecondsToChange, MaxSecondsToChange);
        DirIntens = DirLight.intensity;
        MakeDay();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CycleAllowed)
        {
            SecondsToChange -= Time.fixedDeltaTime;
            if (SecondsToChange <= 0f)
            {
                SecondsToChange = Random.Range(MinSecondsToChange, MaxSecondsToChange);
                Change = true;
            }
        }
        if (Change)
        {
            Change = false;
            if (Day)
            {
                MakeNight();   
            } else
            {
                MakeDay();
            }
        }
    }


    void MakeDay()
    {
        DirLight.intensity = DirIntens;
        Day = true;
        RenderSettings.skybox = DayMat;
        RenderSettings.ambientIntensity = 1f;
        RenderSettings.fog = false;
        AroundWalls.SetActive(false);
    }

    void MakeNight()
    {
        if (GrScr.Grace <= 0f)
        {
            SpawnerScr.SpawnNightEnemies();
        }
        DirLight.intensity = 0.1f;
        Day = false;
        RenderSettings.skybox = NightMat;
        RenderSettings.ambientIntensity = 0.1f;
        RenderSettings.fog = true;
        AroundWalls.SetActive(true);
    }
}

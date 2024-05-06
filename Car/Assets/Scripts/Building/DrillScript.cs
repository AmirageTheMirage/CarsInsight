using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillScript : MonoBehaviour
{
    public StorageScript StorScr;
    [Space]
    public DayNightCycle DayScr;
    public bool IsItDay;
    public bool CanDrill;
    public bool IsDrillAtAStart;
    public bool IsDrillDown;
    public EditProperties EditScr;
    public ParticleSystem WorkingParts;
    
    [Space]
    //This Part is for Appearance
    public GameObject DrillHead;
    private Vector3 DrillStartingPosition;
    private Vector3 DrillTargetPosition;
    
    public float DrillLerpSpeed = 1f;
    public float DrillIdleSpeed = 100f;
    public float DrillRotationSpeed = 0f;
    public float MaxDrillSpeed = 2000f;
    public float DrillYieldPerSecond = 0f;
    public float MaxDrillYieldPerSecond;
    public float DrillHeadOffset = 3.75f;
    public float TotalMined = 0f;
    
    [Space]
    public AudioSource DrillAudio;
    public AudioSource MiningAudio;
    private bool ResAudio = false;
    private bool DownDrillAudio = false;
    public float DrillAudioTargetVol = 0.4f;
    public float DrillAudioMinVol = 0.2f;
    public float DrillAudioMinPitch = 0.2f;
    public float DrillAudioTargetPitch = 1f;
    public float MiningAudioTargetVol = 0.8f;
    public float MiningAudioTargetPitch = 1.0f;

    public float DrillAudioVol = 0f;
    public float DrillAudioPitch = 0f;
    public float MiningAudioVol = 0f;
    public float MiningAudioPitch = 0f;
    void Start()
    {
        WorkingParts.Pause();
        DrillAudio.Play();
        MiningAudio.Stop();
        IsDrillAtAStart = true;
        IsDrillDown = false;
        DrillStartingPosition = DrillHead.transform.position;
        DrillTargetPosition = DrillStartingPosition;
        DrillTargetPosition += Vector3.up * -DrillHeadOffset;
        IsItDay = DayScr.Day;
        CanDrill = false;
        DrillYieldPerSecond = 0f;
        DownDrillAudio = false;

    }

    void FixedUpdate()
    {
        IsItDay = DayScr.Day;
        EditScr.Drill_MiningRate = DrillYieldPerSecond;
        EditScr.Drill_TotalMined = TotalMined;


        
        if (StorScr == null) //StorScript only exists here if there is something to be able to be filled
        {
            if (IsDrillAtAStart)
            {
                GetStorScript();
            }
        }
        else
        {
            if (StorScr.StorageValue + 0.4f >= StorScr.StorageMax || !StorScr.ExportingEnabled)
            {
                StorScr = null;
            }
        }


        if (StorScr && IsItDay && EditScr.StatusActive)
        {
            CanDrill = true;
        } else
        {
            CanDrill = false;
            ResAudio = false;
        }

       
    }

    void Update()
    {
        //ONECYCLE of DrillAppearance
        DrillHead.transform.Rotate(Vector3.up, DrillRotationSpeed * Time.deltaTime);
        DrillAudio.pitch = DrillAudioPitch;
        DrillAudio.volume = DrillAudioVol;
        MiningAudio.pitch = MiningAudioPitch;
        MiningAudio.volume = MiningAudioVol;
        if (CanDrill)
        {
            if (!ResAudio)
            {
                ResAudio = true;
                //DrillAudio.Play();
                MiningAudio.Play();
                DrillAudio.volume = DrillAudioMinVol;
                DrillAudio.pitch = DrillAudioMinPitch;
                MiningAudio.volume = 0f;

                DrillAudioVol = DrillAudioMinVol;
                DrillAudioPitch = DrillAudioMinPitch;
                MiningAudioVol = 0f;
                MiningAudioPitch = 0f;

                }

            //Audio Update
            if (DrillAudioVol < DrillAudioTargetVol)
            {
                DrillAudioVol += 0.1f * Time.deltaTime;
            }
            if (DrillAudioPitch < DrillAudioTargetPitch)
            {
                DrillAudioPitch += 0.1f * Time.deltaTime;
            }

            IsDrillAtAStart = false;
            if (DrillRotationSpeed < MaxDrillSpeed)
            {
                DrillRotationSpeed += 500f * Time.deltaTime;
            }
            if (Vector3.Distance(DrillHead.transform.position, DrillTargetPosition) > 0.1f)
            {
                DrillHead.transform.position = Vector3.Lerp(DrillHead.transform.position, DrillTargetPosition, DrillLerpSpeed * Time.deltaTime);
            } else
            {
                
                    IsDrillDown = true;
                    WorkingParts.Play();
                
            }
            if (Vector3.Distance(DrillHead.transform.position, DrillTargetPosition) < 0.3f)
            {
                DownDrillAudio = true;

            } else
            {
                DownDrillAudio = false;
            }

            if (DownDrillAudio)
            {
                if (MiningAudioVol < MiningAudioTargetVol)
                {
                    MiningAudioVol += 0.5f * Time.deltaTime;
                }
                if (MiningAudioPitch < MiningAudioTargetPitch)
                {
                    MiningAudioPitch += 0.1f * Time.deltaTime;
                }
            }

            if (IsDrillDown)
            {
                if (StorScr)
                {
                    if (DrillYieldPerSecond < MaxDrillYieldPerSecond)
                    {
                        DrillYieldPerSecond += 2f * Time.deltaTime;
                    } else if (DrillYieldPerSecond > MaxDrillYieldPerSecond)
                    {
                        DrillYieldPerSecond = MaxDrillYieldPerSecond;
                    }
                    StorScr.StorageValue += DrillYieldPerSecond * Time.deltaTime;
                    TotalMined += DrillYieldPerSecond * Time.deltaTime;
                }
            }
            ////Bobbing
            //float t = Mathf.PingPong(Time.time * DrillLerpSpeed, 1f); // Calculate the interpolation factor
            //DrillHead.transform.position = Vector3.Lerp(DrillStartingPosition, DrillTargetPosition, t);
        } else
        {
            DownDrillAudio = false;


            if (DrillAudioVol > DrillAudioMinVol)
            {
                DrillAudioVol -= 0.1f * Time.deltaTime;
            }
            if (DrillAudioPitch > DrillAudioMinPitch)
            {
                DrillAudioPitch -= 0.1f * Time.deltaTime;
            }

            if (MiningAudioVol > 0f)
            {
                MiningAudioVol -= 0.5f * Time.deltaTime;
            }
            if (MiningAudioPitch > 0f)
            {
                MiningAudioPitch -= 0.1f * Time.deltaTime;
            }



            DrillYieldPerSecond = 0f;
            WorkingParts.Pause();
            IsDrillDown = false;
            if (DrillRotationSpeed > DrillIdleSpeed)
            {
                DrillRotationSpeed -= 500f * Time.deltaTime;
            }
            if (Vector3.Distance(DrillHead.transform.position, DrillStartingPosition) > 0.1f)
            {
                DrillHead.transform.position = Vector3.Lerp(DrillHead.transform.position, DrillStartingPosition, DrillLerpSpeed * Time.deltaTime);
            } else
            {
                IsDrillAtAStart = true;
            }
        }

       
    }




    void GetStorScript()
    {
        //Debug.Log("GameObject " + gameObject.name + " is searching for a Storage.");
        GameObject[] AllStorages = GameObject.FindGameObjectsWithTag("Camp_Storage");
        if (AllStorages != null && AllStorages.Length > 0) //Are there even any Storages? Makes rest obsolete
        {
            //float shortestDistance = Mathf.Infinity;
            Vector3 currentPos = gameObject.transform.position;
            float MaxRange = 50f;
            
                //Find new Storage, if possible
                int Rand = Mathf.FloorToInt(Random.Range(0f, (float)AllStorages.Length));
                GameObject NewObject = AllStorages[Rand];
                if (Vector3.Distance(NewObject.transform.position, currentPos) <= MaxRange)
                {
                    StorageScript StScrTemp = NewObject.GetComponent<StorageScript>();
                    if (StScrTemp != null && StScrTemp.StorageValue + 0.4f <= StScrTemp.StorageMax && StScrTemp.ExportingEnabled)
                    {
                        
                            StorScr = StScrTemp;
                            //Debug.Log("GameObject " + gameObject.name + " found Storage.");
                        
                    }
                }
            
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenScript : MonoBehaviour
{
    private Renderer SphereFadeRender;
    public GameObject Sphere;
    public Material NotWorking;
    public Material Working;
    public ParticleSystem PartSys;
    public HealthSystem TargetHPSys;
    public float Rate = 5f;
    private float MaxHP;
    public bool CanBeActive = true;
    public bool Active = false;
    public float Offset = 1f;
    private Vector3 InitialVector3;
    private Vector3 TargetVector3;
    private bool LerpToTarget = true;
    public float LerpSpeed = 2f;
    public StorageScript StorScr;
    private float MaxRangeStorage = 50f;
    public float OneRegenUse = 1f; //Use this many score from storage
    public EditProperties MyEditProp;
    public int Regen_Mode;
    public int Regen_Priority;
    private int Regen_ModeBefore;
    private int Regen_PriorityBefore;
    
    //public bool CanActive = true;
    void Start()
    {
        
        Regen_Priority = MyEditProp.Regenerator_Priority;
        Regen_Mode = MyEditProp.Regenerator_Mode;
        Regen_ModeBefore = Regen_Mode;


        SphereFadeRender = Sphere.GetComponent<Renderer>();
        InitialVector3 = Sphere.transform.localPosition;
        TargetVector3 = new Vector3(Sphere.transform.localPosition.x, Sphere.transform.localPosition.y + Offset, Sphere.transform.localPosition.z);
        SphereFadeRender.material = NotWorking;
        Active = false;
        PartSys.Stop();
        GetTargetHPSys();
        
    }


    void Update()
    {
        
        Regen_Priority = MyEditProp.Regenerator_Priority;
        Regen_Mode = MyEditProp.Regenerator_Mode;
        CanBeActive = MyEditProp.StatusActive;
        if (Regen_Priority != Regen_PriorityBefore)
        {
            Regen_PriorityBefore = Regen_Priority;
            //Debug.Log("Prio has changed!");
            GetTargetHPSys();
        }
        if (Regen_Mode != Regen_ModeBefore)
        {
            Regen_ModeBefore = Regen_Mode; 
            //Debug.Log("Mode has changed!");
            GetTargetHPSys();
        }
        if (StorScr == null)
        {
            GetStorScript();
        } else
        {
            if (Vector3.Distance(StorScr.gameObject.transform.position, gameObject.transform.position) > MaxRangeStorage || StorScr.StorageValue <= (OneRegenUse * Rate * Time.deltaTime))
            {
                StorScr = null;
                GetStorScript();
            }
        }
        if (StorScr != null)
        {
            if (TargetHPSys == null) //If ex. the Shield is destroyed we can't "repair" it anymore
            {
                Active = false;
                PartSys.Stop();
                SphereFadeRender.material = NotWorking;
                Sphere.transform.localPosition = Vector3.Lerp(Sphere.transform.localPosition, InitialVector3, LerpSpeed * Time.deltaTime);
                if (CanBeActive)
                {
                    GetTargetHPSys();
                }
            }
            else
            {
                if (CanBeActive)
                {
                    if (TargetHPSys.Health < MaxHP)
                    {
                        if (Active == false)
                        {
                            PartSys.Play();
                            Active = true;
                            SphereFadeRender.material = Working;
                        }

                        TargetHPSys.Health += Rate * Time.deltaTime; //Regen
                        if (StorScr == null)
                        {
                            Debug.LogWarning("StorageScript is assigned, but does not exist."); //This should never come
                        }
                        else
                        {
                            if (StorScr.StorageValue >= (OneRegenUse * Rate * Time.deltaTime) && StorScr.ExportingEnabled)
                            {
                                StorScr.StorageValue -= (OneRegenUse * Rate * Time.deltaTime);
                            }
                            else
                            {
                                StorScr = null; //Cant fulfill Storage Need, try another storage
                            }
                        }

                        if (LerpToTarget)
                        {
                            Sphere.transform.localPosition = Vector3.Lerp(Sphere.transform.localPosition, TargetVector3, LerpSpeed * Time.deltaTime);

                            if (Vector3.Distance(Sphere.transform.localPosition, TargetVector3) <= 0.1f)
                            {
                                LerpToTarget = false;
                            }
                        }
                        else
                        {
                            Sphere.transform.localPosition = Vector3.Lerp(Sphere.transform.localPosition, InitialVector3, LerpSpeed * Time.deltaTime);
                            if (Vector3.Distance(Sphere.transform.localPosition, InitialVector3) <= 0.1f)
                            {
                                LerpToTarget = true;
                            }
                        }


                    }
                    else
                    {
                        if (CanBeActive)
                        {
                            //Debug.Log("This GameObject is fully healed: " + MaxHP + TargetHPSys.Health);
                            //TargetHPSys = null;
                            GetTargetHPSys(); //Find new GameObject to "annoy" (regen I mean)
                            
                                Active = false;
                                PartSys.Stop();
                                SphereFadeRender.material = NotWorking;
                            
                            Sphere.transform.localPosition = Vector3.Lerp(Sphere.transform.localPosition, InitialVector3, LerpSpeed * Time.deltaTime);
                        }
                    }
                } else
                {
                    Active = false;
                    PartSys.Stop();
                    SphereFadeRender.material = NotWorking;

                    Sphere.transform.localPosition = Vector3.Lerp(Sphere.transform.localPosition, InitialVector3, LerpSpeed * Time.deltaTime);
                }
            }
        } else //StorScr = null
        {
            
                Active = false;
                PartSys.Stop();
                SphereFadeRender.material = NotWorking;
            
                Sphere.transform.localPosition = Vector3.Lerp(Sphere.transform.localPosition, InitialVector3, LerpSpeed * Time.deltaTime);
        }
    }
    void GetTargetHPSys()
    {
       // Debug.LogWarning("One RegenScript is searching for an HP System. Mode: " + Regen_Mode + " - " + Regen_Priority);
        if (Regen_Mode == 1) // Shield
        {
            GetShieldTargetHP();
        } else if (Regen_Mode == 2) //All other Objects
        {
            GetFriendlyTargetHP();
        } else if (Regen_Mode == 3)
        {
            if (Regen_Priority == 1) //Mode: Random
            {
                float Rand = Random.Range(0f, 2f);
                if (Rand <= 1f)
                {
                    GetShieldTargetHP();
                    if (TargetHPSys == null)
                    {
                        GetFriendlyTargetHP();
                    }
                }
                else
                {
                    GetFriendlyTargetHP();
                    if (TargetHPSys == null)
                    {
                        GetShieldTargetHP();
                    }
                }
            }
            else if (Regen_Priority == 2) //Mode: Shield First
            {
                GetShieldTargetHP();
                if (TargetHPSys == null)
                {
                    GetFriendlyTargetHP();
                }

            } else if (Regen_Priority == 3) //Mode: Friendly First
            {
                GetFriendlyTargetHP();
                if (TargetHPSys == null)
                {
                    GetShieldTargetHP();
                }
            }
        }

        
    }

    void GetStorScript()
    {
        //Debug.Log("GameObject " + gameObject.name + " is searching for a Storage.");
        GameObject[] AllStorages = GameObject.FindGameObjectsWithTag("Camp_Storage");
        if (AllStorages != null && AllStorages.Length > 0)
        {
            //float shortestDistance = Mathf.Infinity;
            Vector3 currentPos = gameObject.transform.position;
            float MaxRange = 50f;
            if (StorScr == null || StorScr.StorageValue <= 1f || !StorScr.ExportingEnabled)
            {
                //Find new Storage, if possible
                int Rand = Mathf.FloorToInt(Random.Range(0f, (float)AllStorages.Length));
                GameObject NewObject = AllStorages[Rand];
                if (Vector3.Distance(NewObject.transform.position, currentPos) <= MaxRange) {
                    StorageScript StScrTemp = NewObject.GetComponent<StorageScript>();
                    if (StScrTemp != null)
                    {
                        if (StScrTemp.StorageValue > (OneRegenUse * Rate * Time.deltaTime) && StScrTemp.ExportingEnabled)
                        {
                            StorScr = StScrTemp;
                            //Debug.Log("GameObject " + gameObject.name + " found Storage.");
                        }
                    }
                }
            }
        }

    }

    void GetShieldTargetHP()
    {
        GameObject[] AllShields = GameObject.FindGameObjectsWithTag("Shield");
        if (AllShields != null)
        {
            Vector3 currentPos = gameObject.transform.position;
            float MaxRange = MaxRangeStorage;
            GameObject TargetObject = AllShields[Mathf.RoundToInt(Random.Range(0f, (float)(AllShields.Length - 1)))];
            //Debug.Log("Shield: " + TargetObject.name);
                float distance = Vector3.Distance(TargetObject.transform.position, currentPos);
                if (distance <= MaxRange)
                {
                    TargetHPSys = TargetObject.GetComponent<HealthSystem>();
                if (TargetHPSys != null)
                {

                    if (TargetHPSys.Health >= TargetHPSys.MaxHealth)
                    {
                        TargetHPSys = null;
                        //Debug.Log("Health is already maxxed.");
                    }
                    else
                    {
                        MaxHP = TargetHPSys.MaxHealth;
                        //Debug.Log("One RegenScript found a TargetHPSys");
                    }

                }
            }

            
        }
    }


    void GetFriendlyTargetHP()
    {
        GameObject[] AllFriendly = GameObject.FindGameObjectsWithTag("Friendly");
        if (AllFriendly != null)
        {
            Vector3 currentPos = gameObject.transform.position;
            float MaxRange = MaxRangeStorage;
            GameObject TargetObject = AllFriendly[Mathf.RoundToInt(Random.Range(0f, (float)(AllFriendly.Length - 1)))];
            if (TargetObject.name != "Center") //Can't heal Center
            {
                    string myName = gameObject.name;
                    if (TargetObject.name != myName) //Can't heal self
                    {

                        float distance = Vector3.Distance(TargetObject.transform.position, currentPos);
                        if (distance <= MaxRange)
                        {
                            TargetHPSys = TargetObject.GetComponent<HealthSystem>();
                        if (TargetHPSys != null)
                        {

                            if (TargetHPSys.Health >= TargetHPSys.MaxHealth)
                            {
                                TargetHPSys = null;
                                //Debug.Log("Health is already maxxed.");
                            }
                            else
                            {
                                MaxHP = TargetHPSys.MaxHealth;
                                //Debug.Log("One RegenScript found a TargetHPSys");
                            }

                        }
                    }
                    
                    }

            }
        }
    }
}

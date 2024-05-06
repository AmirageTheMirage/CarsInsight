using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GlobalVars : MonoBehaviour
{
    public bool BuildingMode = false;
    public GameObject Car;
    public GameObject[] HideOnBuilding;
    public GameObject[] ShowOnBuilding;
    public GameObject[] HideOnProximity;
    public GameObject[] ShowOnProximity;
    public GameObject[] ShowOnEditing;
    private List<GameObject> ProxmList = new List<GameObject>();
    public float ProximityRange = 50f;
    public bool InProximity = false;
    public BuildingMode BuildScript;
    public GameObject BuildingCamera;
    public Transform BuildingTargetCamPos;
    public GameObject MiddleObj;
    public Transform EditTargetCamPos;
    public Transform EditTargetBasePos;
    private Transform ActualTargetPos;
    private Transform OriginCam;
    public GameObject MainCam;
    public EditID_Texts EditTexts;
    public TMP_Text InfoText;
    public ScoreScript ScorScript;
    [Space]
    public bool Editing = false;
    public GameObject ClosestEditObject;
    public GameObject EditDecal;
    public float MaxEditDistance = 10f;
    public float LerpSpeed = 10f;
    [Space]
    public TMP_Text TitleText;

    private bool Lerp = false;
    private bool LerpDirection = false; //False = neutral, true = buildingpos
    
    private bool OneFrame = false;
    private float TimeToClose = 0f;
    [Space]
    private EditProperties EditScript;
    public bool Edit_Status;
    public string Edit_ID;
    public GameObject Edit_StatusActive;
    public GameObject Edit_StatusInActive;
    public TMP_Text Edit_Regen_ModeText;
    public GameObject Edit_RegenObject;
    public TMP_Text Edit_Regen_PrioText;
    public GameObject Edit_Regen_PrioObject;
    public AudioSource TuckSound;
    [Space]
    public GameObject Storage_PutInObject;
    public TMP_Text Storage_InputAmount;
    public TMP_Text Storage_Amount;
    public TMP_Text Storage_AmountInPercent;
    public Slider Storage_ChoseAmount;
    private float MaxStorageCanTake;
    private float MyScore;
    private bool AllowStorageInput = false;
    [Space]
    public GameObject Drill_Stuff;
    public TMP_Text Drill_RateText;
    public TMP_Text Drill_AmountText;
    void Start()
    {
       
        InfoText.text = "";
        Edit_RegenObject.SetActive(false);
        Storage_PutInObject.SetActive(false);
        Drill_Stuff.SetActive(false);
        Editing = false;
        BuildingCamera.SetActive(false);
        MainCam.SetActive(true);
        InProximity = FetchProximity();
        EditDecal.SetActive(false);
        OriginCam = BuildingCamera.transform;
        Edit_Regen_PrioObject.SetActive(false);

        OneCycle();
    }
    void Update()
    {
        //Fetch closest Edit Object
        if (Editing && Edit_ID == "Storage")
        {
            int RoundedAmount = Mathf.RoundToInt(EditScript.Storage_StoredAmount);
            int RoundedMax = Mathf.RoundToInt(EditScript.Storage_MaxStoredAmount);
            int CalcMax = Mathf.RoundToInt(RoundedAmount * 100f / RoundedMax);
            Storage_Amount.text = RoundedAmount.ToString() + " Blocks";
            Storage_AmountInPercent.text = CalcMax.ToString() + " %";

            //Assign the Storage Maximum Intake:
            MaxStorageCanTake = EditScript.Storage_MaxStoredAmount - EditScript.Storage_StoredAmount;
            MyScore = ScorScript.Score;
            if (RoundedAmount >= RoundedMax || MyScore <= 0.1f)
            {
                //Cant Intake anything, full
                Storage_ChoseAmount.minValue = 0f;
                Storage_ChoseAmount.maxValue = 0f; //THIS NEEDS FIXING MAYBE
                Storage_InputAmount.text = "0 Blocks";
                AllowStorageInput = false;
            } else
            {
                AllowStorageInput = true;
                //Debug.Log("Update");
                if (MyScore > MaxStorageCanTake)
                {
                    Storage_ChoseAmount.minValue = 0f;
                    Storage_ChoseAmount.maxValue = MaxStorageCanTake;
                    
                } else
                {
                    Storage_ChoseAmount.minValue = 0f;
                    Storage_ChoseAmount.maxValue = MyScore;
                    
                }
                int ScoreInp = Mathf.RoundToInt(Storage_ChoseAmount.value);
                if (ScoreInp < 1.4f)
                {

                Storage_InputAmount.text = ScoreInp.ToString() + " Block";
                } else
                {
                Storage_InputAmount.text = ScoreInp.ToString() + " Blocks";
                }

            }
        }

        if (Editing && Edit_ID == "Drill")
        {
            float Rate = EditScript.Drill_MiningRate;
            Rate *= 10f;
            Rate = Mathf.Round(Rate);
            Rate /= 10f;

            Drill_RateText.text = Rate.ToString() + "/s";
            Drill_AmountText.text = Mathf.RoundToInt(EditScript.Drill_TotalMined).ToString() + " Blocks total";
        }
        if (!Editing)
        {
            ClosestEditObject = FetchClosestEditObject(null, false);
        }
       if (Lerp)
        {
            TimeToClose += Time.deltaTime;
            if (LerpDirection)
            {
                //Up
                
                BuildingCamera.transform.position = Vector3.Lerp(BuildingCamera.transform.position, ActualTargetPos.position, LerpSpeed * Time.deltaTime);
                BuildingCamera.transform.rotation = Quaternion.Lerp(BuildingCamera.transform.rotation, ActualTargetPos.rotation, LerpSpeed * Time.deltaTime);
                if (Vector3.Distance(BuildingCamera.transform.position, ActualTargetPos.position) <= 0.1f || TimeToClose >= 2f)
                {
                    Lerp = false;
                }
            } else
            {
                if (OneFrame)
                {
                    OneFrame = false;
                    //MainCam.SetActive(false);
                }
                BuildingCamera.transform.position = Vector3.Lerp(BuildingCamera.transform.position, MainCam.transform.position, LerpSpeed * Time.deltaTime);
                BuildingCamera.transform.rotation = Quaternion.Lerp(BuildingCamera.transform.rotation, MainCam.transform.rotation, LerpSpeed * Time.deltaTime);
                if (Vector3.Distance(BuildingCamera.transform.position, MainCam.transform.position) <= 0.1f || TimeToClose >= 2f)
                {
                    BuildingCamera.SetActive(false);
                    MainCam.SetActive(true);
                    Lerp = false;
                }
            }
        }
       InProximity = FetchProximity();
        if (!InProximity && BuildingMode)
        {
            BackToNoBuild();
        }
        if ((!InProximity && Editing) || (ClosestEditObject == null && Editing))
        {
            //Debug.Log("A");
            BackToNoBuild();
        }

        if (Input.GetKeyDown("e") && InProximity && !BuildingMode && !Editing)
        {
            BuildingMode = true;
            Cursor.lockState = CursorLockMode.None;
            BuildingCamera.transform.position = MainCam.transform.position;
            BuildingCamera.transform.parent = MiddleObj.transform;
            BuildingCamera.transform.position = OriginCam.position;
            BuildingCamera.transform.rotation = OriginCam.rotation;
            
            BuildingCamera.SetActive(true);
            ActualTargetPos = BuildingTargetCamPos;
            MainCam.SetActive(true);//OneFrame
            OneFrame = true;
            Lerp = true;
            TimeToClose = 0f;
            LerpDirection = true;
            PlayTuck();

        }

        if (Input.GetKeyDown("f") && InProximity && !BuildingMode && !Editing && ClosestEditObject != null)
        {
            //Debug.Log("B");
            EditGo();
        }

        OneCycle();
    }
    bool IsInArray(GameObject obj, GameObject[] array)
    {
        foreach (GameObject element in array)
        {
            if (element == obj)
            {
                return true;
            }
        }
        return false;
    }

    bool FetchProximity()
    {

        ProxmList.Clear();
        GameObject[] CampsFound;
        CampsFound = GameObject.FindGameObjectsWithTag("MyBaseProximity");

        foreach (GameObject obj in CampsFound)
        {
            if (Vector3.Distance(Car.transform.position, obj.transform.position) <= ProximityRange)
            {
                if (obj == null)
                {
                    Debug.LogWarning("Object doesn't exist!");
                }
                else
                {
                    ProxmList.Add(obj);
                }
            }
        }
        if (ProxmList == null)
        {
            return false;
        } else
        {
            if (ProxmList.Count > 0)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }

    public void BackToNoBuild()
    {
        BuildScript.SetAllFalse();
        BuildScript.ClosingOut();
        BuildingMode = false;
        Editing = false;
        Cursor.lockState = CursorLockMode.Locked;
        LerpDirection = false;
        Lerp = true;
        TimeToClose = 0f;
        PlayTuck();


    }

    void Objectize(GameObject[] ArrayToObj, bool Show)
    {
        foreach (GameObject obj in ArrayToObj)
        {
            if (obj != null)
            {
                if (Show)
                {
                    obj.SetActive(true);
                } else
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    void OneCycle()
    {
        if (InProximity)
        {
            Objectize(HideOnProximity, false);
            Objectize(ShowOnProximity, true);

            if (BuildingMode && !Editing)
            {
                Objectize(HideOnBuilding, false);
                Objectize(ShowOnBuilding, true);
            }
            else
            {
                if (Editing)
                {
                    Objectize(HideOnBuilding, false);
                    Objectize(ShowOnBuilding, false);
                    Objectize(ShowOnEditing, true);
                }
                else
                {
                    Objectize(HideOnBuilding, true);
                    Objectize(ShowOnBuilding, false);
                    Objectize(ShowOnEditing, false);

                }

                
            }


            if (ClosestEditObject != null && !BuildingMode && !Editing)
            {
                EditDecal.SetActive(true);
                
            }
            else
            {
                EditDecal.SetActive(false);
                
            }


        }
        else
        {
            //if (Editing)
            //{
            //    BackToNoBuild();
            //}
            Objectize(HideOnProximity, true);
            Objectize(HideOnBuilding, true);
            Objectize(ShowOnProximity, false);
            Objectize(ShowOnBuilding, false);
            Objectize(ShowOnEditing, false);
            EditDecal.SetActive(false);
        }
    }
    void EditGo()
    {
        Editing = true;
        Cursor.lockState = CursorLockMode.None;
        //Set EditTargetCamPos Position

        float dis = EditScript.CamDistance;
        EditTargetBasePos.eulerAngles = new Vector3(0f, 0f, 0f);
        EditTargetBasePos.position = EditScript.LockAtObj.transform.position;
        ActualizeEdit();
        
            EditTargetCamPos.eulerAngles = new Vector3(10f, 0f, 0f);
            EditTargetCamPos.localPosition = new Vector3(-0.6f * dis, 1f * dis, -2f * dis);
            //Reset Rotation?
            ActualTargetPos = EditTargetCamPos;
            
            BuildingCamera.transform.parent = EditTargetCamPos.transform;
            BuildingCamera.transform.position = MainCam.transform.position;
            BuildingCamera.SetActive(true);
            Lerp = true;
            TimeToClose = 0f;
            OneFrame = true;
            LerpDirection = true;
        
        PlayTuck();
    }
    public void PlayTuck()
    {
        TuckSound.pitch = Random.Range(0.8f, 1.2f);
        TuckSound.Play();
    }
    GameObject FetchClosestEditObject(GameObject Curr, bool Back)
    {
        
        GameObject[] AllEditObjects = GameObject.FindGameObjectsWithTag("CanEdit");
        if (Curr == null)
        {
            //Debug.Log("Called");
            GameObject ClosObject = null;
            float Dis = Mathf.Infinity;
            if (AllEditObjects.Length > 0)
            {
                foreach (GameObject obj in AllEditObjects)
                {
                    if (obj != null)
                    {
                        float MyDis = Vector3.Distance(obj.transform.position, Car.transform.position);
                        if (MyDis <= Dis && MyDis <= MaxEditDistance)
                        {
                            Dis = MyDis;
                            ClosObject = obj;
                        }
                    }
                }



            }
            if (ClosObject != null)
            {

                if (Editing == false) //else the script will change mid-editing if the car rolls to be closer to another "Edit"-Object
                {
                    EditScript = ClosObject.GetComponent<EditProperties>();
                }
            }
            return ClosObject;

        } else
        {
            if (Back)
            {
                float DistUnder = Vector3.Distance(Curr.transform.position, Car.transform.position);
                GameObject ClosObject = null;
                float Dis = Mathf.Infinity;
                if (AllEditObjects.Length > 0)
                {
                    foreach (GameObject obj in AllEditObjects)
                    {
                        if (obj != null)
                        {
                            float MyDis = Vector3.Distance(obj.transform.position, Car.transform.position);
                            if (MyDis <= Dis && MyDis <= MaxEditDistance && MyDis < DistUnder && obj != Curr)
                            {

                                Dis = MyDis;
                                ClosObject = obj;
                            }
                        }
                    }



                }
                if (ClosObject == null)
                {
                    ClosObject = ClosestEditObject;
                }
                else
                {

                    if (Editing == false) //else the script will change mid-editing if the car rolls to be closer to another "Edit"-Object
                    {
                        EditScript = ClosObject.GetComponent<EditProperties>();
                    }
                }
                //Debug.Log("BackCalled" + ClosObject.transform.parent);
                return ClosObject;



            } else
            {
                float DistOver = Vector3.Distance(Curr.transform.position, Car.transform.position);
                GameObject ClosObject = null;
                float Dis = Mathf.Infinity;
                if (AllEditObjects.Length > 0)
                {
                    foreach (GameObject obj in AllEditObjects)
                    {
                        if (obj != null)
                        {
                            float MyDis = Vector3.Distance(obj.transform.position, Car.transform.position);
                            if (MyDis <= Dis && MyDis <= MaxEditDistance && MyDis > DistOver && obj != Curr)
                            {

                                Dis = MyDis;
                                ClosObject = obj;
                            }
                        }
                    }



                }
                if (ClosObject == null)
                {
                    ClosObject = ClosestEditObject;
                }
                else
                {

                    if (Editing == false) //else the script will change mid-editing if the car rolls to be closer to another "Edit"-Object
                    {
                        EditScript = ClosObject.GetComponent<EditProperties>();
                    }
                }
                //Debug.Log("NextCalled" + ClosObject.transform.parent);
                return ClosObject;
            }
            //Debug.Log("Next");
            
        }

        
    }
    public void StepFurther()
    {

        BuildScript.ClosingOut();
        BuildingMode = false;
        Editing = false;

        ClosestEditObject = FetchClosestEditObject(ClosestEditObject, false);
        EditGo();

        // Set Cam Directly
        BuildingCamera.transform.position = ActualTargetPos.position;
        BuildingCamera.transform.rotation = ActualTargetPos.rotation;


    }

    public void StepBack()
    {
        BackToNoBuild();
        ClosestEditObject = FetchClosestEditObject(ClosestEditObject, true);
        EditGo();

        // Set Cam Directly
        BuildingCamera.transform.position = ActualTargetPos.position;
        BuildingCamera.transform.rotation = ActualTargetPos.rotation;

    }

    void ActualizeEdit()
    {
        
        Edit_ID = EditScript.ID;
        Edit_Status = EditScript.StatusActive;
        int RegenPrio = EditScript.Regenerator_Priority;
        
        if (Edit_ID == "Regenerator")
        {
            InfoText.text = EditTexts.RegeneratorText;
            Edit_RegenObject.SetActive(true);
        } else
        {
            Edit_RegenObject.SetActive(false);
        }

        if (Edit_ID == "Storage")
        {
            InfoText.text = EditTexts.StorageText;
            Storage_PutInObject.SetActive(true);
            Storage_ChoseAmount.value = 0f;
        } else
        {
            Storage_PutInObject.SetActive(false);
        }
        if (Edit_ID == "Orbital Cannon")
        {
            InfoText.text = EditTexts.OrbitalCannonText;
        }
        if (Edit_ID == "Small Cannon")
        {
            InfoText.text = EditTexts.SmallCannonText;
        }
        if (Edit_ID == "Drill")
        {
            Drill_Stuff.SetActive(true);
            InfoText.text = EditTexts.DrillText;
        } else
        {
            Drill_Stuff.SetActive(false);
        }


        TitleText.text = Edit_ID;
        if (Edit_Status)
        {
            Edit_StatusActive.SetActive(true);
            Edit_StatusInActive.SetActive(false);
        } else
        {
            Edit_StatusActive.SetActive(false);
            Edit_StatusInActive.SetActive(true);
        }
        //RegenMode
        if (EditScript.Regenerator_Mode == 1)
        {
            Edit_Regen_ModeText.text = "only Shield";
            

        } else if (EditScript.Regenerator_Mode == 2)
        {
            Edit_Regen_ModeText.text = "other Objects";
        } else if (EditScript.Regenerator_Mode == 3)
        {
            Edit_Regen_ModeText.text = "all";
        } else
        {
            Edit_Regen_ModeText.text = "Error";
        }

        //Regen_Priority
        if (RegenPrio == 1) {

            Edit_Regen_PrioText.text = "Random";
        } else if (RegenPrio == 2)
        {
            Edit_Regen_PrioText.text = "Shield first";
        } else if (RegenPrio == 3)
        {
            Edit_Regen_PrioText.text = "Objects first";
        } else
        {
            Edit_Regen_PrioText.text = "Error";
        }
        if (EditScript.Regenerator_Mode == 3)
        {
            Edit_Regen_PrioObject.SetActive(true);
        }
        else
        {
            Edit_Regen_PrioObject.SetActive(false);
        }
    }

    public void Edit_SetActive(bool Status)
    {
        Edit_Status = Status;
        EditScript.StatusActive = Edit_Status;
        EditScript.TriggerChange();
        ActualizeEdit();
    }

    public void Edit_Regenerator_Mode()
    {
        
        int RegenMode = EditScript.Regenerator_Mode;
        RegenMode += 1;
        if (RegenMode > 3)
        {
            RegenMode = 1;
        }
        
        EditScript.Regenerator_Mode = RegenMode;
        ActualizeEdit();
    }

    public void Edit_Regenerator_Prio()
    {
        int RegenPrioPlus = EditScript.Regenerator_Priority;
        RegenPrioPlus += 1;
        if (RegenPrioPlus > 3)
        {
            RegenPrioPlus = 1;
        }
        EditScript.Regenerator_Priority = RegenPrioPlus;
        ActualizeEdit();
    }

    public void Edit_ApplyStorageInput()
    {
        int Valu = Mathf.RoundToInt(Storage_ChoseAmount.value);
        if (AllowStorageInput)
        {
            ScorScript.Score -= Valu;
            EditScript.StorTransfer += Valu;
            
        }
    }
}

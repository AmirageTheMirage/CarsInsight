using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class BuildingMode : MonoBehaviour
{
    public GameObject Cube;
    public GlobalVars Glob;
    public Camera MyCamera;
    public GameObject StationaryOrbital;
    public GameObject SmallCannon;
    public GameObject Storage;
    public GameObject Regenerator;
    public GameObject Drill;
    public float RangeFromCamp = 30f;
    public ScoreScript ScorScr;
    

    public GameObject DeadStationaryOrbital;
    public GameObject DeadSmallCannon;
    public GameObject DeadStorage;
    public GameObject DeadRegenerator;
    public GameObject DeadDrill;

    public GameObject DeadStationaryOrbital2; //Red Mat
    public GameObject DeadSmallCannon2;
    public GameObject DeadStorage2;
    public GameObject DeadRegenerator2;
    public GameObject DeadDrill2;

    public List<GameObject> DecalBigger = new List<GameObject>();
    public GameObject ActiveObject;
    public string ObjChosenStr;
    public GameObject ObjChosen;
    public GameObject ParentObject;
    public AudioSource BuildSound1;
    public AudioClip BuildSound11;
    public AudioSource ChooseSound;
    
    [Space]
   
    public GameObject MiddleObject;
    public float TurnSpeed = 10f;
    //public EditMode EditScript;
    //private bool Editin;
    private bool OverUI = false;
    private bool RayHit = false;
    private bool InCampRange = false;
    private bool BuildingMod = false;
    private Renderer CubRenderer;
    //private bool ChangedSmth = true;
    private Vector3 TarVec;
    private bool Collides = false;
    //public List<int> Prices = [500, 1000, 300, 250, 500]; // SmallCannon, Orbital, Storage, Regen, Drill
    public int CurrPrice = 0;
    //private bool SkipOne = false; //Skips one Frame in Update();
    void Start()
    {
        CubRenderer = Cube.GetComponent<Renderer>();
        //Editin = EditScript.Editing;
    }
    public void SetAllFalse()
    {
        DeadStationaryOrbital.SetActive(false);
        DeadSmallCannon.SetActive(false);
        DeadStorage.SetActive(false);
        DeadRegenerator.SetActive(false);
        DeadDrill.SetActive(false);

        DeadStationaryOrbital2.SetActive(false);
        DeadSmallCannon2.SetActive(false);
        DeadStorage2.SetActive(false);
        DeadRegenerator2.SetActive(false);
        DeadDrill2.SetActive(false);

    }
    void Update()
    {
           // Editin = EditScript.Editing;
            Collides = IsCollidingWithOtherObjects();
            HandleDecals(false);
            IsCollidingWithOtherObjects();
            int layerMask = ~LayerMask.GetMask("Shield");

            Ray ray = MyCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {

                TarVec = hit.point;
                TarVec.y = 0f;

                RayHit = true;
                //Debug.Log("Hit" + hit.point);
            }
            else
            {
                RayHit = false;
                //Debug.Log("Didnt Hit");
            }
            BuildingMod = Glob.BuildingMode;
            if (Vector3.Distance(ParentObject.transform.position, TarVec) <= RangeFromCamp)
            {
                InCampRange = true;
            }
            else
            {
                InCampRange = false;
            }
            if (EventSystem.current.IsPointerOverGameObject())
            {
                OverUI = true;


            }
            else
            {
                OverUI = false;
            }


            if (ObjChosen != null ) 
            {

                Cube.SetActive(true);

                //AssignMaterial();
                Cube.transform.position = TarVec;

                if (RayHit && !OverUI && InCampRange && BuildingMod && !Collides && CurrPrice <= ScorScr.Score)
                {
                    Actif("Neutral");
                    //CubRenderer.material = NeutralMaterial;

                    if (Input.GetMouseButtonDown(2) && ScorScr.Score >= CurrPrice)
                    {
                    BuildSound1.pitch = Random.Range(0.8f, 1.2f);
                    BuildSound1.volume = Random.Range(0.8f, 1.2f);
                    BuildSound1.Play();
                    ScorScr.Score -= CurrPrice;
                    //BuildSound1.PlayDelayed(0f);
                        //Debug.Log("Camp wants to place new " + ObjChosen.name);
                        GameObject NewCampObj = Instantiate(ObjChosen);
                        NewCampObj.transform.parent = ParentObject.transform;
                        NewCampObj.transform.position = Cube.transform.position;
                        NewCampObj.SetActive(true);
                        NewCampObj.tag = "Friendly";
                        
                        if (ObjChosenStr == "SmallCannon")
                        {
                            NewCampObj.transform.position += Vector3.up * 1.05f;
                        }
                        if (ObjChosenStr == "OrbitalCannon")
                        {
                            NewCampObj.transform.position += Vector3.up * 1.3f;
                        }
                        if (ObjChosenStr == "Regenerator")
                        {
                            NewCampObj.transform.position += Vector3.up * 0.5f;
                        }
                    }
                }
                else
                {


                    Actif("Red");


                }

            }
            else
            {



                Actif("Red");

            }

            if (BuildingMod && !OverUI)
            {
            //AllowTurning
            if (Input.GetMouseButton(0))
            {
                //Turn Left
                MiddleObject.transform.Rotate(0f, TurnSpeed * Time.deltaTime, 0f);
                
            } else if (Input.GetMouseButton(1))
            {
                //Turn Right
                MiddleObject.transform.Rotate(0f, -TurnSpeed * Time.deltaTime, 0f);

            }
            }

        
    }
    public void PlayTick()
    {
        ChooseSound.pitch = Random.Range(0.8f, 1.2f);
        ChooseSound.Play();
    }
    public void ChoseObject(GameObject me)
    {
        
        ActiveObject = me;
        DecalBigger.Add(me);
        
    }
    public void ChooseObject(string Obj)
    {
        //Debug.Log("Choose: " + Obj);
        bool Valid = false;
        if (Obj == "SmallCannon")
        {
            Valid = true;
            ObjChosen = SmallCannon;
            CurrPrice = 500;
            //Cube = DeadSmallCannon;
            //Debug.Log("Assigned");
        }
        if (Obj == "Storage")
        {
            Valid = true;
            ObjChosen = Storage;
            CurrPrice = 300;
            //Cube = DeadStorage;
        }
        if (Obj == "OrbitalCannon")
        {
            Valid = true;
            ObjChosen = StationaryOrbital;
            CurrPrice = 1000;
            //Cube = DeadStationaryOrbital;
        }
        if (Obj == "Regenerator")
        {
            Valid = true;
            ObjChosen = Regenerator;
            CurrPrice = 250;
            //Cube = DeadRegenerator;
        }
        if (Obj == "Drill")
        {
            Valid = true;
            ObjChosen = Drill;
            CurrPrice = 500;
        }

        if (Valid)
        {
            ObjChosenStr = Obj;
            Cube.SetActive(false);
            Actif(ObjChosenStr);


        }

    }
    void Actif(string Mode)
    {
        //Debug.Log("Actif Called");
        //Debug.Log(Cube.activeSelf);
        if (ObjChosenStr == "SmallCannon")
        {
            if (Mode == "Neutral")
            {
                Cube.SetActive(false);
                Cube = DeadSmallCannon;
                Cube.SetActive(true);
                //Debug.Log("Actif Neutral");
            } else
            {
                Cube.SetActive(false);
                Cube = DeadSmallCannon2;
                Cube.SetActive(true);
                //Debug.Log("Actif Red");
            }
        }
        if (ObjChosenStr == "Storage")
        {
            if (Mode == "Neutral")
            {
                Cube.SetActive(false);
                Cube = DeadStorage;
                Cube.SetActive(true);
            }
            else
            {
                Cube.SetActive(false);
                Cube = DeadStorage2;
                Cube.SetActive(true);
            }
        }
        if (ObjChosenStr == "OrbitalCannon")
        {
            if (Mode == "Neutral")
            {
                Cube.SetActive(false);
                Cube = DeadStationaryOrbital;
                Cube.SetActive(true);
            }
            else
            {
                Cube.SetActive(false);
                Cube = DeadStationaryOrbital2;
                Cube.SetActive(true);
            }
        }
        if (ObjChosenStr == "Regenerator")
        {
            if (Mode == "Neutral")
            {
                Cube.SetActive(false);
                Cube = DeadRegenerator;
                Cube.SetActive(true);
            }
            else
            {
                Cube.SetActive(false);
                Cube = DeadRegenerator2;
                Cube.SetActive(true);
            }
        }
        if (ObjChosenStr == "Drill")
        {
            if (Mode == "Neutral")
            {
                Cube.SetActive(false);
                Cube = DeadDrill;
                Cube.SetActive(true);
            }
            else
            {
                Cube.SetActive(false);
                Cube = DeadDrill2;
                Cube.SetActive(true);
            }
        }

    }
    public void ClosingOut()
    {
        //This should be played before the Building Mode is closed
        ObjChosen = null;
        ActiveObject = null;
        HandleDecals(true);
        SetAllFalse();
        Cube.SetActive(false);
        

    }


    void AssignMaterial(Material NewMat)
    {
        Transform[] children = Cube.GetComponentsInChildren<Transform>(true);
        GameObject[] childObjects = new GameObject[children.Length - 1];
        Debug.Log(childObjects);
        foreach (GameObject Gobj in childObjects)
        {
            if (Gobj != null)
            {
                Debug.Log(Gobj.name);
            
            Renderer Rend = Gobj.GetComponent<Renderer>();
            if (Rend != null)
            {
                Rend.material = NewMat;
            }
            }
        }
    }


    void HandleDecals(bool Reset)
    {
        //ActiveDecals
        //DecalBigger
        float TargetSize = 0.9f;
        float StandardSize = 0.7316f; //From Editor Menu
        float TimeLerp = 12f;
        if (Reset)
        {
            foreach (GameObject Decx in DecalBigger)
            {
                if (Decx != null)
                {

                    Decx.transform.localScale = new Vector3(StandardSize, StandardSize, StandardSize);
                }
            }

                } else { 
        if (DecalBigger.Count > 0)
        {
                foreach (GameObject Dec in DecalBigger)
                {
                    if (Dec != null)
                    {
                        if (ActiveObject != null)
                        {
                            if (Dec == ActiveObject)
                            {
                                Dec.transform.localScale = Vector3.Lerp(Dec.transform.localScale, new Vector3(TargetSize, TargetSize, TargetSize), TimeLerp * Time.deltaTime);
                            }
                            else
                            {
                                Dec.transform.localScale = Vector3.Lerp(Dec.transform.localScale, new Vector3(StandardSize, StandardSize, StandardSize), TimeLerp * Time.deltaTime);
                            }

                        }
                        else
                        {
                            Dec.transform.localScale = Vector3.Lerp(Dec.transform.localScale, new Vector3(StandardSize, StandardSize, StandardSize), TimeLerp * Time.deltaTime);

                        }
                    }
                }
            }
        }
    }




    bool IsCollidingWithOtherObjects()
    {
        // Check if any collider attached to this GameObject is colliding with other colliders
        Collider[] colliders = Physics.OverlapBox(Cube.transform.position, Cube.transform.localScale / 2);
        //Debug.Log(colliders.Length);
        // Check if there are colliders other than this GameObject's colliders
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != Cube && collider.gameObject.tag != "TerrainTag")
            {
                //Debug.Log("True + " + collider.gameObject.name);
                return true;
            }
        }

        //Debug.Log("False");
        return false;
    }

}

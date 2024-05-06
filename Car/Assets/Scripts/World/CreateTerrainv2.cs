using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTerrainv2 : MonoBehaviour
{
    public GameObject TerrainTemplate1;
    public GameObject TerrainParent;
    public GameObject Car;
    public GameObject MapObject1;
    public GameObject MapObject2;
    public GameObject MapObject3;
    public int MapObjectsPerTerrain = 25;
    public List<float> ListX = new List<float>();
    public List<float> ListZ = new List<float>();
    public GenerateCamp CampScript;
    public GameObject[] AllWalls;
    private float CarX;
    private float CarZ;
    public AnimationCurve WallsAmount;
    public bool UseCurve = true; //If false, Use the Fix Amount "MapObjectsPerTerrain"
    public GameObject MiddleObject;
    public float OneInXHasCamp;


    void Start()
    {
        MakeTerrain(0f, -1000f);
        ListX.Add(0f);
        ListZ.Add(0f);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AllWalls = GameObject.FindGameObjectsWithTag("WallObstacle");
        
            CarX = Car.transform.position.x;
        CarZ = Car.transform.position.z;
        CarX = Mathf.Round(CarX / 1000f) * 1000f;
        CarZ = Mathf.Round(CarZ / 1000f) * 1000f;
        TryTerrainAround(CarX, CarZ);

        //Get TerrainWalls

        //Debug.Log("ListX: " + string.Join(", ", ListX));
        //Debug.Log("ListZ: " + string.Join(", ", ListZ));
        //int ItemsRemoved = 0;
        //for (int i = ListX.Count - 1; i >= 0; i--)
        //{
        //    Debug.Log(i);
           
        //    if (i >= 0 && i < ListZ.Count)
        //    {
        //        if (i >= 0 && i < ListX.Count)
        //        {
        //            if (ListX.Count == ListZ.Count)
        //            {
        //                if (Vector3.Distance(new Vector3(Car.transform.position.x, 0f, Car.transform.position.z), new Vector3(ListX[i], 0f, ListZ[i])) >= 3000f) //3000f is synced with FarAway.cs
        //                {
        //                    ItemsRemoved += 1;
        //                    ListX.Remove(i);
        //                    ListZ.Remove(i);

        //                }
        //            } else
        //            {
        //                Debug.Log("Length of ListX isnt ListZ. PROBLEM");
        //            }
        //        }

        //    }
        //    if (ItemsRemoved > 0)
        //    {
        //        Debug.Log("Removed " + ItemsRemoved.ToString() + " Terrains.");
        //    }
        //}
        
    }

    void MakeTerrain(float xF, float zF)
    {
        if (!TerrainExists(xF, zF))
        {
            GameObject NewTerrain = Instantiate(TerrainTemplate1);
            NewTerrain.transform.parent = TerrainParent.transform;
            NewTerrain.transform.position = new Vector3(xF, 0f, zF);
            FarAway FarAwayScript = NewTerrain.GetComponent<FarAway>();
            FarAwayScript.enabled = true;
            
            ListX.Add(xF);
            ListZ.Add(zF);
            NewTerrain.name = ListX.Count.ToString();
            float CampX = Random.Range(0f, 500f);
            float CampZ = Random.Range(0f, 500f);

            //Calc Distance
            if (UseCurve)
            {
            float Dist = Vector3.Distance(NewTerrain.transform.position, MiddleObject.transform.position);
            MapObjectsPerTerrain = Mathf.RoundToInt(WallsAmount.Evaluate(Dist));
            }

            if (Random.Range(0f, OneInXHasCamp) <= 1f) //One in 4 Terrains has a Camp
            {
            //CampScript.GenCamp(Random.Range(NewTerrain.transform.position.x - 500f, NewTerrain.transform.position.x + 500f), 0f, Random.Range(NewTerrain.transform.position.z - 500f, NewTerrain.transform.position.z + 500f), NewTerrain, false);
            CampScript.GenCamp(NewTerrain.transform.position.x + CampX, 0f, NewTerrain.transform.position.z + CampZ, NewTerrain, false);
            GenerateWalls(true, NewTerrain, NewTerrain.transform.position.x + CampX, NewTerrain.transform.position.z + CampZ);
            } else {
                GenerateWalls(false, NewTerrain, 0f, 0f);
            }
            //Generate 
            //Debug.Log("ListX: " + string.Join(", ", ListX));
            //Debug.Log("ListZ: " + string.Join(", ", ListZ));
        }
    }

    void GenerateWalls(bool HasCamp, GameObject TerrainParent, float xCamp, float zCamp)
    {
        float xWall = 0f;
        float zWall = 0f;
        
        
        for (int i = 0; i < MapObjectsPerTerrain; i++)
        {
            if (Random.Range(0f, 2f) <= 1f)
            {
                
                    xWall = Random.Range(0, 1000f);
                    zWall = Random.Range(0, 1000f);
                
                    if (Vector3.Distance(new Vector3(xWall + TerrainParent.transform.position.x, 0f, zWall + TerrainParent.transform.position.z), new Vector3(xCamp, 0f, zCamp)) >= 150f || !HasCamp)
                    {
                        //Delete all other Objects
                        //List<GameObject> nearbyObjects = new List<GameObject>();

                        //Generate Map Object
                        GameObject Obst;
                        if (Random.Range(0f, 3f) <= 1f)
                        {
                            Obst = MapObject1;
                        }
                        else if (Random.Range(0f, 3f) <= 1f)
                        {
                            Obst = MapObject2;
                        }
                        else
                        {
                            Obst = MapObject3;
                        }
                        GameObject NewMapObj = Instantiate(Obst);
                        NewMapObj.transform.parent = TerrainParent.transform;
                        NewMapObj.transform.localPosition = new Vector3(xWall, 0f, zWall);
                       
                        NewMapObj.transform.Rotate(new Vector3(0f, Random.Range(0f, 360f), 0f));
                        NewMapObj.isStatic = true;
                        NewMapObj.SetActive(true);
                    }
                
            }
        }
        //if (HasCamp)
        //{
        //    foreach (GameObject obj in AllWalls)
        //    {
        //        if (obj != null)
        //        {
        //            if (Vector3.Distance(obj.transform.position, new Vector3(xCamp, 0f, zCamp)) <= 400f)
        //            {
        //                //nearbyObjects.Add(obj);
        //                obj.isStatic = false;
        //                //Destroy(obj);
        //            }
        //        }

        //    }
        //}
    }
    bool TerrainExists(float x, float z)
    {
        for (int i = 0; i < ListX.Count; i++)
        {
            if (Mathf.Approximately(ListX[i], x) && Mathf.Approximately(ListZ[i], z))
            {
                return true;
            }
        }
        //Debug.Log("Terrain Exists.");
        return false;
    }


    void TryTerrainAround(float Tx, float Tz)
    {
        MakeTerrain(Tx - 1000f, Tz + 1000f);
        MakeTerrain(Tx - 1000f, Tz);
        MakeTerrain(Tx - 1000f, Tz - 1000f);
        MakeTerrain(Tx, Tz + 1000f);
        //MakeTerrain(Tx, Tz);
        MakeTerrain(Tx, Tz - 1000f);
        MakeTerrain(Tx + 1000f, Tz + 1000f);
        MakeTerrain(Tx + 1000f, Tz);
        MakeTerrain(Tx + 1000f, Tz - 1000f);

        MakeTerrain(Tx - 2000f, Tz + 2000f);
        MakeTerrain(Tx - 2000f, Tz + 1000f);
        MakeTerrain(Tx - 2000f, Tz);
        MakeTerrain(Tx - 2000f, Tz - 1000f);
        MakeTerrain(Tx - 2000f, Tz - 2000f);

        MakeTerrain(Tx - 1000f, Tz + 2000f);
        MakeTerrain(Tx - 1000f, Tz - 2000f);
        MakeTerrain(Tx, Tz + 2000f);
        MakeTerrain(Tx, Tz - 2000f);
        MakeTerrain(Tx + 1000f, Tz + 2000f);
        MakeTerrain(Tx + 1000f, Tz - 2000f);

        MakeTerrain(Tx + 2000f, Tz + 2000f);
        MakeTerrain(Tx + 2000f, Tz + 1000f);
        MakeTerrain(Tx + 2000f, Tz);
        MakeTerrain(Tx + 2000f, Tz - 1000f);
        MakeTerrain(Tx + 2000f, Tz - 2000f);



    }
}

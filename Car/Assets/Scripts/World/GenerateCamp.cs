using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GenerateCamp : MonoBehaviour
{
    public GameObject EnemyOrbital;
    public GameObject EnemyCube;
    public GameObject EnemySmallCannon;

    public bool Generate = true;
    public float Range = 1000f;

    public GameObject CampParent;
    public GameObject Targeter;
    public GameObject CampCenter;
    


    public int CampsCreated = 0;
    public void GenCamp(float x, float y, float z, GameObject ParentObject, bool UseRay)
    {
        CampsCreated += 1;
        if (UseRay == true)
        {
            Targeter.transform.position = new Vector3(x, 100f, z);
        } else
        {

            Targeter.transform.position = new Vector3(x, y, z);
        }
        //Targeter.transform.position = gameObject.transform.position;
        Ray ray = new Ray(Targeter.transform.position, Vector3.down);
        RaycastHit hit;
        //Debug.Log("CampIsInCreation0: Added to Stats");
        if (Physics.Raycast(ray, out hit) || UseRay == false)
        {
            //Debug.Log("Ray Hit For once");
            if (UseRay)
            {
                Targeter.transform.position = hit.point;
            }

            //Add Camp Center

            GameObject CentObj = Instantiate(CampCenter);
            CentObj.transform.position = Targeter.transform.position;
            //CentObj.transform.position += Vector3.up * 2f;
            CentObj.transform.parent = ParentObject.transform;
            CentObj.SetActive(true);
            //Debug.Log("CampIsInCreation2: Camp has Parent");
            //SetObject(CentObj, 0f, true);


            Targeter.transform.Rotate(0f, Random.Range(0f, 360f), 0f);
            for (int i = 0; i <= 3; i++)
            {
                Quaternion CurrentRotation = Targeter.transform.rotation;
                //Place an Orbital
                Targeter.transform.Translate(Vector3.forward * Random.Range(20f, 30f));
                if (Random.Range(0f, 2f) < 1f)
                {
                    GameObject Object1 = Instantiate(EnemyOrbital);
                    Object1.transform.position = Targeter.transform.position;
                    Object1.transform.position += Vector3.up * 3f;
                    Object1.transform.parent = CentObj.transform;
                    Object1.SetActive(true);
                    //SetObject(Object1, 0f, false);
                }
                //Place 1 - 2 GameObjects from the Orbital out
                Targeter.transform.Rotate(0f, Random.Range(70f, 110f), 0f);
                float StepForward = Random.Range(6f, 16f);
                Targeter.transform.Translate(Vector3.forward * StepForward);
                GameObject Object2 = Instantiate(EnemySmallCannon);
                Object2.transform.position = Targeter.transform.position;
                Object2.transform.position += Vector3.up * 1.2f;
                Object2.transform.parent = CentObj.transform;
                Object2.SetActive(true);
                //SetObject(Object2, 0f, false);
                if (Random.Range(0f, 2f) < 1f)
                {
                    Targeter.transform.Translate(Vector3.forward * StepForward * Random.Range(-1.8f, -2.3f));
                    GameObject Object3 = Instantiate(EnemySmallCannon);
                    Object3.transform.position = Targeter.transform.position;
                    Object3.transform.position += Vector3.up * 1.2f;
                    Object3.transform.parent = CentObj.transform;
                    Object3.SetActive(true);
                    //SetObject(Object3, 0f, false);
                }

                Targeter.transform.position = CentObj.transform.position;
                Targeter.transform.rotation = CurrentRotation;
                Targeter.transform.Rotate(0f, Random.Range(80f, 100f), 0f);
            }
            for (int i = 0; i < 20f; i++)
            {
                if (Random.Range(0f, 2f) <= 1f)
                {
                    //Generate Cubes
                    GameObject NewCube = Instantiate(EnemyCube);
                    Targeter.transform.position = CentObj.transform.position;
                    Targeter.transform.Rotate(0f, Random.Range(0f, 360f), 0f);
                    Targeter.transform.Translate(Vector3.forward * Random.Range(5f, 25f));
                    NewCube.transform.position = Targeter.transform.position;
                    NewCube.transform.position += Vector3.up * 0.51f;
                    NewCube.transform.parent = CentObj.transform;
                    NewCube.SetActive(true);
                    //SetObject(NewCube, 0f, false);
                }
            }
            //Debug.Log("CampIsInCreation3: Done");
            //GameObject Center = Instantiate(CampCenter);
            //Center.transform.parent = CampCenter.transform;
            //Center.transform.position = CampCenter.transform.position;
            //CentObj.transform.position += Vector3.up * 5f;
        } else
        {
            Debug.Log("Camp Failed: Raycast didn't hit");
        }
    }
    void FixedUpdate()
    {
        if (Generate)
        {
            Generate = false;
            GenCamp(gameObject.transform.position.x + Random.Range(-1 * Range, Range), 0f, gameObject.transform.position.z + Random.Range(-1 * Range, Range), CampParent, false);
        }
    }

    public void SetObject(GameObject Obj, float yOffset, bool Override)
    {
        Obj.transform.position += Vector3.up * 200f;
        Vector3 newPos = new Vector3(Obj.transform.position.x, 100f, Obj.transform.position.z);
        Ray ray = new Ray(newPos, Vector3.down);
        RaycastHit hit;        
        //Debug.Log("SetObject");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            //Debug.Log("Hit at: " + hit.point.y);
            Obj.transform.position = new Vector3(hit.point.x, hit.point.y + yOffset, hit.point.z);
            Obj.transform.rotation = Quaternion.Euler(Obj.transform.eulerAngles.x, Random.Range(0f, 360f), Obj.transform.eulerAngles.z);
        } else
        {
            Debug.Log("No Raycast Target Found.");
        }

    }
}

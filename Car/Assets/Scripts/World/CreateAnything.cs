using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAnything : MonoBehaviour
{
    public int Orbitals;
    public int SmallCannons;
    public int Cubes;

    public GameObject OrbitalCannonObject;
    public GameObject SmallCannonObject;
    public GameObject CubeObject;
    public float MaxRange;

    public GameObject Targeter;
    public GameObject ParentObject;
    public bool Create = true;
    void Start()
    {
        if (Create)
        {
            CreateElements();
        }
    }

    void FixedUpdate()
    {
        if (Create)
        {
            if (Orbitals != 0 || SmallCannons != 0 || Cubes != 0)
            {
                CreateElements();
            }
        }
    }

    public void CreateElements()
    {
        for (int i = 0; i < Orbitals; i++)
        {
            CreateOne("Orbital");
        }
        for (int i = 0; i < SmallCannons; i++)
        {
            CreateOne("SmallCannon");
        }
        for (int i = 0; i < Cubes; i++)
        {
            CreateOne("Cube");
        }
        Cubes = 0;
        Orbitals = 0;
        SmallCannons = 0;
        Create = false;
    }
    public void CreateOne(string Type)
    {
        Targeter.transform.position = new Vector3(Random.Range(gameObject.transform.position.x - MaxRange, gameObject.transform.position.x + MaxRange), 1000f, Random.Range(gameObject.transform.position.z - MaxRange, gameObject.transform.position.z + MaxRange));
        Ray ray = new Ray(Targeter.transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Targeter.transform.position = hit.point;
        }
        if (Type == "Orbital")
        {
            GameObject NewOrbital = Instantiate(OrbitalCannonObject);
            NewOrbital.transform.parent = ParentObject.transform;
            NewOrbital.transform.position = new Vector3(Targeter.transform.position.x, Targeter.transform.position.y + 1f, Targeter.transform.position.z);
        } else if (Type == "SmallCannon")
        {
            GameObject NewCannon = Instantiate(SmallCannonObject);
            NewCannon.transform.parent = ParentObject.transform;
            NewCannon.transform.position = new Vector3(Targeter.transform.position.x, Targeter.transform.position.y + 1.2f, Targeter.transform.position.z);
        }
        else if (Type == "Cube")
        {
            GameObject NewCube = Instantiate(CubeObject);
            NewCube.transform.parent = ParentObject.transform;
            NewCube.transform.position = new Vector3(Targeter.transform.position.x, Targeter.transform.position.y + 0.5f, Targeter.transform.position.z);
        }

    }
}

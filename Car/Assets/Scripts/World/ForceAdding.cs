using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAdding : MonoBehaviour
{
    public GameObject Car;
    public GameObject[] FriendlyObjects;
    public CamShake CamShakeScript;
    public float ImpForceThreshold = 0f;


    private void Update()
    {
        FriendlyObjects = GameObject.FindGameObjectsWithTag("Friendly");
    }
    public void FriendlyDmg(float ImpactForce, float Damage, GameObject You, float DeathRange)
    {
        
        List<GameObject> nearbyObjects = new List<GameObject>();
        foreach (GameObject FriendObj in FriendlyObjects)
        {
            if (FriendObj != null)
            {
                float distance = Vector3.Distance(FriendObj.transform.position, You.transform.position);
                if (distance <= 50f)
                {
                    nearbyObjects.Add(FriendObj);
                }
            }
        }
        foreach (GameObject nearbyObject in nearbyObjects)
        {
            Rigidbody objectRB = nearbyObject.GetComponent<Rigidbody>();
            if (objectRB != null)
            {
                Vector3 randomDirection = Random.onUnitSphere;
                objectRB.AddForce(randomDirection * ImpactForce, ForceMode.Impulse);
            }

            HealthSystem HPSys = nearbyObject.GetComponent<HealthSystem>();
            if (HPSys != null)
            {
                //Dmg with Dropoff:
                float DmgRange = Vector3.Distance(You.transform.position, nearbyObject.transform.position);
                if (DmgRange <= DeathRange)
                {
                    HPSys.Health -= Damage;

                } else
                {
                    if (Damage / DmgRange >= 1f)
                    {

                        HPSys.Health -= Damage / DmgRange;
                    }
                    else
                    {
                        HPSys.Health -= Damage / DmgRange;
                    }
                }
            }
        }
    }

    public void CarRB(float ImpactForce, GameObject You)
    {
        if (Vector3.Distance(Car.transform.position, You.transform.position) <= 50f)
        {

            Rigidbody CarRB = Car.GetComponent<Rigidbody>();
            float CurDistance = Vector3.Distance(Car.transform.position, You.transform.position);
            if (CarRB != null)
            {
                Vector3 randomDirection = Random.insideUnitSphere;
                if (randomDirection.y < 0)
                {
                    randomDirection.y = Random.Range(0f, 90f);
                }
                if (CurDistance <= 10f)
                {

                    CarRB.AddForce(randomDirection * ImpactForce, ForceMode.Impulse);
                    ShakeHandling(ImpactForce);
                }
                else if (CurDistance <= 30f)
                {
                    CarRB.AddForce(randomDirection * (ImpactForce / 2f), ForceMode.Impulse);
                    ShakeHandling(ImpactForce / 2f);
                }
                else
                {
                    CarRB.AddForce(randomDirection * (ImpactForce / CurDistance), ForceMode.Impulse);
                }
            }
        }
    }

    public void ShakeHandling(float ImpForce)
    {
        //Debug.Log("ImpactForce: " + ImpForce);
        if (ImpForce > ImpForceThreshold)
        {
            CamShakeScript.CamShakeGo(ImpForce / 2f, ImpForce / 2f * 3f, Random.Range(0.6f, 0.8f));
        }
    }
}

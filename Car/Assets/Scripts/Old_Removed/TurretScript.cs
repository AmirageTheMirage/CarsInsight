using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    public GameObject TurretMain;
    public GameObject MainCar;
    public float MaxRange = 100f;
    private Transform lastClosestObject = null;
    private float lastClosestTime = 0f;
    public GameObject ShooterObject;
    public float ShootingForce = 50f;
    public float TurnSpeed = 10f;


    void Update()
    {
        GameObject[] destroyableObjects = GameObject.FindGameObjectsWithTag("DestroyableObject");
        Transform closestObject = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPos = TurretMain.transform.position;

        foreach (GameObject obj in destroyableObjects)
        {
            float distance = Vector3.Distance(obj.transform.position, currentPos);
            if (distance < shortestDistance)
            {
                closestObject = obj.transform;
                shortestDistance = distance;
            }
        }
        if (shortestDistance <= MaxRange && closestObject != null)
        {
            if (lastClosestObject == closestObject)
            {
                lastClosestTime += Time.deltaTime;

                if (lastClosestTime >= 3f)
                {
                    lastClosestTime = 0f;
                    StartCoroutine(InstantiateShooterObject());
                }
            }
            else
            {
                lastClosestObject = closestObject;
                lastClosestTime = 0f;
            }

            Vector3 directionToTarget = closestObject.position - TurretMain.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            TurretMain.transform.rotation = Quaternion.Lerp(TurretMain.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
        }
        else
        {

            Vector3 directionToTarget = MainCar.transform.position - TurretMain.transform.position;
            directionToTarget.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            TurretMain.transform.rotation = Quaternion.Lerp(TurretMain.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
        }
    }


    IEnumerator InstantiateShooterObject()
    {
        
        GameObject ShotObject = Instantiate(ShooterObject, TurretMain.transform.position, Quaternion.identity);
        Rigidbody ShotObjectRb = ShotObject.GetComponent<Rigidbody>();
        ShotObject.transform.rotation = TurretMain.transform.rotation;
        ShotObject.transform.Translate(0f, 0f, 2.5f);
        ShotObject.SetActive(true);
        ShotObjectRb.AddForce(ShotObject.transform.forward * ShootingForce, ForceMode.Impulse);
        

        yield return null;
    }
}

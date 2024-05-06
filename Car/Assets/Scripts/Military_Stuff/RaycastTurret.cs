using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastTurret : MonoBehaviour
{
    public GameObject TurretMain;
    public GameObject MainCar;
    public GameObject FlickerLight;
    public GameObject DebugObject;
    public GameObject ShooterObject;
    public GameObject CrosshairDecal;
    public GameObject CamObject;
    public ParticleSystem ShootingParticles;
    private Vector3 OriginalPos;
    private Vector3 HiddenPos;
    public float MaxRange = 100f;
    private float lastClosestTime = 0f;
    public float RecoilForce = 50f;
    public float TurnSpeed = 10f;
    public float ExplosionForce = 50f;
    public float CoolDownTime = 0.1f;
    private Rigidbody rb;
    public float TurningTolerance = 5f;
    public Transform closestObject = null;
    private Transform lastClosestObject = null;
    public AudioSource ShootAudio;
    public Camera mainCamera;
    public GameObject[] destroyableObjects;
    public bool AllowTurretSnapping = false;
    public GameObject HiddenPosObject;
    public float Damage = 20f;

    public float CamLerpSpeed = 5f;
    private void Start()
    {

        OriginalPos = CamObject.transform.position;
        HiddenPos = HiddenPosObject.transform.position;
        CamObject.transform.position = HiddenPos;
        FlickerLight.SetActive(false);
        rb = MainCar.GetComponent<Rigidbody>();
        
        CrosshairDecal.SetActive(false);
    }

    void Update()
    {
        destroyableObjects = GameObject.FindGameObjectsWithTag("DestroyableObject");
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
            CrosshairDecal.SetActive(true);
            CamObject.transform.position = Vector3.Lerp(CamObject.transform.position, OriginalPos, CamLerpSpeed * Time.deltaTime);

            if (lastClosestObject == closestObject)
            {
                lastClosestTime += Time.deltaTime;

                if (lastClosestTime >= CoolDownTime)
                {
                    lastClosestTime = 0f;
                    InstantiateShooterObject();
                }
            }
            else
            {
                lastClosestObject = closestObject;
                lastClosestTime = 0f;
            }
            
            ScaleCrosshair();
            Vector3 directionToTarget = closestObject.position - TurretMain.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            float angle = Quaternion.Angle(TurretMain.transform.rotation, targetRotation);
            
            if (angle < TurningTolerance && AllowTurretSnapping)
            {
                TurretMain.transform.rotation = targetRotation;
            }
            else
            {
                TurretMain.transform.rotation = Quaternion.RotateTowards(TurretMain.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
            }
        }
        else
        {
            CrosshairDecal.SetActive(false);
            CamObject.transform.position = Vector3.Lerp(CamObject.transform.position, HiddenPos, CamLerpSpeed * Time.deltaTime);
            Vector3 directionToTarget = MainCar.transform.position - TurretMain.transform.position;
            directionToTarget.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            float angle = Quaternion.Angle(TurretMain.transform.rotation, targetRotation);
            
            if (angle < TurningTolerance)
            {
                TurretMain.transform.rotation = targetRotation;
            }
            else
            {
                TurretMain.transform.rotation = Quaternion.RotateTowards(TurretMain.transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
            }
        }
    }


    void InstantiateShooterObject()
    {
        //Debug.Log("Sending Ray");
        // Create a ray from the turret's position in the direction of the turret's forward vector
        Vector3 rayStart = TurretMain.transform.position + TurretMain.transform.forward * 2;
        Vector3 rayDirection = TurretMain.transform.forward;
        
        Ray ray = new Ray(rayStart, rayDirection);
        
        // Cast the ray
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, MaxRange))
        {
            // Debug.Log("Hit smth");
            // If the ray hits something, check if the hit object has the "DestroyableObject" tag

            
            if (hit.collider.gameObject.tag == "DestroyableObject")
            {
                //Debug.Log("Hit");
                //rb.AddForce(-1 * TurretMain.transform.forward * RecoilForce, ForceMode.Impulse);
                Rigidbody hitRb = hit.collider.gameObject.GetComponent<Rigidbody>();
                
               
                rb.AddForce(-1 * TurretMain.transform.forward * RecoilForce, ForceMode.Impulse);
                ShootingParticles.Play();
                ShootAudio.pitch = Random.Range(0.8f, 1.2f);
                ShootAudio.Play();
                HealthSystem HPSys = hit.collider.gameObject.GetComponent<HealthSystem>();
                if (HPSys != null)
                {
                    HPSys.Health -= Damage;
                }
                StartCoroutine(LightFlicker());

                if (hitRb != null)
                {
                    Vector3 randomDirection = Random.insideUnitSphere;
                    if (randomDirection.y < 0)
                    {
                        randomDirection.y = Random.Range(0f, 90f);
                    }
                    hitRb.AddForce(randomDirection * ExplosionForce, ForceMode.Impulse);
                }
            }
        }

        
    }

    IEnumerator LightFlicker()
    {
        FlickerLight.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        FlickerLight.SetActive(false);
    }


    void ScaleCrosshair()
    {

        //CrosshairDecal.transform.SetParent(closestObject.transform);
        //CrosshairDecal.transform.localPosition = Vector3.zero;
        CrosshairDecal.transform.position = closestObject.transform.position;
        CrosshairDecal.transform.LookAt(mainCamera.transform);
        CrosshairDecal.transform.Rotate(0, 180, 0);
        Renderer TryRender = closestObject.GetComponent<Renderer>();
        if (TryRender != null) {
            float SizeChange = TryRender.bounds.size.y;
        }
        //CrosshairDecal.transform.position += transform.forward * -1 * SizeChange; //Edit this Line

        float distance = Vector3.Distance(mainCamera.transform.position, CrosshairDecal.transform.position);
        float baseSize = 1f; // Base size of the CrosshairDecal
        float baseDistance = 10f; // Distance at which the CrosshairDecal has a base size

        float scaleFactor = baseSize / baseDistance * distance;
        CrosshairDecal.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCannon : MonoBehaviour
{
    public GameObject Car;
    public GameObject Barrel;
    public RaycastTurret RayScript;
    public float CoolDownTimer;
    public float MaxRange = 100f;
    public float MaxSpeed = 40f;
    private GameObject[] DestObjects;
    public GameObject RayHitPointDebug;
    public GameObject RaySpitter;
    public float ExplosionForce = 10f;
    public ParticleSystem ShootingParticles;
    public GameObject BarrelFront;
    public float SnapBackRange = 0.5f;
    public float SnapBackSpeed = 5f;
    private Vector3 OriginPosBarrel;
    private bool SnapBack = false;
    public GameObject MyTarget;
    public AudioSource ServoAudio;
    public AudioSource ShootingAudio;
    public Material NeutralMaterial;
    public Material EnemyMaterial;
    public GameObject Base;
    private bool SoundMoving = false;
    public EditProperties EditScript;
    public bool CanBeActive = true;
    
    [Tooltip("Car, Friendly (WIP), Enemy, Target")]
    public string AttackMode;
    private float NormalVolume;
    void Start()
    {
        NormalVolume = ServoAudio.volume;
        ServoAudio.pitch = Random.Range(1f, 1.4f);
        OriginPosBarrel = BarrelFront.transform.localPosition;
        CoolDownTimer = 1f;
        Renderer RendComp = Base.GetComponent<Renderer>();
        if (AttackMode == "Car")
        {

            RendComp.material = EnemyMaterial;
            gameObject.tag = "DestroyableObject";
        }
        else
        {
            RendComp.material = NeutralMaterial;
        }
    }

    private void FixedUpdate()
    {
        DestObjects = RayScript.destroyableObjects;
        if (CoolDownTimer >= 0f)
        {
            CoolDownTimer -= Time.deltaTime;
        }
    }
    void Update()
    {
        if (EditScript != null)
        {
            CanBeActive = EditScript.StatusActive;
        }
        if (SnapBack)
        {
            BarrelFront.transform.localPosition = Vector3.Lerp(BarrelFront.transform.localPosition, OriginPosBarrel, SnapBackSpeed * Time.deltaTime);
            if (Vector3.Distance(BarrelFront.transform.localPosition, OriginPosBarrel) <= 0.1f)
            {
                SnapBack = false;
                BarrelFront.transform.localPosition = OriginPosBarrel;
            }
        }
        if (CanBeActive)
        {
            ServoAudio.volume = NormalVolume;
            if (AttackMode == "Enemy")
            {
                MyTarget = FetchTarget();
            }
            else if (AttackMode == "Car")
            {
                MyTarget = Car;
            }

            if (MyTarget != null)
            {
                if (Vector3.Distance(MyTarget.transform.position, Barrel.transform.position) >= MaxRange)
                {
                    if (SoundMoving == false)
                    {
                        SoundMoving = true;
                        ServoAudio.pitch = Random.Range(1f, 1.4f);
                        ServoAudio.Play();
                    }
                    Barrel.transform.rotation = Quaternion.RotateTowards(Barrel.transform.rotation, Quaternion.identity, MaxSpeed * Time.deltaTime);
                }
                else
                {
                    Vector3 dir = MyTarget.transform.position - Barrel.transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(dir);

                    // rotate towards the target rotation with a max speed of 1f

                    if (Quaternion.Angle(Barrel.transform.rotation, targetRotation) < 0.3f)
                    {
                        if (SoundMoving)
                        {
                            SoundMoving = false;
                            ServoAudio.pitch = Random.Range(1f, 1.2f);
                            ServoAudio.Stop();
                        }
                    }
                    else
                    {
                        if (SoundMoving == false)
                        {
                            SoundMoving = true;
                            ServoAudio.pitch = Random.Range(1f, 1.2f);
                            ServoAudio.Play();
                        }
                        Barrel.transform.rotation = Quaternion.RotateTowards(Barrel.transform.rotation, targetRotation, MaxSpeed * Time.deltaTime);
                    }
                    //Barrel.transform.rotation = Quaternion.RotateTowards(Barrel.transform.rotation, targetRotation, 60f);
                    // Clamp the rotation angles to the specified range
                    //Vector3 eulerAngles = Barrel.transform.rotation.eulerAngles;
                    //eulerAngles.x = Mathf.Clamp(eulerAngles.x, 0f, 60f);
                    //Barrel.transform.rotation = Quaternion.Euler(eulerAngles);


                    if (CoolDownTimer <= 0f)
                    {
                        SendRay();
                        Debug.Log("SendRay");
                    }
                }
            }
            else
            {

                if (Quaternion.Angle(Barrel.transform.rotation, Quaternion.identity) < 0.3f)
                {
                    if (SoundMoving)
                    {
                        SoundMoving = false;
                        ServoAudio.pitch = Random.Range(1f, 1.2f);
                        ServoAudio.Stop();
                    }
                }
                else
                {
                    if (SoundMoving == false)
                    {
                        SoundMoving = true;
                        ServoAudio.pitch = Random.Range(1f, 1.2f);
                        ServoAudio.Play();
                    }
                    Barrel.transform.rotation = Quaternion.RotateTowards(Barrel.transform.rotation, Quaternion.identity, MaxSpeed * Time.deltaTime);
                }

            }
        } else
        {
            ServoAudio.volume = 0f;
        }

        
    }
    void SendRay()
    {

        Ray ray = new Ray(RaySpitter.transform.position, RaySpitter.transform.forward);
        RaycastHit hit;
        //Debug.DrawRay(RaySpitter.transform.position, RaySpitter.transform.forward * MaxRange);

        if (Physics.Raycast(ray, out hit, MaxRange))
        {
            RayHitPointDebug.transform.position = hit.point; 
            if (hit.collider.CompareTag("DestroyableObject") && AttackMode == "Enemy" || hit.collider.CompareTag("CarObject") && AttackMode == "Car")
            {
                //Debug.Log(hit.collider.gameObject);
                //Destroy(hit.collider.gameObject);
                GameObject HitObject = hit.collider.gameObject;
                ShootingParticles.Play();
                ShootingAudio.pitch = Random.Range(1f, 1.2f);
                ShootingAudio.Play();
                CoolDownTimer = 1f;
                SnapBack = true;
                BarrelFront.transform.localPosition = new Vector3(BarrelFront.transform.localPosition.x, BarrelFront.transform.localPosition.y, BarrelFront.transform.localPosition.z - SnapBackRange);
                //if (HitObject != null)
                //{
                //    Rigidbody HORB = HitObject.GetComponent<Rigidbody>();
                //    if (HORB != null)
                //    {
                //        Vector3 randomDirection = Random.insideUnitSphere;
                //        if (randomDirection.y < 0)
                //        {
                //            randomDirection.y = Random.Range(0f, 90f);
                //        }
                //        HORB.AddForce(randomDirection * ExplosionForce * HORB.mass, ForceMode.Impulse);
                //    }
                //}
            }
        }
    }

    GameObject FetchTarget()
    {
        if (DestObjects != null && DestObjects.Length > 0)
        {
            float ClosestDistance = 99999999f;
            GameObject ClosestObject = null;
            foreach (GameObject obj in DestObjects)
            {
                if (obj != null)
                {
                    float distance = Vector3.Distance(obj.transform.position, gameObject.transform.position);
                    if (distance < ClosestDistance)
                    {
                        ClosestObject = obj;
                        ClosestDistance = distance;
                    }
                }
            }
            if (ClosestDistance <= MaxRange)
            {
            return ClosestObject;

            } else
            {
            return null;

            }
        } else
        {
            return null;
        }
    }
}

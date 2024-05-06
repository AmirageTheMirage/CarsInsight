using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalCannon : MonoBehaviour
{

    public ParticleSystem ShootingParticles1;
    public GameObject Car;
    public GameObject TurnBase;

    public GameObject Barrel;
    public GameObject Target;
    public GameObject Barrels;
    public GameObject StrikeObject;

    public GameObject HitBox;
    public RaycastTurret RayScript;
    private bool ShootOnTarget; //For Manual Target Selection
    public float TurnBaseSpeed = 1f;
    public float TurnBarrelSpeed = 2f;
    public float Barrels_Offset = 3f;
    public float Barrels_SnapBackSpeed = 1f;
    public int Orbital_Mode;
    public float Barrel_Angle = 80f;

    public Material NeutralMaterial;
    public Material EnemyMaterial;
    public AudioSource ServoAudio;
    public AudioSource ShootingAudio;

    private bool BarrelIsMovingBack = false;
    private Vector3 barrelsOriginalPosition;
    private float WaitTimer = 0f;
    public float TargetWaitTime = 5f;
    public GameObject[] DestroyAbleObjects;
    public GameObject[] FriendlyObjects;
    [Header("ImpactStuff")]
    public GameObject PartObject;
    public GameObject PartObjectSmall;
    public GameObject PartObjectOrbit;
    private ParticleSystem PartSys;
    private ParticleSystem PartSys2;
    private ParticleSystem PartSys3;
    public AudioSource BoomSound;
    public AudioSource BoomSound2;
    public float ImpactForce = 100f;
    public float Damage = 1000f;
    private float MaxRange;
    public float MaxRangeFriendly = 4000f;
    public float MaxRangeEnemy = 2000f;
    [Tooltip("Car, Friendly, Enemy, Target, Support")]
    public string AttackMode;
    public bool OnShield = false;
    public EditProperties EditScript;
    public bool CanBeActive = true;
    private float StandardVolume;

    public float ImpForceThreshold = 0f;
    public CamShake CamShakeScript;
    public GracePeriod GrScr;
    void Start()
    {
        StandardVolume = ServoAudio.volume;
        if (AttackMode == "Car" || AttackMode == "Friendly")
        {
            MaxRange = MaxRangeEnemy;
        }
        else
        {
            MaxRange = MaxRangeFriendly;
        }
        PartSys = PartObject.GetComponent<ParticleSystem>();
        PartSys2 = PartObjectSmall.GetComponent<ParticleSystem>();
        PartSys3 = PartObjectOrbit.GetComponent<ParticleSystem>();
        AssignMaterial();



        if (AttackMode == "Target")
        {
            ShootOnTarget = true;
        }
        else
        {
            ShootOnTarget = false;
        }
        barrelsOriginalPosition = Barrels.transform.localPosition;
        Orbital_Mode = -1;
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2f);
        Orbital_Mode = 0;
    }
    void Update()
    {
        if (GrScr.Grace <= 0f || AttackMode != "Friendly")
        {
            //Wait...
        
            if (EditScript != null)
            {
                CanBeActive = EditScript.StatusActive;
            }
            if (CanBeActive || BarrelIsMovingBack)
            {
                ServoAudio.volume = StandardVolume;
                FriendlyObjects = GameObject.FindGameObjectsWithTag("Friendly");
                if (Orbital_Mode == 0) //Choosing Object, Target of this Mode is to assign the Target, then continue
                {
                    OnShield = false;
                    ServoAudio.pitch = Random.Range(0.8f, 1.2f);
                    if (ShootOnTarget)
                    {
                        Orbital_Mode = 1;
                        ServoAudio.Play();
                    }
                    else
                    {
                        if (AttackMode == "Car")
                        {
                            if (Vector3.Distance(Car.transform.position, gameObject.transform.position) <= MaxRange)
                            {
                                Target.transform.position = Car.transform.position;
                                OnShield = CheckShield(Target.transform.position.x, Target.transform.position.z);
                                Orbital_Mode = 1;
                                ServoAudio.Play();
                            }
                        }
                        else if (AttackMode == "Enemy")
                        {
                            DestroyAbleObjects = RayScript.destroyableObjects;
                            if (DestroyAbleObjects.Length > 1) //At least 5 Objects to operate
                            {
                                int randomIndex = Random.Range(0, DestroyAbleObjects.Length);
                                Target.transform.position = DestroyAbleObjects[randomIndex].transform.position;
                                if (Vector3.Distance(Target.transform.position, gameObject.transform.position) <= MaxRange)
                                {
                                    Target.transform.position = DestroyAbleObjects[randomIndex].transform.position;
                                    Orbital_Mode = 1;
                                    ServoAudio.Play();
                                }

                                //Debug.Log("Target Aquired");

                            }
                        }
                        else if (AttackMode == "Support")
                        {
                            DestroyAbleObjects = RayScript.destroyableObjects;
                            float shortestDistance = Mathf.Infinity;
                            Vector3 currentPos = Car.transform.position;
                            bool Assigned = false;
                            foreach (GameObject obj in DestroyAbleObjects)
                            {
                                float distance = Vector3.Distance(obj.transform.position, currentPos);
                                if (distance < shortestDistance && distance <= MaxRange)
                                {
                                    Target.transform.position = obj.transform.position;
                                    shortestDistance = distance;
                                    Assigned = true;
                                }

                            }
                            if (Assigned)
                            {
                                ServoAudio.Play();
                                Orbital_Mode = 1;
                            }


                        }
                        else if (AttackMode == "Friendly")
                        {
                            bool Assigned = false;
                            float shortestDistance = Mathf.Infinity;
                            Vector3 currentPos = Car.transform.position;
                            foreach (GameObject obj2 in FriendlyObjects)
                            {
                                float distance = Vector3.Distance(currentPos, obj2.transform.position);
                                if (distance < shortestDistance && distance <= MaxRange)
                                {
                                    Target.transform.position = obj2.transform.position;
                                    shortestDistance = distance;
                                    Assigned = true;
                                }
                            }
                            if (Assigned)
                            {
                                OnShield = CheckShield(Target.transform.position.x, Target.transform.position.z);
                                ServoAudio.Play();
                                Orbital_Mode = 1;
                            }
                        }

                    }
                }
                else if (Orbital_Mode == 1) //Aiming TurnBase
                {

                    Vector3 directionToTarget = Target.transform.position - TurnBase.transform.position;
                    directionToTarget.y = 0;


                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

                    TurnBase.transform.localRotation = Quaternion.RotateTowards(TurnBase.transform.localRotation, targetRotation, TurnBaseSpeed * Time.deltaTime);
                    if (Quaternion.Angle(TurnBase.transform.localRotation, targetRotation) <= 1 || Quaternion.Dot(TurnBase.transform.localRotation, targetRotation) >= 0.9999f)
                    {
                        StrikeObject.transform.position = Target.transform.position;
                        Orbital_Mode = 8;
                        ServoAudio.Stop();
                        ServoAudio.pitch = Random.Range(0.7f, 1f);
                    }
                }
                else if (Orbital_Mode == 2) //Aiming Barrel
                {
                    Barrel.transform.Rotate(new Vector3(0, TurnBarrelSpeed * Time.deltaTime, 0), Space.Self);

                    // Check if the Barrel has reached its target angle using localRotation
                    if (Barrel.transform.localRotation.eulerAngles.y >= Barrel_Angle)
                    {
                        Orbital_Mode = 7;
                        ServoAudio.Stop();
                    }
                }
                else if (Orbital_Mode == 3) //Firing
                {
                    if (!BarrelIsMovingBack)
                    {
                        Barrels.transform.localPosition = new Vector3(Barrels.transform.localPosition.x - Barrels_Offset, Barrels.transform.localPosition.y, Barrels.transform.localPosition.z);
                        BarrelIsMovingBack = true;
                        ShootingParticles1.Play();
                        ShootingAudio.pitch = Random.Range(0.8f, 1.2f);
                        ShootingAudio.Play();
                        if (Vector3.Distance(gameObject.transform.position, Car.transform.position) <= 30f)
                        {
                            if (Vector3.Distance(gameObject.transform.position, Car.transform.position) <= 15f)
                            {
                                ShakeHandling(10f);
                            }
                            else
                            {
                                ShakeHandling(6f);
                            }
                        }
                        //Add Force
                        //float ForceToAdd = 0;
                        //if (Vector3.Distance(gameObject.transform.position, Car.transform.position) <= 150f)
                        //{
                        //    ForceToAdd = 500 / Vector3.Distance(gameObject.transform.position, Car.transform.position);
                        //}
                        //Rigidbody CarRB = Car.GetComponent<Rigidbody>();
                        //Vector3 randomDirection = Random.insideUnitSphere;
                        //if (randomDirection.y > 0)
                        //{
                        //    randomDirection.y = 0f;
                        //}
                        //CarRB.AddForce(randomDirection * ForceToAdd, ForceMode.Impulse);
                        //End
                        StartCoroutine("AttackObject");

                    }
                    else if (BarrelIsMovingBack)
                    {

                        Barrels.transform.localPosition = Vector3.Lerp(Barrels.transform.localPosition, barrelsOriginalPosition, Barrels_SnapBackSpeed * Time.deltaTime);

                        if (Barrels.transform.localPosition.x - barrelsOriginalPosition.x >= -0.1f)
                        {
                            Barrels.transform.localPosition = barrelsOriginalPosition;
                            Orbital_Mode = 6;
                            BarrelIsMovingBack = false;
                        }

                    }
                }
                else if (Orbital_Mode == 4)
                {

                    Barrel.transform.Rotate(new Vector3(0, TurnBarrelSpeed * Time.deltaTime * -1f, 0), Space.Self);

                    // Check if the Barrel has reached its target angle
                    if (Barrel.transform.localRotation.y <= 0f)
                    {
                        Orbital_Mode = 5;
                        ServoAudio.Stop();
                        WaitTimer = 0f;
                    }
                }
                else if (Orbital_Mode == 5)
                {
                    WaitTimer += Time.deltaTime;

                    if (WaitTimer >= TargetWaitTime)
                    {
                        Orbital_Mode = 0;
                        WaitTimer = 0f;
                    }

                }
                else if (Orbital_Mode == 6)
                {
                    WaitTimer += Time.deltaTime;

                    if (WaitTimer >= TargetWaitTime)
                    {
                        Orbital_Mode = 4;
                        WaitTimer = 0f;
                        ServoAudio.Play();
                    }

                }
                else if (Orbital_Mode == 7)
                {
                    WaitTimer += Time.deltaTime;

                    if (WaitTimer >= TargetWaitTime * 2)
                    {
                        Orbital_Mode = 3;
                        WaitTimer = 0f;
                    }

                }
                else if (Orbital_Mode == 8)
                {
                    WaitTimer += Time.deltaTime;

                    if (WaitTimer >= TargetWaitTime * 2)
                    {
                        Orbital_Mode = 2;
                        WaitTimer = 0f;
                        ServoAudio.Play();
                    }

                }
                else if (Orbital_Mode == 9)
                {
                    WaitTimer += Time.deltaTime;

                    if (WaitTimer >= Random.Range(0.1f, 3f))
                    {
                        Orbital_Mode = 0;
                        WaitTimer = 0f;
                        //ServoAudio.Play();
                    }

                }
            }
            else
            {
                ServoAudio.volume = 0f;
            }
        }
    }

    public IEnumerator AttackObject()
    {
        float TimeTillInpact = 5f + (Vector3.Distance(Barrels.transform.position, Target.transform.position) / 500f);
        //Debug.Log("TimeInpact: " + TimeTillInpact.ToString());



        yield return new WaitForSeconds(TimeTillInpact);

        OnShield = CheckShield(Target.transform.position.x, Target.transform.position.z);
        if (OnShield)
        {
            PartObjectSmall.transform.position = Target.transform.position;
            PartObjectSmall.SetActive(true);
            PartSys2.Play();
        } else
        {
            

            PartObject.transform.position = Target.transform.position;
            PartObjectSmall.transform.position = Target.transform.position;
            PartObjectOrbit.transform.position = Target.transform.position;
            PartObject.SetActive(true);
            PartObjectSmall.SetActive(true);
            PartObjectOrbit.SetActive(true);
            PartSys.Play();
            PartSys2.Play();
            PartSys3.Play();
        }

        BoomSound.pitch = Random.Range(0.8f, 1.2f);
        BoomSound.Play();
        BoomSound2.pitch = Random.Range(0.8f, 1.2f);
        BoomSound2.Play();

        //Add Damage and Forces
        if (AttackMode == "Car" || AttackMode == "Friendly")
        {
            if (OnShield)
            {
                //Subtract ShieldHP
                GameObject[] Shields = GameObject.FindGameObjectsWithTag("Shield");
                float CloseDis = Mathf.Infinity;
                float distance = 0f;
                float MaxRangeShield = 10000f; //Isnt this dumb because the target is on shield anyways
                GameObject NearestShield = null;
                //float MaxRangeShield = 20f; //Max Range, detects if shield has been broken before
                foreach (GameObject obj in Shields){
                    if (obj != null)
                    {
                        distance = Vector3.Distance(Target.transform.position, obj.transform.position);
                        if (distance <= CloseDis && distance <= MaxRangeShield)
                        {
                            CloseDis = distance;
                            NearestShield = obj;
                        }
                    }
                }
                if (NearestShield != null)
                {
                    HealthSystem NearestShieldHPSys = NearestShield.GetComponent<HealthSystem>();
                    if (NearestShieldHPSys != null)
                    {
                        NearestShieldHPSys.Health -= Damage;
                        //Debug.Log("Subtracted Shield Dmg");
                    } else
                    {
                        Debug.LogWarning("Neatest Shield: HealthScript not found");
                    }

                    if (Vector3.Distance(NearestShield.transform.position, Car.transform.position) < 30f)
                    {
                        ShakeHandling(8f);
                    }
                } else
                {
                    Debug.LogWarning("Neareast Shield was not found.");
                }
                

            } else { 
                FriendlyDmg();
                CarRB();
            }
        }
        else if (AttackMode == "Enemy" ||AttackMode == "Support")
        {
            List<GameObject> nearbyObjects = new List<GameObject>();
            foreach (GameObject destroyableObject in DestroyAbleObjects)
            {
                if (destroyableObject != null)
                {
                    float distance = Vector3.Distance(destroyableObject.transform.position, Target.transform.position);
                    if (distance <= 50f)
                    {
                        nearbyObjects.Add(destroyableObject);
                    }
                }
            }

            // Apply random force to each nearby object
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
                    HPSys.Health -= Damage;
                }
            }
        }







    }


    void AssignMaterial()
    {


        if (AttackMode == "Car" || AttackMode == "Friendly")
        {
            Renderer PR = GetComponent<Renderer>();
            gameObject.tag = "DestroyableObject";
            PR.material = EnemyMaterial;
            HitBox.tag = "DestroyableObject";





        }
        else
        {
            Renderer PR = GetComponent<Renderer>();
            PR.material = NeutralMaterial;
        }
    }
    void FriendlyDmg()
    {
        List<GameObject> nearbyObjects = new List<GameObject>();
        foreach (GameObject FriendObj in FriendlyObjects)
        {
            if (FriendObj != null)
            {
                float distance = Vector3.Distance(FriendObj.transform.position, Target.transform.position);
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
                HPSys.Health -= Damage;
            }
        }
    }
    void CarRB()
    {
        if (Vector3.Distance(Car.transform.position, Target.transform.position) <= 50f)
        {

            Rigidbody CarRB = Car.GetComponent<Rigidbody>();
            float CurDistance = Vector3.Distance(Car.transform.position, Target.transform.position);
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

    bool CheckShield(float x, float z)
    {
        Vector3 PosRay = new Vector3(x, 100f, z);
        Ray ray = new Ray(PosRay, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f)){
            Target.transform.position = new Vector3(Target.transform.position.x, hit.point.y + 0.1f, Target.transform.position.z);
            if (hit.collider.CompareTag("Shield"))
            {
                //Debug.Log("Shield");
                // Assign Target
                return true;

            } else
            {
                //Debug.Log("No Shield");
                return false;
            }


        } else
        {
            Debug.LogWarning("SheildRaycastError");
        }
        return false;

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
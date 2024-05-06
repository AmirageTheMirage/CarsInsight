using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingCubes : MonoBehaviour
{
    public GameObject Car;
    public float Range = 110f;
    public float TurnSpeed = 10f;
    public float JumpForce = 10f;
    private Rigidbody MyRB;
    public bool InRange = false;
    public bool Grounded = true;
    public GameObject PartSysObj;
    public Transform PartSysParent;
    public Transform NewParent;
    public float ImpForce = 10f;
    public float Dmg = 30f;
    private bool AssignedNew = false;
    public GameObject Target;
    public string Mode = "Car"; //Car: Attacks Car, Friendly = Attacks any Friendly Object
    public bool WallInFront = false;
    public float DthRange = 3f;
    public DayNightCycle DayScr;
    //public bool NonStationary = false;
    [Space]
    public float DestroyTimer = 120f;
    public bool UseDestroyTimer = false;
    //public ParticleSystem ActualPartSys;
    private float GroundedCoolDown = 1f;
    private float SeenCoolDown = 2f;
    private ForceAdding ForScript;
    private Quaternion targetRotation;
    private bool HighJump = false;
    
    
    void Start()
    {
        MyRB = gameObject.GetComponent<Rigidbody>();
        ForScript = NewParent.gameObject.GetComponent<ForceAdding>();
        if (Mode == "Car")
        {
        Target = Car;

        } else if (Mode == "Friendly")
        {
            FetchFriendly();

        }


    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= 300f || transform.position.y <= -300f)
        {
            Destroy(gameObject);
        }
        if (UseDestroyTimer)
        {
            if (DestroyTimer > 0f)
            {
                DestroyTimer -= Time.deltaTime;
            } else
            {
                GameObject Nobj = Instantiate(PartSysObj);
                Nobj.transform.position = transform.position;
                Nobj.transform.parent = PartSysParent;
                //ParticleSystem ActualPartSys = Nobj.GetComponent<ParticleSystem>();
                Nobj.SetActive(true);
                ForScript.FriendlyDmg(ImpForce, Dmg, gameObject, DthRange);
                ForScript.CarRB(ImpForce, gameObject);
            }
        }
        if (Target == null)
        {
            
            FetchFriendly();
        } else
        {

        
            if (Vector3.Distance(Target.transform.position, gameObject.transform.position) <= 5f)
            {
                DestSequence();

            }
            //Check if Car is in Proximity
            if (Mode == "Car")
            {
                if (Vector3.Distance(Target.transform.position, gameObject.transform.position) <= Range)
                {

                    InRange = true;
                    SeenCoolDown -= Time.deltaTime;
                }
                else
                {
                    SeenCoolDown = 2f;
                    InRange = false;
                }
            } else
            {
                SeenCoolDown = 0f;
                InRange = true;
                if (DayScr.Day == true)
                {
                    DestSequence();
                }
            }

            //Check if Grounded
            RaycastHit hit;
            float distance = 10f;

            WallInFront = CheckWall(targetRotation.eulerAngles);
            if (HighJump)
            {
                MyRB.AddForce(transform.forward.normalized * JumpForce / 10f * Time.deltaTime, ForceMode.Impulse);
            }
            if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, distance))
            {
                float HeightAboveGround = Vector3.Distance(hit.point, transform.position);
                if (HeightAboveGround <= 0.9f)
                {
                    Grounded = true;
                    GroundedCoolDown -= Time.deltaTime;
                    
                    if (GroundedCoolDown <= 0f && InRange && SeenCoolDown <= 0f)
                    {
                        //Jump Up & forward
                        if (HighJump)
                        {
                            HighJump = false;
                        }
                        Vector3 forwardDirection = transform.forward.normalized;
                        WallInFront = CheckWall(transform.forward);
                        
                        if (WallInFront)
                        {
                            HighJump = true;
                            MyRB.AddForce(Vector3.up * JumpForce * 2.1f, ForceMode.Impulse);
                            MyRB.AddForce(forwardDirection * JumpForce / 10f, ForceMode.Impulse);
                        } else
                        {
                            MyRB.AddForce(Vector3.up * JumpForce / 2f, ForceMode.Impulse);
                            MyRB.AddForce(forwardDirection * JumpForce / 1.5f, ForceMode.Impulse);
                        }
                        
                        
                        Grounded = false;
                        GroundedCoolDown = Random.Range(0.1f, 0.7f);

                    }
                }
                else
                {
                    Grounded = false;
                    GroundedCoolDown = Random.Range(0.1f, 0.7f);
                }
            }
            else
            {
                Grounded = false;
            }

            if (InRange)
            {
                if (AssignedNew == false)
                {
                    AssignedNew = true;
                    transform.parent = NewParent;
                }
                //Turn towards Car
                Vector3 dire = transform.position - Target.transform.position;
                dire.y = 0f;
                targetRotation = Quaternion.LookRotation(-dire);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);

            }
        }
    }

    private void FetchFriendly()
    {
        
        GameObject[] MyFriendlies = ForScript.FriendlyObjects;
        if (MyFriendlies.Length > 0){
        int Rand = Mathf.FloorToInt(Random.Range(0f, (float)MyFriendlies.Length));
        Target = MyFriendlies[Rand];
        } else
        {
            Target = null;
        }

    }
    bool CheckWall(Vector3 dir)
    {
        Vector3 MyPos = transform.position;
        //MyPos += -Vector3.forward * 1.2f;

        Ray ray = new Ray(MyPos, dir);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f))
        {
            //Debug.DrawLine(MyPos, hit.point);
            if (hit.collider.tag == "DestroyableObject")
            {
                return false;
            }
            else
            {
                return true;
            }
        } else
        {
            //Debug.DrawLine(MyPos, hit.point);
            return false;
        }
    }


    void DestSequence()
    {
        GameObject Nobj = Instantiate(PartSysObj);
        Nobj.transform.position = transform.position;
        Nobj.transform.parent = PartSysParent;
        //ParticleSystem ActualPartSys = Nobj.GetComponent<ParticleSystem>();
        Nobj.SetActive(true);
        ForScript.FriendlyDmg(ImpForce, Dmg, gameObject, DthRange);
        ForScript.CarRB(ImpForce, gameObject);

        Destroy(gameObject);
    }
}

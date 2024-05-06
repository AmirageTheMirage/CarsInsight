using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public GameObject Actual;
    public GameObject Target;
    public GameObject Car;
    
    [Space]
    public float MovementSpeed = 0f;
    public float MaxMovementSpeed = 10f;
    public float MaxAcc = 4f; //Per Second
    public float TurnSpeed = 30f;
    public float DistanceToTarget = 99999f;
    public float RemainingChaseTime = 0f;
    public float MinChaseTime = 10f;
    public float MaxChaseTime = 30f;
    [Space]
    //FlyBySection
    public GameObject FlyByTarget1Middle;
    public GameObject FlyBySpawnMiddle;
    public GameObject FlyByTarget;
    public GameObject FlyBySpawn;
    public GameObject FlyByFinalTarget;
    public float FlyBySpeed = 100f;
    
    private bool FlyByFinalPhase = false;
    [Space]
    public AudioSource FlyingSound;
    public bool AudioEnabled = true;
    public bool BInitiateChase = false;
    public enum MyDrop
    {
        Unassigned,
        Chase,
        FlyBy,
        QuickLeave
    }
    public MyDrop SelectedOption;
    void Start()
    {
        
        FlyByFinalPhase = false;
        if (!AudioEnabled)
        {
            FlyingSound.volume = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (BInitiateChase)
        {
            BInitiateChase = false;
            InitiateChase();
        }
        if (SelectedOption == MyDrop.Chase) //Mode: Chase
        {
            RemainingChaseTime -= Time.deltaTime;
            if (RemainingChaseTime < 0f)
            {
                RemainingChaseTime = 0f;
                InitiateQuickLeave();
            }
            //Update Target Position
            Target.transform.position = new Vector3(Car.transform.position.x, 60f, Car.transform.position.z);
            //Look towards Target

            LookTowards(Target, TurnSpeed);
            //Move
            DistanceToTarget = Vector3.Distance(Target.transform.position, Actual.transform.position);
            if (DistanceToTarget > 30f)
            {
                if (MovementSpeed < MaxMovementSpeed)
                {
                    MovementSpeed += MaxAcc * Time.deltaTime;
                }
                else
                {
                    MovementSpeed = MaxMovementSpeed;
                }
            }
            else
            {
                if (MovementSpeed > 0f)
                {
                    MovementSpeed -= MaxAcc * 3f * Time.deltaTime;
                }
                else
                {
                    MovementSpeed = 0f;
                }
            }

            Debug.DrawLine(Target.transform.position, Actual.transform.position);
            //Vector3 direction = (Target.transform.position - Actual.transform.position).normalized;
            Move(MovementSpeed);
        }
        else if (SelectedOption == MyDrop.FlyBy)
        {

            //Debug.Log("Done, FlyingPath is Set up!");
            FlyByTarget1Middle.transform.position = new Vector3(Car.transform.position.x, 60f, Car.transform.position.z);

            //Move
            if (FlyByFinalPhase)
            {

                //LookTowards(FlyByFinalTarget, 9999f);
                Move(FlyBySpeed);
                if (Vector3.Distance(Car.transform.position, Actual.transform.position) >= 2000f)
                {
                    //Debug.Log("FlyBy completed");
                    if (Random.Range(0f, 3f) < 1f)
                    {
                        InitiateFlyBy();
                    }
                    else
                    {
                        InitiateChase();

                    }

                    FlyByFinalPhase = false;
                    
                }
            }
            else
            {
                LookTowards(FlyByTarget, 9999f);
                Move(FlyBySpeed);
                float MyDis = Vector3.Distance(FlyByTarget.transform.position, Actual.transform.position);
                //Debug.Log(MyDis);
                if (MyDis <= 40f)
                {

                    FlyByFinalPhase = true;
                    FlyByFinalTarget.transform.position = Actual.transform.position;
                    Vector3 MyVec = FlyByTarget.transform.position - Actual.transform.position; //Get Vector normalized

                    while (Vector3.Distance(MyVec, Vector3.zero) < 400f) //Make Vector3 bigger until its a certain length
                    {
                        MyVec *= 2f;
                    }
                    MyVec.y = 0f;
                    FlyByFinalTarget.transform.position += MyVec;

                    //Rotate with infinite Speed:
                    Vector3 dire = Actual.transform.position - FlyByFinalTarget.transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(-dire);
                    Actual.transform.rotation = Quaternion.RotateTowards(Actual.transform.rotation, targetRotation, 99999999999999f * Time.deltaTime);


                }
            }




        }
        else if (SelectedOption == MyDrop.QuickLeave)
        {
            LookTowards(FlyBySpawn, TurnSpeed);
            if (MovementSpeed < MaxMovementSpeed)
            {
                MovementSpeed += MaxAcc * Time.deltaTime;
            }
            else
            {
                MovementSpeed = MaxMovementSpeed;
            }
            Move(MovementSpeed);
            if (Vector3.Distance(FlyBySpawn.transform.position, Actual.transform.position) <= 80f)
            {
                if (Vector3.Distance(Car.transform.position, Actual.transform.position) <= 200f)
                {
                    InitiateQuickLeave();
                }
                else
                {
                    if (Random.Range(0f, 2f) < 1f)
                    {
                        InitiateFlyBy();
                    }
                    else
                    {
                        InitiateChase();

                    }
                }
            }

        }
        void LookTowards(GameObject obj, float LocalTurnSpeed)
        {
            Vector3 dire = Actual.transform.position - obj.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(-dire);
            Actual.transform.rotation = Quaternion.RotateTowards(Actual.transform.rotation, targetRotation, LocalTurnSpeed * Time.deltaTime);
        }


        void InitiateChase()
        {

            FlyBySpawnMiddle.transform.position = new Vector3(Car.transform.position.x, 40f, Car.transform.position.z);
            FlyBySpawnMiddle.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));
            Actual.transform.position = FlyBySpawn.transform.position;
            RemainingChaseTime = Random.Range(MinChaseTime, MaxChaseTime);
            SelectedOption = MyDrop.Chase;
            FlyingSound.Play();
        }

        void InitiateQuickLeave(){


            FlyBySpawnMiddle.transform.position = new Vector3(Car.transform.position.x, 40f, Car.transform.position.z);
            FlyBySpawnMiddle.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));
            //Actual.transform.position = FlyBySpawn.transform.position;
            SelectedOption = MyDrop.QuickLeave;
        }


        void Move(float Spee)
        {       
            Actual.transform.Translate(transform.forward.normalized * Spee * Time.deltaTime);
        }

        void InitiateFlyBy()
        {
            
            // Set up the Flying Route here
            //So: Target will still be the same above the car, but there will be a second one around 20f beside the Car-Target
            //This will be the FlyBy-Target.
            //The RotationSpeed will now be endless (so around 360) so "Actual" will reach the FlyByTarget at one point.
            //Finally, once the Distance to "FlyByTarget" is small enough, a second Target will be spawned further away from the car.
            //"Actual" will then fly towards that second target
            FlyByTarget1Middle.transform.position = new Vector3(Car.transform.position.x, 40f, Car.transform.position.z);
            FlyByTarget1Middle.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));

            FlyBySpawnMiddle.transform.position = new Vector3(Car.transform.position.x, 40f, Car.transform.position.z);
            FlyBySpawnMiddle.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));
            Actual.transform.position = FlyBySpawn.transform.position;
            FlyByFinalPhase = false;
            
            SelectedOption = MyDrop.FlyBy;
        }
    }
}

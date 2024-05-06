using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AutoDriving : MonoBehaviour
{
    public float CurrentSpeed = 0f;
    
    public float Acceleration = 10f;
    public float RotationSpeed = 100f;
    public Image SpeedsterUI;
    public Rigidbody rb;
    public float MaximumSpeed = 0f;
    public float MaximumMaximumSpeed = 100f;
    public bool BISGrounded = true;
    public GameObject Car;
    public float HeightAboveGround = 0f;
    public GameObject RightBooster;
    public GameObject LeftBooster;
    private Vector3 RightBoosterTarget;
    private Vector3 LeftBoosterTarget;
    public float BoosterSpeed = 1f;
    public float CarAirSpeed = 1f;
    public GameObject LBoosterPart;
    public GameObject RBoosterPart;
    public float AccChange = 20f;

    private ParticleSystem LBPart;
    private ParticleSystem RBPart;
    private bool FirstTimeReset = true;
    private bool CarCanTurnInAir = true;
    private Vector3 MyZero = new Vector3(0f, 0f, 0f);
    public float HalfDragAt = 0.5f;
    private float OriginDrag;
    public AudioSource CarSound;
    [Space]
    public bool AllowSpeedToggle = false;
    private bool SToggle = false;
    public float N_MaxMaxSpeed = 1000f;
    public float N_AccChange = 100f;
    public float N_Acceleration = 300f;

    public ParticleSystem BackR;
    public ParticleSystem BackL;
    public ParticleSystem FrontR;
    public ParticleSystem FrontL;
    private bool IsPlayingBack = false;

    public float GraduallyChangeCarSound = 0f;
    private bool CarSoundChange = true;
    public bool UseKeyboard = true;
    public GlobalVars GlobalScript;
    private bool BuildingMode;
    private bool EditingMode;
    private float NormSpeed;
    private float NormAcc;
    private float NormChange;

    void Start()
    {
        Application.targetFrameRate = 300; //Unlimited FPS :)
        NormSpeed = MaximumMaximumSpeed;
        NormAcc = Acceleration;
        NormChange = AccChange;
        //Get Building Mode Var:
        BuildingMode = GlobalScript.BuildingMode;
        BackR.Stop();
        BackL.Stop();
        FrontR.Stop();
        FrontL.Stop();
        CarSound.Play();
        LBPart = LBoosterPart.GetComponent<ParticleSystem>();
        RBPart = RBoosterPart.GetComponent<ParticleSystem>();
        LBPart.Stop();
        RBPart.Stop();
        RightBoosterTarget = RightBooster.transform.localPosition;
        LeftBoosterTarget = LeftBooster.transform.localPosition;
        RightBooster.transform.localPosition = new Vector3(0f, 0f, 0f);
        LeftBooster.transform.localPosition = new Vector3(0f, 0f, 0f);
        rb.centerOfMass = new Vector3(0, -1, 0);
        Cursor.visible = true;
        OriginDrag = rb.drag;
    }


    void Update()
    {
        if (AllowSpeedToggle)
        {
            if (Input.GetKeyDown("x"))
            {
                if (SToggle)
                {
                    SToggle = false;
                    MaximumMaximumSpeed = N_MaxMaxSpeed;
                    Acceleration = N_Acceleration;
                    AccChange = N_AccChange;

                } else
                {
                    SToggle = true;
                    MaximumMaximumSpeed = NormSpeed;
                    Acceleration = NormAcc;
                    AccChange = NormChange;
                }
            }
        }
        BuildingMode = GlobalScript.BuildingMode;
        EditingMode = GlobalScript.Editing;
        if (HeightAboveGround <= 1f)
        {
            if (CarSoundChange == false)
            {
                GraduallyChangeCarSound = CarSound.pitch;
            }
            //Debug.Log(0.5f + 2f * (MaximumMaximumSpeed / CurrentSpeed));
            if (GraduallyChangeCarSound <= 0.5f + 2f * (CurrentSpeed / MaximumMaximumSpeed)) //MaxSpeedPercentage * 10
            {
                GraduallyChangeCarSound += 1f * Time.deltaTime;
            }
            else if (GraduallyChangeCarSound >= 0.5f + 2f * (CurrentSpeed / MaximumMaximumSpeed))
            {
                GraduallyChangeCarSound -= 1f * Time.deltaTime;
            }
            CarSound.pitch = GraduallyChangeCarSound;
            CarSoundChange = true;
        }
        else
        {
            if (CarSoundChange)
            {
                GraduallyChangeCarSound = CarSound.pitch;
            }
            if (GraduallyChangeCarSound >= 0.5f)
            {
                GraduallyChangeCarSound -= 1f * Time.deltaTime;
            }
            CarSound.pitch = GraduallyChangeCarSound;
            CarSoundChange = false;
            //BasePitch = 0.5f;
        }
        IsGrounded();

        //Debug.Log((MaximumMaximumSpeed * HalfDragAt));
        //Debug.Log(MaximumMaximumSpeed);

        if (CurrentSpeed >= MaximumMaximumSpeed * HalfDragAt)
        {
            rb.drag = OriginDrag / 1.5f; //Not /2, for now
        }
        else
        {
            rb.drag = OriginDrag;
        }
        if (HeightAboveGround <= 2f && (CurrentSpeed / MaximumMaximumSpeed) >= 0.05f && MaximumSpeed > 0.1f)
        {
            if (IsPlayingBack == false)
            {
                BackR.Play();
                FrontR.Play();
                BackL.Play();
                FrontL.Play();
                IsPlayingBack = true;
            }
        }
        else
        {
            IsPlayingBack = false;
            BackR.Stop();
            FrontR.Stop();
            BackL.Stop();
            FrontL.Stop();
        }
        if (HeightAboveGround >= 5f)
        {
            if (FirstTimeReset)
            {
                FirstTimeReset = false;
                rb.angularVelocity = new Vector3(0f, 0f, 0f); //rb.angularVelocity.x for example

                LBPart.Play();
                RBPart.Play();
                //var Lemission = LBPart.emission;
                //Lemission.rateOverTime = MaximumSpeed;
                //var Remission = LBPart.emission;
                //Remission.rateOverTime = MaximumSpeed;
            }
            //Quaternion TarRotation = Quaternion.Euler(0f, Car.transform.eulerAngles.y, 0f);
            if (CarCanTurnInAir)
            {
                CarCanTurnInAir = false;
                Car.transform.rotation = Quaternion.Euler(0f, Car.transform.eulerAngles.y, 0f);
                //Car.transform.rotation = Quaternion.Lerp(Car.transform.rotation, Quaternion.Euler(0f, Car.transform.eulerAngles.y, 0f), Time.deltaTime * CarAirSpeed);
                //float angleDifference = Quaternion.Angle(Car.transform.rotation, Quaternion.Euler(0f, Car.transform.eulerAngles.y, 0f));
                //if (angleDifference <= 0.1f)
                //{
                // CarCanTurnInAir = false;
                //}
                //Car.transform.rotation = Quaternion.Lerp(Car.transform.rotation, TarRotation, Time.deltaTime * CarAirSpeed);
            }
            RightBooster.transform.localPosition = Vector3.Lerp(RightBooster.transform.localPosition, RightBoosterTarget, Time.deltaTime * BoosterSpeed);
            LeftBooster.transform.localPosition = Vector3.Lerp(LeftBooster.transform.localPosition, LeftBoosterTarget, Time.deltaTime * BoosterSpeed);
        }
        else
        {
            LBPart.Stop();
            RBPart.Stop();
            FirstTimeReset = true;
            CarCanTurnInAir = true;
            RightBooster.transform.localPosition = Vector3.Lerp(RightBooster.transform.localPosition, MyZero, Time.deltaTime * BoosterSpeed);
            LeftBooster.transform.localPosition = Vector3.Lerp(LeftBooster.transform.localPosition, MyZero, Time.deltaTime * BoosterSpeed);
        }

        

        CurrentSpeed = rb.velocity.magnitude;
        if (CurrentSpeed < MaximumSpeed && HeightAboveGround <= 1f || CurrentSpeed < MaximumSpeed && CarCanTurnInAir == false)
        {
            if (BuildingMode == false && EditingMode == false)
            {
                Vector3 ForceToBeAdded = transform.forward * Acceleration * Time.deltaTime * 1000f;
                rb.AddForce(ForceToBeAdded);
                //Debug.Log(ForceToBeAdded);
            } else
            {
                MaximumSpeed = 0f;
            }
        }

        float rotation = 0;
        if (Input.GetMouseButton(0) && UseKeyboard == false || Input.GetKey(KeyCode.A) && UseKeyboard || Input.GetKey(KeyCode.LeftArrow) && UseKeyboard)
        {
            if (BuildingMode == false && EditingMode == false)
            {
                rotation = -1;
            }
        }
        else if (Input.GetMouseButton(1) && !UseKeyboard || Input.GetKey(KeyCode.D) && UseKeyboard || Input.GetKey(KeyCode.RightArrow) && UseKeyboard)
        {
            if (BuildingMode == false && EditingMode == false)
            {
                rotation = 1;
            }
        }

        rotation *= RotationSpeed * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotation);
        rb.MoveRotation(rb.rotation * deltaRotation);

        //from -50 to 50
        RectTransform rectTransform = SpeedsterUI.GetComponent<RectTransform>();
        Vector2 targetPosition = new Vector2(rectTransform.anchoredPosition.x, -100f + (2f * MaximumSpeed / (MaximumMaximumSpeed / 100f)));
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * 5f);

        Vector2 scrollDelta = Input.mouseScrollDelta;
        if (BuildingMode == false)
        {
            if (scrollDelta.y != 0 || UseKeyboard)
            {
                if (UseKeyboard)
                {
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    {
                        MaximumSpeed += AccChange * Time.deltaTime * 6f;
                    }
                    else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    {
                        MaximumSpeed -= AccChange * Time.deltaTime * 6f;
                    }
                }
                else
                {

                    if (scrollDelta.y > 0)
                    {

                        MaximumSpeed += AccChange;
                    }
                    else if (scrollDelta.y < 0)
                    {
                        MaximumSpeed -= AccChange;
                    }

                }

                MaximumSpeed = Mathf.Clamp(MaximumSpeed, 0, MaximumMaximumSpeed);
            }
        }
    }
    void IsGrounded()
    {
        RaycastHit hit;
        float distance = 100f;
        Vector3 dir = new Vector3(0, -1);

        if (Physics.Raycast(transform.position, dir, out hit, distance))
        {
            HeightAboveGround = Vector3.Distance(hit.point, transform.position);
            if (HeightAboveGround <= 1f)
            {
                BISGrounded = true;
            }
            else
            {
                BISGrounded = false;
            }
        }
        
    }
}

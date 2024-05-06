using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float MaxHealth;
    public float Health;
    public Slider SliderObject;
    public GameObject HealthObject;
    public GameObject Camera;
    public GameObject ParticleSystem;
    public GameObject PartSysParent;
    public int ScoreOnDestroy = 0;
    public ScoreScript ScoreS;
    public bool AlwaysShowHPBar = false;
    [Header("Optional")]
    public GameObject DestroyInstead;
    
    //public bool IsCamp = false;
    [Tooltip("If Destroying Camp-Center:")]
    public ClearCamp CampScript;
    public ParticleSystem ShieldParts;
    public GameObject PartBlueObj;
    public PauseScript PausScr;
    private float EmissionRate = 0f;
    private bool UseShield = false;
    private BoxCollider BoxColliderShield;
    private float HealthBefore;
    public bool EndGameOnDestroy = false;
    

    void Start()
    {
        Health = MaxHealth;
        HealthBefore = Health;
        if (ShieldParts != null)
        {
            BoxColliderShield = ShieldParts.gameObject.GetComponent<BoxCollider>();
            UseShield = true;
            EmissionRate = ShieldParts.emission.rateOverTime.constant;
            //Debug.Log(EmissionRate);

        } else
        {
            UseShield = false;
        }
    }

    void FixedUpdate()
    {
        if (HealthBefore > Health) //Lost HP
        {
            if (PartBlueObj != null)
            {
                GameObject NewPart = Instantiate(PartBlueObj);
                NewPart.transform.parent = ShieldParts.gameObject.transform;
                NewPart.transform.position = new Vector3(ShieldParts.gameObject.transform.position.x, ShieldParts.gameObject.transform.position.y + 0.1f, ShieldParts.gameObject.transform.position.z);
                NewPart.SetActive(true);
            }
        }

        
        HealthBefore = Health;
        if (UseShield)
        {
            //Assign Amount of emission based on HP;
            var emissionModule = ShieldParts.emission;
            float NewEmission = Health / MaxHealth * EmissionRate;
            emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(NewEmission);
            if (BoxColliderShield != null)
            {
                if (Health <= 0) {
                    Health = 0;
                    BoxColliderShield.enabled = false;
                } else
                {
                    BoxColliderShield.enabled = true;

                }
            }


        }
        if (Health >= MaxHealth)
        {
            Health = MaxHealth;
        }
        if (Health <= 0)
        {
            Health = 0;
            if (EndGameOnDestroy)
            {
                PausScr.Death();
            }
            if (!UseShield)
            {
            GameObject PartSysInstantiate = Instantiate(ParticleSystem);
            PartSysInstantiate.transform.parent = PartSysParent.transform;
            PartSysInstantiate.transform.position = gameObject.transform.position;
            PartSysInstantiate.SetActive(true);
            ParticleSystem PartSys = PartSysInstantiate.GetComponent<ParticleSystem>();
            PartSys.Play();
            Destroy(PartSysInstantiate, 5f);
            ScoreS.AddScore(ScoreOnDestroy);
                if (DestroyInstead != null)
                {
                    Destroy(DestroyInstead);
                }
                else
                {
                    Destroy(gameObject);
                }

                if (CampScript != null)
                {
                    //Debug.Log("Yes");
                    CampScript.StartCall();
                    //StartCoroutine(CampScript.ShowText());
                }
            }
        }
        if (Vector3.Distance(Camera.transform.position, gameObject.transform.position) >= 250f || Health == MaxHealth && !AlwaysShowHPBar)
        {
            HealthObject.SetActive(false);
        } else {
            if (AlwaysShowHPBar)
            {
                HealthObject.SetActive(true);
                SliderObject.value = Health / MaxHealth;
                Vector3 directionToCamera = Camera.transform.position - HealthObject.transform.position;
                Vector3 oppositeDirection = -directionToCamera;
                HealthObject.transform.rotation = Quaternion.LookRotation(oppositeDirection);
            }
            else
            {


            
                HealthObject.SetActive(true);
                SliderObject.value = Health / MaxHealth;
                Vector3 directionToCamera = Camera.transform.position - HealthObject.transform.position;
                Vector3 oppositeDirection = -directionToCamera;
                HealthObject.transform.rotation = Quaternion.LookRotation(oppositeDirection);

                
            }
        }
        
    }
}

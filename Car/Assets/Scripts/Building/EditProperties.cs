using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditProperties : MonoBehaviour
{
    public string ID = "";
    public float CamDistance;
    public GameObject LockAtObj;
    public bool StatusActive = true;
    
    [Space]
    [Tooltip("Structure Personal Variables here")]
    public float Storage_StoredAmount = 0f;
    public float Storage_MaxStoredAmount = 0f;
    public float StorTransfer = 0f;
    public int Regenerator_Mode = 1;
    public int Regenerator_Priority = 1; //1 = Random, 2 = Shield, 3 = GameObjects
    public float Drill_TotalMined = 0f;
    public float Drill_MiningRate = 0f;

    [Space]
    //Definements
    // public MonoBehaviour DriverScript;
    public SpriteRenderer BackGColor;
    public SpriteRenderer IconColor;


    [Space]
    private bool ChangeAlpha;
    private bool ChangeAlphaUp;


    
    void Start()
    {
        ChangeAlpha = false;
        ChangeAlphaUp = false;
    }
    private void FixedUpdate()
    {
        if (ChangeAlpha)
        {
            Color newColor;
            if (ChangeAlphaUp)
            {
                newColor = BackGColor.color;
                newColor.a += 1f * Time.deltaTime;
                BackGColor.color = newColor;

                newColor = IconColor.color;
                newColor.a += 1f * Time.deltaTime;
                IconColor.color = newColor;
                if (newColor.a >= 1f)
                {
                    ChangeAlpha = false;
                    newColor = BackGColor.color;
                    newColor.a = 1f;
                    BackGColor.color = newColor;

                    newColor = IconColor.color;
                    newColor.a = 1f;
                    IconColor.color = newColor;
                }
            } else
            {
                newColor = BackGColor.color;
                newColor.a -= 1f * Time.deltaTime;
                BackGColor.color = newColor;

                newColor = IconColor.color;
                newColor.a -= 1f * Time.deltaTime;
                IconColor.color = newColor;
                if (newColor.a <= 0.5f)
                {
                    ChangeAlpha = false;
                    newColor = BackGColor.color;
                    newColor.a = 0.5f;
                    BackGColor.color = newColor;

                    newColor = IconColor.color;
                    newColor.a = 0.5f;
                    IconColor.color = newColor;
                }

            }
            
            
        }
    }
    public void TriggerChange()
    {
        
        if (StatusActive)
        {
            ChangeAlpha = true;
            ChangeAlphaUp = true;
        }
        else
        {
            ChangeAlpha = true;
            ChangeAlphaUp = false;
        }
    }

}

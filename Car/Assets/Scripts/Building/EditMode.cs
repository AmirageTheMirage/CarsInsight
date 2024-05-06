using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class EditMode : MonoBehaviour
{
    public GlobalVars Glob;
    public GameObject MiddleObject;
    public float TurnSpeed;
    private bool EditingMode;
    private bool OverUI;
    
    void Start()
    {
        EditingMode = Glob.Editing;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            OverUI = true;


        }
        else
        {
            OverUI = false;
        }
         
        EditingMode = Glob.Editing;
        if (EditingMode && !OverUI)
        {
            //AllowTurning
            //Debug.Log("AllowTurning");
            if (Input.GetMouseButton(0))
            {
                //Turn Left
                //Debug.Log("Mous");
                MiddleObject.transform.Rotate(0f, TurnSpeed * Time.deltaTime, 0f);

            }
            else if (Input.GetMouseButton(1))
            {
                //Turn Right
                MiddleObject.transform.Rotate(0f, -TurnSpeed * Time.deltaTime, 0f);

            }
        }

    }
}

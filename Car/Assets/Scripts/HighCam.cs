using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighCam : MonoBehaviour
{
    public GameObject CamObject;
    public GameObject TransformObject;
    public GlobalVars Glob;
    public Vector3 TargetVector = new Vector3(0f, 35f, -100f);
    public Vector3 OriginalVector;
    //public float LerpSpeed = 10f;
    private bool Toggle = false;


    public bool CanUseF = true; //Not F anymore

    void Start()
    {
        OriginalVector = TransformObject.transform.localPosition;
        Toggle = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Glob.BuildingMode == false)
        {
            if (Input.GetMouseButtonDown(2) || CanUseF && Input.GetKeyDown(KeyCode.Space))
            {
                if (Toggle)
                {
                    Toggle = false;
                    TransformObject.transform.localPosition = TargetVector;
                }
                else
                {
                    Toggle = true;
                    TransformObject.transform.localPosition = OriginalVector;
                }

                //if (Toggle)
                //{
                //    if (Vector3.Distance(TargetVector, TransformObject.transform.localPosition) >= 0.1f)
                //    {
                //        TransformObject.transform.localPosition = Vector3.Lerp(TransformObject.transform.localPosition, TargetVector, LerpSpeed * Time.deltaTime);
                //    }
                //}
                //else
                //{
                //    if (Vector3.Distance(OriginalVector, TransformObject.transform.localPosition) >= 0.1f)
                //    {
                //        TransformObject.transform.localPosition = Vector3.Lerp(TransformObject.transform.localPosition, OriginalVector, LerpSpeed * Time.deltaTime);
                //    }
                //}
            }
        }

    }
}

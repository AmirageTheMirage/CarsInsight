using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GracePeriod : MonoBehaviour
{
    public float Grace = 20f;
    public GameObject GraceParent;
    public TMP_Text Txt;
    public float FullWaitSeconds = 10f;
    [Space]
    public Color White;
    public Color Red;
    private float WaitSeconds;
    public GlobalVars GlobScr;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        


            if (Grace > 0f)
            {
                Txt.color = White;
                Grace -= Time.deltaTime;
                float DisplayGrace = Mathf.Round(Grace);
                Txt.text = "Grace Period: " + DisplayGrace.ToString() + "s";
            WaitSeconds = FullWaitSeconds;
            if (GlobScr.Editing || GlobScr.BuildingMode)
            {
                GraceParent.SetActive(false);
                
            } else
            {
                GraceParent.SetActive(true);
            }
            }
            else
            {
                Txt.color = Red;
                Txt.text = "Grace Period over!";
                Grace = 0f;
                if (WaitSeconds > 0f)
                {
                    WaitSeconds -= Time.deltaTime;
                    
                if (GlobScr.Editing || GlobScr.BuildingMode)
                {
                    GraceParent.SetActive(false);

                }
                else
                {
                    GraceParent.SetActive(true);
                }
            }
                else
                {
                    GraceParent.SetActive(false);
                }
            }
        
    }
}

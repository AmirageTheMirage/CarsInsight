using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleFPS : MonoBehaviour
{
    public TMP_Text FPSText;
    public int FPS_Actual = 0;
    private int FPSCounter = 0;
    void Start()
    {
        FPSText.text = "0";
        InvokeRepeating("FPSCount", 1f, 1f);
    }

    
    void Update()
    {
        FPSCounter += 1;
    }

    void FPSCount()
    {
        FPS_Actual = FPSCounter;
        FPSCounter = 0;
        FPSText.text = FPS_Actual.ToString();
    }
}

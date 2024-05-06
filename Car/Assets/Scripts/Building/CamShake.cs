using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamShake : MonoBehaviour
{
    private float ShakeDur = 0f;
    private float ShakeMag = 0f;
    private float Frequency = 0f;
    public bool Trigger = false;

    public CinemachineVirtualCamera Cam1;
    private float ShakeTime = 0f;
    public bool Shaking = false;
    void Start()
    {
        Cam1 = GetComponent<CinemachineVirtualCamera>();
        Shaking = false;
    }
    void FixedUpdate()
    {
        if (Trigger)
        {
            Trigger = false;
            CamShakeGo(8f, 24f, 0.5f); //StandardValues
        }

        if (Shaking)
        {
            ShakeTime += Time.fixedDeltaTime;
            if (ShakeTime * 1.5f >= ShakeDur)
            {
                if (ShakeTime >= ShakeDur)
                {
                    Shaking = false;
                    ShakeTime = 0f;
                    Cam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
                    Cam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0f;
                }
                else
                {
                    Cam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = ShakeMag / 2f;
                    Cam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = Frequency / 2f;
                }
            }


        }  
    }

    public void CamShakeGo(float MyShakeMag, float MyShakeFreq, float MyTime)
    {
        ShakeTime = 0f;
        ShakeMag = MyShakeMag;
        Frequency = MyShakeFreq;
        ShakeDur = MyTime;

        Cam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = ShakeMag;
        Cam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = Frequency;
        Shaking = true;

        
        
    }

    
}

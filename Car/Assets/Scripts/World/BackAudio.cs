using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAudio : MonoBehaviour
{
    public AudioSource Chosen;
    public float TimeLeft = 0f;
    //public GameObject Cam1;
    public bool Skip = false;
    public DayNightCycle DayScr;

    [Space]
    public AudioSource DaySound;
    public AudioSource NightSound;

    public AudioSource[] DayAudios;
    public AudioSource[] NightAudios;
    private bool IsDay;
    
    void Start()
    {
        IsDay = DayScr.Day;
        PreLoadAudios();
        ChooseAudio();
    }

   
    void Update()
    {
        if (DayScr.Day != IsDay) {
            IsDay = DayScr.Day;
            Chosen.Stop();
            if (IsDay == true)
            {
                //It just turned Day
                DaySound.Play();
            } else
            {
                //It just turned Night
                NightSound.Play();
            }
            ChooseAudio();

        }
        
        if (TimeLeft > 0f)
        {
            TimeLeft -= Time.deltaTime;
        } else
        {
            ChooseAudio();
        }
        if (Skip)
        {
            Skip = false;
            ChooseAudio();

        }
    }

    void ChooseAudio()
    {

        if (Chosen != null)
        {
        Chosen.Stop();
        }
        if (IsDay == true)
        {
            int Rand = Mathf.FloorToInt(Random.Range(0f, DayAudios.Length - 0.1f));
            Chosen = DayAudios[Rand];
        } else
        {
            int Rand = Mathf.FloorToInt(Random.Range(0f, NightAudios.Length - 0.1f));
            Chosen = NightAudios[Rand];
        }
        TimeLeft = Chosen.clip.length;
        TimeLeft += 2f; //Wait between clips
        Chosen.Play();

    }

   


    void PreLoadAudios()
    {
        foreach (AudioSource Aud in DayAudios)
        {
            //float OriginVol = Aud.volume;
            Aud.Play();
            Aud.Stop();
        }

        foreach (AudioSource Aud in NightAudios)
        {
            Aud.Play();
            Aud.Stop();
        }
    }


}

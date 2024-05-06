using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreScript : MonoBehaviour
{
    public int Score = 0;
    public TMP_Text ScoreText;
    public GameObject MyPlus;
    public float FlySpeed = 10f;
    public int PlusTextValue = 0;
    public int ScoreChangeSpeed = 40;


    private TMP_Text PlusText;
    private Vector3 PlusTextTarget;
    private Vector3 PlusTextOrigin;
    private int CurScore;
    private float StayHereTextFullTime = 2f;
    private float StayHereText;
    private float WaitDelayFull = 1f;
    private float WaitDelay;
    void Start()
    {
        StayHereText = 0f;
        WaitDelay = 0f;
        ScoreText.text = "0";
        PlusText = MyPlus.GetComponent<TMP_Text>();
        PlusTextTarget = MyPlus.transform.position;
        PlusTextOrigin = new Vector3(PlusTextTarget.x - 120f, PlusTextTarget.y, PlusTextTarget.z);
        PlusText.text = "";
        MyPlus.transform.position = PlusTextOrigin;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlusText.text = "+ " + PlusTextValue.ToString();
        if (StayHereText >= 0f)
        {
            MyPlus.transform.position = Vector3.Lerp(MyPlus.transform.position, PlusTextTarget, FlySpeed * Time.deltaTime);
            
        }
        else
        {
            MyPlus.transform.position = Vector3.Lerp(MyPlus.transform.position, PlusTextOrigin, FlySpeed * Time.deltaTime);
        }
        if (WaitDelay >= 0)
        {
            WaitDelay -= Time.deltaTime;
        }
        else
        {
            if (CurScore != Score)
            {
                int CalcPlus = ScoreChangeSpeed;
                
                if (CurScore < Score)
                {
                    if (CalcPlus > Score - CurScore)
                    {
                        CalcPlus = Score - CurScore;
                    }
                    StayHereText = StayHereTextFullTime;
                    CurScore += CalcPlus;
                    PlusTextValue -= CalcPlus;
                }
                else
                {
                    CurScore = Score;
                    //CurScore -= CalcPlus;
                }
            }
            ScoreText.text = CurScore.ToString();
            
            if (StayHereText >= 0f)
            {
                StayHereText -= Time.deltaTime;
            }
        }
    }

    public void AddScore(int HowMuch)
    {
        if (HowMuch != 0)
        {
            Score += HowMuch;
            PlusTextValue += HowMuch;
            StayHereText = StayHereTextFullTime;
            WaitDelay = WaitDelayFull;
        }
    }
}

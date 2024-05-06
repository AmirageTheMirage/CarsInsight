using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanAfford : MonoBehaviour
{
    public GameObject MyObject;
    public int Price;
    public ScoreScript ScorScr;

    private void FixedUpdate()
    {
        if (ScorScr.Score >= Price)
        {
            MyObject.SetActive(false);
        } else
        {
            MyObject.SetActive(true);
        }
    }
}

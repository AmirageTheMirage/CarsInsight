using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ClearCamp : MonoBehaviour
{
    public GameObject ClearCampObj;
    public float MinSize = 0.1f;
    public float MaxSize = 1f;
    public float GrowTime = 10f;
    private bool IsShowingText = false;
    private TMP_Text ClearCampText;
    public float RotSpeed = 10f;
    private bool TurningLeft = true;
    public GracePeriod GrScr;
    private float MyTimer = 60f;
    void Start()
    {
        ClearCampText = ClearCampObj.GetComponent<TMP_Text>(); //For Future
        ClearCampObj.SetActive(false);
    }

    public void StartCall()
    {
        if (IsShowingText == false && GrScr.Grace <= 0f)
        {
            StartCoroutine(ShowText());
        }
    }
    public IEnumerator ShowText()
    {
        
            IsShowingText = true;
            ClearCampObj.SetActive(true);
            float CurrentSize = MinSize;
            float Timer = 0f;
            while (Timer < GrowTime)
            {
                CurrentSize = Mathf.Lerp(MinSize, MaxSize, Timer / GrowTime);
                ClearCampObj.transform.localScale = new Vector3(CurrentSize, CurrentSize, CurrentSize);
                Timer += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(6f);
            CurrentSize = MaxSize;
            Timer = 0f;
            while (Timer < GrowTime)
            {
                CurrentSize = Mathf.Lerp(MaxSize, MinSize, Timer / GrowTime);
                ClearCampObj.transform.localScale = new Vector3(CurrentSize, CurrentSize, CurrentSize);
                Timer += Time.deltaTime;
                yield return null;
            }
            
            ClearCampObj.SetActive(false);
            IsShowingText = false;

        

        yield return null;
    }
    void ResetBug()
    {
        ClearCampObj.SetActive(false);
        IsShowingText = false;
        MyTimer = 60f;
    }
    private void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartCoroutine(ShowText());
        //}
        //Turn Left and right
        if (MyTimer > 0f)
        {
            MyTimer -= Time.deltaTime;
        } else
        {
            ResetBug();
        }
        if (TurningLeft)
        {
            ClearCampObj.transform.Rotate(0f, 0f, Time.fixedDeltaTime * RotSpeed);
            if (ClearCampObj.transform.eulerAngles.z >= 10f && ClearCampObj.transform.eulerAngles.z <= 100f)
            {
                TurningLeft = false;
            }
        } else if (!TurningLeft)
        {
            ClearCampObj.transform.Rotate(0f, 0f, Time.fixedDeltaTime * RotSpeed * -1f);
            if (ClearCampObj.transform.eulerAngles.z <= 350f && ClearCampObj.transform.eulerAngles.z >= 100f)
            {
                TurningLeft = true;
            }
        }
        //Debug.Log(ClearCampObj.transform.eulerAngles.z);
    }
}

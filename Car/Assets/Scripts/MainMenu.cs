using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuObj;
    public GameObject HowToPlayObj;
    public AudioSource TickSound;
    public GameObject Panel;
    public Vector3 TargetTransform;
    [Space]
    private Vector3 OriginalTransform;
    private bool ShowPanel = true;
    private bool Direction = false; //False = Away
    
    private void Start()
    {
        MainMenuObj.SetActive(true);
        HowToPlayObj.SetActive(false);
        OriginalTransform = Panel.transform.localPosition;
        Panel.transform.localPosition = TargetTransform;
        //Panel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void Update()
    {
        if (ShowPanel)
        {
            if (Direction)
            {
                Panel.SetActive(true);
                Panel.transform.localPosition = Vector3.Lerp(Panel.transform.localPosition, TargetTransform, Time.deltaTime * 2f);
                if (Vector3.Distance(Panel.transform.localPosition, TargetTransform) < 0.6f)
                {

                    Panel.transform.localPosition = TargetTransform;
                    SceneManager.LoadScene(1);
                }
                
            }
            else
            {
                Panel.SetActive(true);
                Panel.transform.localPosition = Vector3.Lerp(Panel.transform.localPosition, OriginalTransform, Time.deltaTime * 2f);
                if (Vector3.Distance(Panel.transform.localPosition, OriginalTransform) < 0.6f)
                {

                    Panel.transform.localPosition = OriginalTransform;
                    Panel.SetActive(false);
                }
            }
        }
    }
    public void PlayGame()
    {
        if (!Panel.activeSelf)
        {
            TickSound.Play();
            ShowPanel = true;
            Direction = true;
        }
        
    }

    public void HowToPlay()
    {
        if (!Panel.activeSelf)
        {
            TickSound.Play();
            MainMenuObj.SetActive(false);
            HowToPlayObj.SetActive(true);
        }
    }

    public void BackToMain()
    {
        TickSound.Play();
        MainMenuObj.SetActive(true);
        HowToPlayObj.SetActive(false);
    }
}

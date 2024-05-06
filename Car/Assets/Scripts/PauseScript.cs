using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject Panel;
    public bool Paused = false;
    public bool ShowPanel = true;
    public bool Direction = false;
    public GameObject PauseMenu;
    public GameObject DeathMenu;
    [Space]
    private Vector3 OriginalVector;
    public Vector3 TargetVector;
    public bool WasMouseInCenter = false;
    public GameObject OtherCanvas;
    private AudioSource[] allAudioSources;
    public AudioSource ByPassed;

    void Start()
    {
        OriginalVector = Panel.transform.localPosition;
        Panel.transform.localPosition = TargetVector;
        PauseMenu.SetActive(false);
        OtherCanvas.SetActive(true);
        DeathMenu.SetActive(false);
        allAudioSources = FindObjectsOfType<AudioSource>();
        ByPassed.enabled = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
        OtherCanvas.SetActive(true);
        UnmuteAudioSources();
        ByPassed.enabled = true;
        ByPassed.mute = false;
        
            ByPassed.Stop();
            ByPassed.Play();
        
        if (WasMouseInCenter)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        ShowPanel = true;
        Direction = true;
        // Code for quitting game
    }

    public void Death()
    {
        PauseMenu.SetActive(false);
        DeathMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        OtherCanvas.SetActive(false);
        ByPassed.enabled = false;
        MuteAudioSources();
    }

    public void PauseGame()
    {
        Paused = true;
        OtherCanvas.SetActive(false);
        PauseMenu.SetActive(true);
        ByPassed.enabled = false;
        MuteAudioSources();
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            WasMouseInCenter = true;
        }
        else
        {
            WasMouseInCenter = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    private void MuteAudioSources()
    {
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != null)
            {
                audioSource.mute = true;

            }
        }
    }

    private void UnmuteAudioSources()
    {
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource != null)
            {
            audioSource.mute = false;

            }
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !PauseMenu.activeSelf && !DeathMenu.activeSelf)
        {
            if (!Panel.activeSelf)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

        if (ShowPanel)
        {
            if (Direction)
            {
                Panel.SetActive(true);
                Panel.transform.localPosition = Vector3.Lerp(Panel.transform.localPosition, TargetVector, Time.deltaTime * 2f);
                if (Vector3.Distance(Panel.transform.localPosition, TargetVector) < 0.2f)
                {
                    Panel.transform.localPosition = TargetVector;
                    SceneManager.LoadScene(0);
                }
            }
            else
            {
                Panel.SetActive(true);
                Panel.transform.localPosition = Vector3.Lerp(Panel.transform.localPosition, OriginalVector, Time.deltaTime * 2f);
                if (Vector3.Distance(Panel.transform.localPosition, OriginalVector) < 0.2f)
                {
                    Panel.transform.localPosition = OriginalVector;
                    Panel.SetActive(false);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public AudioSource musicBg;
    public GameObject pauseMenuScreen;
    public void OpenPauseMenu()
    {
        if (TutoTrigger.instance.isFinished)
        {
            pauseMenuScreen.SetActive(true);
            musicBg.Pause();
            Time.timeScale = 0;
        }
    }

    public void ClosePauseMenu()
    {
        pauseMenuScreen.SetActive(false);
        musicBg.Play();
        Time.timeScale = 1;
    }
}

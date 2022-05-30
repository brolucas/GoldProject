using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuScreen;
    public void OpenPauseMenu()
    {
        pauseMenuScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void ClosePauseMenu()
    {
        pauseMenuScreen.SetActive(false);
        Time.timeScale = 1;
    }
}

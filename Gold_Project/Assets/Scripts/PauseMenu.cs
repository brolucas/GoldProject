using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuScreen;
    public void OpenPauseMenu()
    {
        pauseMenuScreen.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        pauseMenuScreen.SetActive(false);
    }
}

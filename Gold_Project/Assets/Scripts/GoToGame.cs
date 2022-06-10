using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToGame : MonoBehaviour
{
    public static int levelIndex = 1;
    public static bool tutoDone = false;

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void BackToMainMenuVictory()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        levelIndex++;
        DontDestroyOnLoad(this);
    }

    

}

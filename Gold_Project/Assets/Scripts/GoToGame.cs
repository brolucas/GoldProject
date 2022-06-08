using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToGame : MonoBehaviour
{
    public int levelIndex = 1;
    
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

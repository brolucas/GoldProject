using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToGame : MonoBehaviour
{
    public GameObject HUDLeft;
    public GameObject HUDRight;
    public SwitchHUDSide switchHUDSide;
    public Text levelName;

    private Scene gameScene;

    public void LaunchGame()
    {
        SceneManager.LoadScene(levelName.text);
        Time.timeScale = 1;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 0;
    }
}

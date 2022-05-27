using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToGame : MonoBehaviour
{
    public GameObject HUDLeft;
    public GameObject HUDRight;
    public GameObject mainMenuScreen;
    public SwitchHUDSide switchHUDSide;
    public GameObject gameOverScreen;

    private Scene gameScene;

    public void LaunchGame()
    {
        SceneManager.LoadScene("Téo");
        Time.timeScale = 1;
        /*if (switchHUDSide.isLeft)
        {
            HUDLeft.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("Téo");
            HUDRight.SetActive(true);
        }

        mainMenuScreen.SetActive(false);*/
    }

    public void BackToMainMenu()
    {
        //GameScreen.SetActive(false);

        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 0;
    }
}

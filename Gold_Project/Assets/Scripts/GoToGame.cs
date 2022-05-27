using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoToGame : MonoBehaviour
{
    public GameObject GameScreen;
    public GameObject MainMenuScreen;

    public void LaunchGame()
    {
        GameScreen.SetActive(true);
        MainMenuScreen.SetActive(false);
    }

    public void BackToMainMenu()
    {
        GameScreen.SetActive(false);
        MainMenuScreen.SetActive(true);
    }
}

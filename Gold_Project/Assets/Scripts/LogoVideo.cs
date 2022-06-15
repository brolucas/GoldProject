using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoVideo : MonoBehaviour
{
    public SceneFader sceneFader;

    public void StartGame()
    {
        sceneFader.FadeTo("Title");
    }
}

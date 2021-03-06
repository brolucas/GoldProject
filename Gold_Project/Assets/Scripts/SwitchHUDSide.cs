using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchHUDSide : MonoBehaviour
{
    public GameObject leftHUD;
    public GameObject rightHUD;
    public bool isLeft = true;

    public void GoLeft()
    {
        leftHUD.SetActive(true);
        rightHUD.SetActive(false);
        isLeft = true;
    }

    public void GoRight()
    {
        leftHUD.SetActive(false);
        rightHUD.SetActive(true);
        isLeft = false;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public static TimerController instance;

    public Text timerCounterRight;
    public Text timerCounterLeft;

    private TimeSpan timePlaying;
    private bool timerGoing;
    private float elapsedTime;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        timerCounterRight.text = "00 : 00";
        timerCounterLeft.text = "00 : 00";
        timerGoing = false;

        BeginTimer();
    }

    public void BeginTimer()
    {
        timerGoing = true;
        elapsedTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    public void EndTimer()
    {
        timerGoing = false;
    }

    private IEnumerator UpdateTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timePlayingStr = timePlaying.ToString("mm' : 'ss");
            timerCounterRight.text = timePlayingStr;
            timerCounterLeft.text = timePlayingStr;

            yield return null;
        }
    }
}

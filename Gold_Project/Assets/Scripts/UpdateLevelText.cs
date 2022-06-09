using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateLevelText : MonoBehaviour
{
    private Text levelText;

    private void Awake()
    {
        levelText = GetComponent<Text>();
    }

    void Start()
    {
        levelText.text = ("Level " + GoToGame.levelIndex);
    }
    
    void Update()
    {
        levelText.text = ("Level " + GoToGame.levelIndex);
    }
}

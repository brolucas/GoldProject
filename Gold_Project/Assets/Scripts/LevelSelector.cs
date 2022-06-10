using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelSelector : MonoBehaviour
{
    public GameObject panel;
    public Text levelText;
    public Button[] listeLevel;
    

    // Start is called before the first frame update
    void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);
        for (int i = 0; i < listeLevel.Length; i++)
        {
            if (i + 1 > levelReached)
            {
                listeLevel[i].interactable = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClosePanel()
    {
        panel.SetActive(false);
    }
    public void OpenPanel()
    {
        panel.SetActive(true);
    }
    public void UpdateLevelText()
    {
        
    }
    public void Level1()
    {
        levelText.text = "Level 1";
    }
    public void Level2()
    {
        levelText.text = "Level 2";
    }
    public void Level3()
    {
        levelText.text = "Level 3";
    }
    public void Level4()
    {
        levelText.text = "Level 4";
    }
    public void Level5()
    {
        levelText.text = "Level 5";
    }
    public void Level6()
    {
        levelText.text = "Level 6";
    }
    public void Level7()
    {
        levelText.text = "Level 7";
    }
    public void Level8()
    {
        levelText.text = "Level 8";
    }
    public void Level9()
    {
        levelText.text = "Level 9";
    }
}

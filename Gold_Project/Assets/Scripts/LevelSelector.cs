using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelSelector : MonoBehaviour
{
    public GameObject panel;
    public GameObject LevelText;

    // Start is called before the first frame update
    void Start()
    {
        
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
}

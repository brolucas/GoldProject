using UnityEngine;

public class TutoTrigger : MonoBehaviour
{
    public Tuto tuto;
    private bool activeIt = false;
    public bool isFinished = false;

    public GameObject tutoPanel;

    public static TutoTrigger instance;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
        
    }

    public void TriggerTuto()
    {
        tutoPanel.SetActive(true);
        Time.timeScale = 0;
        TutoManager.instance.StartTuto(tuto);
    }
}

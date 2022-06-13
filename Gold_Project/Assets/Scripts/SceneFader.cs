using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneFader : MonoBehaviour
{
    public Image img;
    public Text levelName;
    public AnimationCurve curve;
    public GameObject gameManager;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 1f;

        while (t > 0)
        {
            t -= Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
        if (SceneManager.GetActiveScene().name == "Logo")
        {
            string scene = "Title";
            StartCoroutine(FadeOut(scene));
        }

        if (SceneManager.GetActiveScene().name == "Level 1" && PlayerPrefs.GetInt("tuto",0)==0)
        {
            TutoTrigger.instance.TriggerTuto();
        }

        
        gameManager.SetActive(true);
    }

    public void FadeTo(string scene)
    {
        Time.timeScale = 1;
        if (SceneManager.GetActiveScene().name == "Title" && PlayerPrefs.GetInt("firstTime", 0) == 0)
        {
            StartCoroutine(FadeOut("Level 1"));
        }
        else
        {

            StartCoroutine(FadeOut(scene));
        }
    } 

    public void FadeToGame(Text levelName)
    {
        if (DataManager.Instance.deckData.deckTurret.Contains(KindOfTurret.DefaultDoNotUseIt))
        {
            Debug.LogWarning("You need to have 4 turrets in the Deck !!");
            return;

        }

        StartCoroutine(FadeOut(levelName.text));
    }

    IEnumerator FadeOut(string scene)
    {
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }

        SceneManager.LoadScene(scene);
    }
}

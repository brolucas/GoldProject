using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneFader : MonoBehaviour
{
    public Image img;
    public Text levelName;
    public AnimationCurve curve;

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

        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            TutoTrigger.instance.TriggerTuto();
        }
    }

    public void FadeTo(string scene)
    {
        Time.timeScale = 1;
        StartCoroutine(FadeOut(scene));
    } 

    public void FadeToGame(Text levelName)
    {
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

        /*if (SceneManager.GetActiveScene().name != "MainMenu" || SceneManager.GetActiveScene().name != "Title" || SceneManager.GetActiveScene().name != "Logo")
        {
            GameManager.Instance.enemies.Clear();
        }*/
        SceneManager.LoadScene(scene);

    }
    
}

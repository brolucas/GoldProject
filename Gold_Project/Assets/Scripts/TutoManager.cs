using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoManager : MonoBehaviour
{
    public Text tutoText;
    public Image tutoImg;

    private Queue<string> sentences;
    private Queue<Sprite> sprites;

    public static TutoManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;

        sentences = new Queue<string>();
        sprites = new Queue<Sprite>();
    }

    public void StartTuto(Tuto tuto)
    {
        sentences.Clear();
        sprites.Clear();

        foreach (string sentence in tuto.sentences)
        {
           sentences.Enqueue(sentence);
        }

        DisplayNextSentence();

        foreach (Sprite sprite in tuto.tutoSprites)
        {
            sprites.Enqueue(sprite);
        }

        DisplayNextSprites();
    }

    public void DisplayNextSentence()
    {
        Debug.Log("Sentences.Count = " + sentences.Count);
        if (sentences.Count == 0)
        {
            TutoTrigger.instance.isFinished = true;
            EndTuto();
            return;
        }

        string sentence = sentences.Dequeue();
        tutoText.text = sentence;
    }

    public void DisplayNextSprites()
    {
        if (sprites.Count == 0)
        {
            TutoTrigger.instance.isFinished = true;
            EndTuto();
            return;
        }

        Sprite sprite = sprites.Dequeue();
        tutoImg.sprite = sprite;

    }

    void EndTuto()
    {
        Time.timeScale = 1;
        AchivementsFinishing.instance.Achievement(true, GPGSIds.achievement_finishing_tutorial);
        TutoTrigger.instance.tutoPanel.SetActive(false);
    }
}

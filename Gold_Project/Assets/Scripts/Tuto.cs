using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Tuto
{
    public Sprite[] tutoSprites;

    [TextArea(5, 10)]
    public string[] sentences;
}

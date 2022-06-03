using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Tuto
{
    public Image tutoImage;

    [TextArea(5, 10)]
    public string[] sentences;
}

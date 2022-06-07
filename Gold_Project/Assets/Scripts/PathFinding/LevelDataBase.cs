using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class LevelDataBase : ScriptableObject
{
    public List<LevelData> levels = new List<LevelData>();
}

[System.Serializable]
public class LevelData
{
    [Header("Name")]
    public string label;

    [Header("Level Info")]
    public List<Decor> decors = new List<Decor>();
    public GameObject DecorPrefab;
}

[System.Serializable]
public class Decor
{
    public Sprite image;
    public int x;
    public int y;
}


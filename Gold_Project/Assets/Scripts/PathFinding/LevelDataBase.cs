using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelLabel
{
    Level1,
    Level2,
    Level3,
    Level4,
    Level5,
    Level6,
    Level7,
    Level8,
    Level9,
    Level10,
    Level11,
    Level12,
    Level13,
    Level14
};

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
    public LevelLabel levelLabel;

    [Header("Level Info")]
    public List<Decor> decors = new List<Decor>();
}

[System.Serializable]
public class Decor
{
    public Sprite image;
    public int x;
    public int y;
}


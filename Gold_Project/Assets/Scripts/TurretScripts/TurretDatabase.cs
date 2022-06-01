using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Default,
    Terreste,
    Aerial,
    Both
}

public enum KindOfTurret
{
    Default,
    SniperTower,
    Furnace
};

[CreateAssetMenu(fileName = "New Turret", menuName ="Turret")]
public class TurretDatabase : ScriptableObject
{

    public List<TurretData> turrets = new List<TurretData>();

}

[System.Serializable]
public class TurretData
{
    [Header("Name")]
    public string label;
    public KindOfTurret kindOfTurret;

    [Header("Turret Stats")]
    public float healthPoints;
    [Range(0, 10)]
    public float range;
    public int nbrOfTarget;
    public int turretPrice;
    public TargetType targetType;

    [Header("Attack Stats")]
    public int atqPoints;
    public float fireRate;
    [Tooltip("Limit the maximum damage with the passive included so 200 = 50 + 1000 = 50 + 150")]
    public int maxAtqPoints;
    [Tooltip("Check if yes")]
    public bool activeAtqCap;


    [Header("Design")]
    public Sprite inGameDesign;
    public Sprite UIDesign;
}

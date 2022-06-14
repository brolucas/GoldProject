using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TargetType
{
    DefaultDoNotUseIt,
    Terreste,
    Aerial,
    Both
}

public enum KindOfTurret
{
    // -- /!\ Do not Add a new Type above another add it below the last one PLS /!\ -- //
    // -- /!\ If you need to delete one replace every one in the ScriptableObject /!\ -- //
    DefaultDoNotUseIt,
    Basic,
    Mortar,
    Discord,
    SniperTower,
    Furnace,
    Channelizer,
    Generator,
    Immobilizer,
    Zap,
    Teleporter,
    // ex : NewType,
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

    [Header("Passive Stats")]
    public float basePassiveParameters;
    public float maxPassiveParameters;
    public float capPassive;

    [Header("Design")]
    public Sprite inGameDesign;
    public Sprite UIDesign;

    [TextArea(5, 20)]
    public string description;
}

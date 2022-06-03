using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turret", menuName ="Turret")]
public class TurretScriptable : ScriptableObject
{
    [Header("Turret Stats")]
    [Range(1, 3)]
    [Tooltip("1 = Terreste, 2 = Aerial, 3 = Both")]
    public int turretType;
    public float healthPoints;
    [Range(0, 10)]
    public float range;
    public int nbrOfTarget;
    public int turretPrice;

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

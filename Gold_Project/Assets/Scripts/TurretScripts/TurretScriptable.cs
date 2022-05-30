using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turret", menuName ="Turret")]
public class TurretScriptable : ScriptableObject
{
    [Header("Turret Stats")]
    public float healthPoints;
    public int atqPoints;
    public float fireRate;
    [Range(0, 10)]
    public float range;
    public int nbrOfTarget;
    [Tooltip("Limit the maximum damage with the passive included so 200 = 50 + 1000 = 50 + 150")]
    public int maxAtqPoints;
    public int turretPrice;

    public Sprite inGameDesign;
    public Sprite UIDesign;
}

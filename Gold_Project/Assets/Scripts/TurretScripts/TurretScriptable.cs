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
    public int range;
    public int nbrOfTarget;
    public int maxAtqPoints;

    public Sprite inGameDesign;
    public Sprite UIDesign;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SniperTower : Turret
{
    public float damageBonusBaseOnHP = 10;
    public int pushHowFar = 2;

    private float thickness = 1.0f;

    private void Awake()
    {
        damageBonusBaseOnHP = damageBonusBaseOnHP / 100;
    }

    public override void ChooseTarget(Vector3 origin)
    {
        EnemiesTemp currentTarget = targets[0];

        //Search for the target with the highest maxHealth
        foreach (var target in targets)
        {
            if (target.startingHealth > currentTarget.startingHealth)
            {
                currentTarget = target;
            }
        }

        #region RayToTarget
        Vector3 rayToTarget = currentTarget.transform.position - origin;
        Gizmos.color = Color.red;
        #if UNITY_EDITOR
        Handles.DrawLine(origin, origin + rayToTarget, thickness);
        #endif
        #endregion

        if (fireCountDown <= 0f)
        {
            Shoot(currentTarget.GetComponent<EnemiesTemp>());
            fireCountDown = 1 / fireRate;
        }

        fireCountDown -= Time.deltaTime / 2;
    }

    public override void TurretPassive(EnemiesTemp enemy)
    {
        // inflicts % of the target's max hp per attack

        //atqPtsBonus = Mathf.Clamp(enemy.startingHealth * damageBonusBaseOnHP, 0, maxAtqPoints - atqPoints);

        atqPtsBonus = enemy.startingHealth * damageBonusBaseOnHP; // no cap so may be 9999
    }

    public override void PassiveLevelmax(EnemiesTemp enemy)
    {
        // To do 
    }
}

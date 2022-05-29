using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SniperTower : Turret
{
    public float damageBonusBaseOnHP = 15;

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
        //Handles.DrawLine(origin, origin + rayToTarget, thickness);
        #endregion

        if (fireCountDown <= 0f)
        {
            Shoot(currentTarget.GetComponent<EnemiesTemp>());
            fireCountDown = 1 / fireRate;
        }

        fireCountDown -= Time.deltaTime / 2;
    }

    //empty
    public override void TurretPassive(EnemiesTemp enemy)
    {

    }

    public override void PassiveLevelmax(EnemiesTemp enemy)
    {
        // inflicts 15% of the target's max hp per attack

        thickness = 3.0f;

        atqPtsBonus = Mathf.Clamp(enemy.startingHealth * damageBonusBaseOnHP, 0, maxAtqPoints - atqPoints);
    }
}

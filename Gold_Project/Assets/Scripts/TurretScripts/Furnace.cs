using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Furnace : Turret
{
    public float explosionRange = 1;

    public override void ChooseTarget(Vector3 origin)
    {
        EnemiesTemp currentTarget = targets[0];

        //Search for the target with the highest maxHealth
        foreach (var target in targets)
        {
            if (target.currentHealth > currentTarget.currentHealth)
            {
                currentTarget = target;
            }
        }

        #region RayToTarget
        Vector3 rayToTarget = currentTarget.transform.position - origin;
        Debug.DrawLine(origin, origin + rayToTarget, Color.red);
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
        // inflicts damage to enemies near the target

        /*#if UNITY_EDITOR
        Handles.color = Color.red;
        Handles.DrawWireDisc(enemy.transform.position              // position
                             , transform.forward // normal or new Vector3(0,0,1) same thing
                             , 0.1f
                             , explosionRange * 150.0f);  // range
        
        Handles.color = Color.blue;
        Handles.DrawWireDisc(enemy.transform.position              // position
                             , transform.forward // normal or new Vector3(0,0,1) same thing
                             , explosionRange);  // range

        #endif*/

        foreach (var enemies in GameManager.Instance.enemies)
        {
            if (enemies == enemy)
                return;

            Vector2 objPos = enemies.transform.position;

            float distance = Vector2.Distance(enemy.transform.position, objPos);

            bool isInside = distance < explosionRange;

            if (isInside)
            {
                enemies.TakeDamage(this.atqPoints);

                if (currentLevel == 10)
                {
                    PassiveLevelmax(enemies);
                }
            }
        }
    }

    public override void PassiveLevelmax(EnemiesTemp enemy)
    {
        // applies the burn to the affected targets inflicting 1 % of the target's max hp per second

        if (enemy.isBurning == false)
            StartCoroutine(enemy.Burn());
    }
}

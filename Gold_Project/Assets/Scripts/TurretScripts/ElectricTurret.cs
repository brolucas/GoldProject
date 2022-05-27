using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricTurret : Turret
{
    // ----- To Do ----- //

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

        Vector3 rayToTarget = currentTarget.transform.position - origin;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + rayToTarget);

        if (fireCountDown <= 0f)
        {
            Shoot(currentTarget.GetComponent<EnemiesTemp>());
            fireCountDown = 1 / fireRate;
        }

        fireCountDown -= Time.deltaTime / 2;
    }

    public override void PassiveLevelmax(EnemiesTemp enemy)
    {
        // attacks briefly immobilize the target (0.1s)
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RadialTrigger : MonoBehaviour
{
    

    [Header("Turret Stats")]
    [Range(0f, 4f)]
    public float radius = 1f;
    public int atqPoints = 100;
    public float fireRate = 1.0f;

    private bool isInside;

    public float nextActionTime = 0.0f;

    public List<EnemiesTemp> targets = new List<EnemiesTemp>();

    

/*    public void Start()
    {
        startTime = Time.time;
    }*/

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;

        if (GameManager.Instance.enemies.Count <= 0)
            return;

        foreach (var enemy in GameManager.Instance.enemies)
        {
            Vector2 objPos = enemy.transform.position;
            
            float distance = Vector2.Distance(objPos, origin);

            isInside = distance < radius;

            Handles.color = isInside ? Color.red : Color.grey;
            Handles.DrawWireDisc(origin            // position
                                 , transform.forward // normal or new Vector3(0,0,1) same thing
                                 , radius);          // radius

            if(isInside)
            {
                if (!targets.Contains(enemy))
                {
                    targets.Add(enemy);

                    enemy.startTime = Time.time;
                }
            }
            else
            {
                targets.Remove(enemy);
            }
            
        }

        Vector3 firstTarget = targets[0].transform.position - origin;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + firstTarget);

        EnemiesTemp enemyScript = targets[0].GetComponent<EnemiesTemp>();

        

        if (Time.time - enemyScript.startTime > nextActionTime)
        {
            nextActionTime += fireRate;

            Attack(enemyScript);
        }
    }

    public void Attack(EnemiesTemp enemy)
    {
        enemy.TakeDamage(atqPoints);

        if (!enemy.attackingTurret.Contains(this))
        {
            enemy.attackingTurret.Add(this);
        }

       enemy.durationToDie = nextActionTime + fireRate;
    }
}

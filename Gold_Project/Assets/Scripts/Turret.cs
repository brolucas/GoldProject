using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Turret : MonoBehaviour
{

    [Header("Turret Stats")]
    public float healthPoints = 0;
    public int atqPoints = 100;
    public float fireRate = 1.0f;
    [Range(0f, 4f)]
    public float range = 1f;
    public int nbrOfTarget = 1;

    private bool isInside;

    public float fireCountDown = 0.0f;

    public bool canBeMoved = false;

    public List<EnemiesTemp> targets = new List<EnemiesTemp>();

    public void Start()
    {
        Debug.Log(Time.time);
    }

    private void OnMouseDrag()
    {
        if (!canBeMoved)
            return;

        this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.transform.position = new Vector3(
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 
            0);
    }

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;

        if (GameManager.Instance.enemies.Count <= 0)
            return;

        foreach (var enemy in GameManager.Instance.enemies)
        {
            Vector2 objPos = enemy.transform.position;
            
            float distance = Vector2.Distance(objPos, origin);

            isInside = distance < range;

            if(isInside)
            {
                if (!targets.Contains(enemy))
                {
                    targets.Add(enemy);

                    //enemy.startTime = Time.time;
                }
            }
            else
            {
                targets.Remove(enemy);
            }
        }

        Handles.color = targets.Count > 0 ? Color.red : Color.grey;
        Handles.DrawWireDisc(origin            // position
                             , transform.forward // normal or new Vector3(0,0,1) same thing
                             , range);          // range

        if (targets.Count <= 0)
            return;

        Vector3 firstTarget = targets[0].transform.position - origin;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + firstTarget);

        EnemiesTemp enemyScript = targets[0].GetComponent<EnemiesTemp>();

        if (fireCountDown <= 0f)
        {
            Shoot(enemyScript);
            fireCountDown = 1 / fireRate;
        }

        fireCountDown -= Time.deltaTime / 2;
    }

    public void Shoot(EnemiesTemp enemy)
    {
        enemy.TakeDamage(atqPoints);

        //Add this turret to the attacking turret
        if (!enemy.attackingTurret.Contains(this))
        {
            enemy.attackingTurret.Add(this);
        }

        enemy.durationToDie += fireRate;
    }
}

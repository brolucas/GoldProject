using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class Turret : MonoBehaviour
{
    [SerializeField]
    private TurretScriptable turret;

    #region Turret Stats
    public float healthPoints {get; private set;}
    public int atqPoints { get; private set; }
    public float fireRate { get; private set; }
    public float range { get; private set; }
    public int nbrOfTarget { get; private set; }
    [Range(0, 10)]
    public int currentLevel;
    public float atqPtsBonus { get; set; }
    public float maxAtqPoints { get; set; }
    #endregion

    private bool isInside;

    public float fireCountDown { get; protected set; } = 0.0f;

    public bool canBeMoved = false;

    public List<EnemiesTemp> targets = new List<EnemiesTemp>();

    public void Start()
    {
        if (turret == null)
        {
            Debug.LogError("This turret doens't have a TurretScriptable attach to it : " + this);
            return;
        }

        healthPoints = turret.healthPoints;

        atqPoints = turret.atqPoints;
        atqPtsBonus = 0;
        maxAtqPoints = turret.maxAtqPoints;

        fireRate = turret.fireRate;
        range = turret.range;
        nbrOfTarget = turret.nbrOfTarget;
        currentLevel = 0;
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
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.enemies.Count <= 0)
            return;

        foreach (var enemy in GameManager.Instance.enemies)
        {
            if (enemy == null)
                return;

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
                enemy.attackingTurret.Remove(this);
                targets.Remove(enemy);
            }
        }

        #if UNITY_EDITOR
        Handles.color = targets.Count > 0 ? Color.red : Color.grey;
        Handles.DrawWireDisc(origin            // position
                             , transform.forward // normal or new Vector3(0,0,1) same thing
                             , range);          // range

        #endif
        Gizmos.DrawWireSphere(origin            // position
            , range);          // range
        if (targets.Count <= 0)
            return;
        ChooseTarget(origin);

        /*switch (currentLevel)
        {
            case 10:
                Debug.Log("The Turret is level max");
                break;
        }*/
    }

    public virtual void ChooseTarget(Vector3 origin)
    {
        //Attack the first target to enter the range until it die or goes out of range

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

    public virtual void Shoot(EnemiesTemp enemy)
    {
        TurretPassive(enemy);

        if (currentLevel == 10)
            PassiveLevelmax(enemy);

        float damage = Mathf.Clamp(atqPoints + atqPtsBonus, 0, maxAtqPoints);

        enemy.TakeDamage(damage);

        //Debug.Log(damage + " damage dealt");

        //Add this turret to the attacking turret of the ennemy
        if (!enemy.attackingTurret.Contains(this))
        {
            enemy.attackingTurret.Add(this);
        }
    }

    public virtual void TurretPassive(EnemiesTemp enemy)
    {
        Debug.LogError("Shouln't be using this virtual method (TurretPassive)");
    }

    public virtual void PassiveLevelmax(EnemiesTemp enemy)
    {
        Debug.LogError("Shouln't be using this virtual method (PassiveLevelmax)");
    }
}

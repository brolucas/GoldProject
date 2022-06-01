using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Turret : MonoBehaviour
{
    [SerializeField]
    private TurretScriptable turret;

    private GameManager gameManager;

    private GameObject rangeSprite;
    private Vector3 localScale;

    #region Turret Stats
    public float healthPoints {get; private set;}
    public int atqPoints { get; private set; }
    public float fireRate { get; private set; }
    public float range;
    public int nbrOfTarget { get; private set; }
    [Range(0, 10)]
    public int currentLevel;
    public float atqPtsBonus { get; set; }
    public float maxAtqPoints { get; set; }
    protected bool isAtqCap;

    public enum TurretType
    {
        terreste,
        aerial,
        both
    }

    public TurretType turretType { get; protected set; }

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

        gameManager = GameManager.Instance;

        gameManager.allTurret.Add(this);

        #region CheckStats
        if (turret.healthPoints <= 0)
            Debug.LogError("This is turret.healthPoints : 0" + this);
        if (turret.atqPoints <= 0)
            Debug.LogError("This is turret.atqPoints : 0" + this);
        if (turret.fireRate <= 0)
            Debug.LogError("This is turret.fireRate : 0" + this);
        if (turret.range <= 0)
            Debug.LogError("This is turret.range : 0" + this);
        if (turret.turretPrice <= 0)
            Debug.LogError("This is turret.turretPrice : 0" + this);
        #endregion

        healthPoints = turret.healthPoints;

        atqPoints = turret.atqPoints;
        atqPtsBonus = 0;
        maxAtqPoints = turret.maxAtqPoints;
        isAtqCap = turret.activeAtqCap;

        fireRate = turret.fireRate;
        range = turret.range;
        nbrOfTarget = turret.nbrOfTarget;
        currentLevel = 0;

        switch (turret.turretType)
        {
            case 1:
                turretType = TurretType.terreste;
                break;
            case 2:
                turretType = TurretType.aerial;
                break;
            case 3:
                turretType = TurretType.both;
                break;
            default:
                break;
        }

        //Show the range of the turret
        rangeSprite = Instantiate(gameManager.rangeSprite, this.transform.position, this.transform.rotation, this.transform);

        localScale = rangeSprite.transform.localScale;
        rangeSprite.transform.localScale = new Vector3(localScale.x * range, localScale.y * range, 0);
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

    private void Update()
    {
        Vector3 origin = transform.position;
        if (gameManager == null)
            return;

        if (gameManager.enemies.Count <= 0)
            return;

        // Change the visibility range // need opti
        rangeSprite.transform.localScale = new Vector3(localScale.x * range, localScale.y * range, 0);

        foreach (var enemy in gameManager.enemies)
        {
            if (enemy == null)
            {
                Debug.LogError("There is a null error");
                return;
            }

            Vector2 objPos = enemy.transform.position;

            float distance = Vector2.Distance(objPos, origin);

            isInside = distance < range;

            if (isInside)
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

        /*#if UNITY_EDITOR
        Gizmos.color = targets.Count > 0 ? Color.red : Color.grey;
        Gizmos.DrawWireSphere(origin            // position
                             , range);          // range

        #endif

        Gizmos.DrawWireSphere(origin            // position
            , range);          // range*/

        if (targets.Count <= 0)
            return;
        ChooseTarget(origin);
    }


    public virtual void ChooseTarget(Vector3 origin)
    {
        //Attack the first target to enter the range until it die or goes out of range

        Vector3 firstTarget = targets[0].transform.position - origin;

        Debug.DrawLine(origin, origin + firstTarget, Color.red);

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

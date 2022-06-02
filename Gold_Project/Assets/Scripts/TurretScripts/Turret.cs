using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Turret : MonoBehaviour
{
    [SerializeField]
    private TurretDatabase turretDatabase;

    private GameManager gameManager;

    private GameObject rangeSprite;
    private Vector3 localScale;

    #region Turret Stats

    public string label { get; private set; }
    public KindOfTurret kindOfTurret { get; private set; }

    //Turret Stats
    public float healthPoints { get; private set; }
    [Range(0, 10)]
    public float range;
    public int turretPrice { get; private set; }
    public int nbrOfTarget { get; private set; }
    public int turretPrice { get; private set; }
    public TargetType targetType { get; private set; }
    [Range(0, 10)]
    public int currentLevel;

    //Attack Stats
    public int atqPoints { get; private set; }
    public float atqPtsBonus { get; set; }
    public float fireRate { get; private set; }
    public int maxAtqPoints { get; private set; }
    protected bool isAtqCap;

    //Design
    public Sprite inGameDesign { get; private set; }
    public Sprite UIDesign { get; private set; }
    #endregion

    private bool isInside;

    public float fireCountDown { get; protected set; } = 0.0f;

    public bool canBeMoved = false;

    public List<EnemiesTemp> targets = new List<EnemiesTemp>();

    public void Start()
    {

        if (turretDatabase == null)
        {
            Debug.LogError("This turret doens't have a TurretScriptable attach to it : " + this);
            return;
        }

        gameManager = GameManager.Instance;

        gameManager.allTurret.Add(this);

        #region Show the range of the turret
        rangeSprite = Instantiate(gameManager.rangeSprite, this.transform.position, this.transform.rotation, this.transform);

        localScale = rangeSprite.transform.localScale;
        rangeSprite.transform.localScale = new Vector3(localScale.x * range, localScale.y * range, 0);
        #endregion
    }

    public void InitTurretData(KindOfTurret type)
    {
        TurretData turretData = turretDatabase.turrets.Find(data => data.kindOfTurret == type);

        if (turretData == null)
        {
            Debug.LogError("There is TurretData of the type : " + type);
            return;
        }

        #region CheckStats

        if (turretData.healthPoints <= 0)
            Debug.LogError("Shouldn't this turret.healthPoints be : 0" + this);
        if (turretData.range <= 0)
            Debug.LogError("Shouldn't this turret.range be : 0" + this);
        if (turretData.nbrOfTarget <= 0)
            Debug.LogError("Shouldn't this turret.nbrOfTarget be : 0" + this);
        if (turretData.turretPrice <= 0)
            Debug.LogError("Shouldn't this turret.turretPrice be : 0" + this);
        if (turretData.targetType == TargetType.Default)
            Debug.LogError("Shouldn't this turret.targetType be : DEFAULT " + this);
        
        if (turretData.atqPoints <= 0)
            Debug.LogError("Shouldn't this turret.atqPoints be : 0" + this);
        if (turretData.fireRate <= 0)
            Debug.LogError("Shouldn't this turret.fireRate be : 0" + this);

        /*if (turretData.inGameDesign == null)
            Debug.LogError(this + "Doesn't have a sprite");
        if (turretData.UIDesign == null)
            Debug.LogError(this + "Doesn't have a sprite");*/
        #endregion

        #region Initialize Stats

        label = turretData.label;
        kindOfTurret = turretData.kindOfTurret;

        healthPoints = turretData.healthPoints;
        range = turretData.range;
        nbrOfTarget = turretData.nbrOfTarget;
        turretPrice = turretData.turretPrice;

        atqPoints = turretData.atqPoints;
        atqPtsBonus = 0;
        fireRate = turretData.fireRate;
        maxAtqPoints = turretData.maxAtqPoints;
        isAtqCap = turretData.activeAtqCap;

        inGameDesign = turretData.inGameDesign;
        UIDesign = turretData.UIDesign;

        currentLevel = 0;
        #endregion
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
        Handles.DrawWireSphere(origin            // position
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
        switch (kindOfTurret)
        {
            case KindOfTurret.SniperTower:
                {
                    // inflicts % of the target's max hp per attack

                    int damageBonusBaseOnHP = 10;

                    if (!isAtqCap)
                    {
                        atqPtsBonus = enemy.startingHealth * damageBonusBaseOnHP; // no cap so may be 9999
                    }
                    else
                    {
                        atqPtsBonus = Mathf.Clamp(enemy.startingHealth * damageBonusBaseOnHP, 0, maxAtqPoints - atqPoints);
                    }
                    break;
                }
           case KindOfTurret.Furnace:
                {
                    foreach (var enemies in GameManager.Instance.enemies)
                    {
                        if (enemies == enemy)
                            return;

                        Vector2 objPos = enemies.transform.position;

                        float distance = Vector2.Distance(enemy.transform.position, objPos);

                        float explosionRange = 1;

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

                    break;
                }

            default:
                
                break;
        }
    }

    public virtual void PassiveLevelmax(EnemiesTemp enemy)
    {
        Debug.LogError("Shouln't be using this virtual method (PassiveLevelmax)");
    }
}

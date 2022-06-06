using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Turret : MonoBehaviour
{
    [SerializeField]
    private TurretDatabase turretDatabase;

    [SerializeField]
    private GameManager gameManager;

    private GameObject rangeSprite;
    private float rangeMult = 1.52f;
    private Vector3 localScale;

    public SpriteRenderer spriteRenderer;

    #region Turret Stats

    public string label { get; private set; }
    public KindOfTurret kindOfTurret { get; private set; }

    //Turret Stats
    public float healthPoints { get; private set; }
    [Range(0, 10)]
    public float range;
    public int turretPrice { get; private set; }
    public int nbrOfTarget { get; private set; }
    public TargetType targetType { get; private set; }
    [Range(0, 10)]
    public int currentLevel;
    public int maxLevel = 3;

    //Attack Stats
    public int atqPoints { get; private set; }
    public float atqPtsBonus { get; set; }
    public float fireRate { get; private set; }
    public int maxAtqPoints { get; private set; }
    protected bool isAtqCap;

    //Design
    public Sprite inGameDesign { get; private set; }
    public Sprite UIDesign { get; private set; }

    //Passive Stats
    public float basePassiveParameters { get; private set; }
    public float maxPassiveParameters { get; private set; }
    public float capPassive { get; private set; }
    #endregion

    private bool isInside;

    public float fireCountDown { get; protected set; } = 0.0f;

    public bool canBeMoved = false;

    public List<EnemiesTemp> targets = new List<EnemiesTemp>();

    public void Awake()
    {
        if (turretDatabase == null)
        {
            TurretDatabase turretDatabaseNew = (TurretDatabase)ScriptableObject.CreateInstance(typeof(TurretDatabase));

            turretDatabase = turretDatabaseNew; 
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("There is no GameManager in the scene");
        }

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        InitTurretData(BuildManager.Instance.turretToBuild);
    }

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
        rangeSprite.transform.localScale = new Vector3(localScale.x * range, localScale.y * range, 0);//1.265
        #endregion

        spriteRenderer.sprite = inGameDesign;
    }

    public void InitTurretData(KindOfTurret type)
    {
        TurretData turretData = gameManager.GetStatsKindOfTurret(type);

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
        if (turretData.targetType == TargetType.DefaultDoNotUseIt)
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
        range = turretData.range * rangeMult;// x 1.52
        nbrOfTarget = turretData.nbrOfTarget;
        turretPrice = turretData.turretPrice;

        atqPoints = turretData.atqPoints;
        atqPtsBonus = 0;
        fireRate = turretData.fireRate;
        maxAtqPoints = turretData.maxAtqPoints;
        isAtqCap = turretData.activeAtqCap;

        inGameDesign = turretData.inGameDesign;
        UIDesign = turretData.UIDesign;

        basePassiveParameters = turretData.basePassiveParameters;
        maxPassiveParameters = turretData.maxPassiveParameters;
        capPassive = turretData.capPassive;

        currentLevel = 0;
        #endregion
    }

    /*private void OnMouseDrag()
    {
        if (!canBeMoved)
            return;

        this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.transform.position = new Vector3(
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 
            0);
    }*/

    private void OnMouseDown()
    {
        BuildManager.Instance.shop.selectedTurretInGame = this.gameObject;
    }

    private void Update()
    {
        // Change the visibility range // need opti
        rangeSprite.transform.localScale = new Vector3(localScale.x * range, localScale.y * range, 0);

        Vector3 origin = transform.position;
        ChooseTarget(origin);
    }

    public virtual void ChooseTarget(Vector3 origin)
    {
        if (gameManager.enemies.Count <= 0)
            return;

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

        if (targets.Count <= 0)
            return;

        #region Normal Attack
        /*//Attack the first target to enter the range until it die or goes out of range

        Vector3 firstTarget = targets[0].transform.position - origin;

        Debug.DrawLine(origin, origin + firstTarget, Color.red);

        EnemiesTemp enemyScript = targets[0].GetComponent<EnemiesTemp>();

        if (fireCountDown <= 0f)
        {
            Shoot(enemyScript);
            fireCountDown = 1 / fireRate;
        }
        fireCountDown -= Time.deltaTime / 2;*/
        #endregion

        EnemiesTemp currentTarget = targets[0];

        switch (kindOfTurret)
        {
            case KindOfTurret.Basic:
                {
                    //


                    break;
                }
            case KindOfTurret.SniperTower:
                {
                    //Search for the target with the highest maxHealth
                    foreach (var target in targets)
                    {
                        if (target.startingHealth > currentTarget.startingHealth)
                        {
                            currentTarget = target;
                        }
                    }
                    break;
                }
            case KindOfTurret.Furnace:
                {
                    SortListClosestToTruckAndDoesntBurn(targets);

                    bool allTargetAreBurning = false;

                    foreach (var target in targets)
                    {
                        if (target.isBurning)
                        {
                            allTargetAreBurning = true;
                        }
                        else
                        {
                            target.isBurning = false;
                            break;
                        }
                    }

                    if (allTargetAreBurning)
                    {
                        currentTarget = ChooseTargetClosestToTruck();
                    }
                    break;
                }
            case KindOfTurret.Zap:
                {
                    // 


                    break;
                }
            default:

                break;
        }

        #region RayToTarget
        Vector3 rayToTarget = currentTarget.transform.position - origin;
        Debug.DrawLine(origin, origin + rayToTarget, Color.red);
        #endregion

        if (fireCountDown <= 0f)
        {
            Shoot(currentTarget);
            fireCountDown = 1 / fireRate;
        }

        fireCountDown -= Time.deltaTime;
    }

    public virtual void Shoot(EnemiesTemp enemy)
    {
        TurretPassive(enemy);

        if (currentLevel >= maxLevel)
            PassiveLevelmax(enemy);

        float damage = atqPoints + atqPtsBonus;

        if (isAtqCap)
        {
             damage = Mathf.Clamp(atqPoints + atqPtsBonus, 0, maxAtqPoints);
        }

        enemy.TakeDamage(damage);

        //Add this turret to the attacking turret of the ennemy
        if (!enemy.attackingTurret.Contains(this))
        {
            enemy.attackingTurret.Add(this);
        }

        if (enemy == null)
            targets.Remove(enemy);
    }

    public virtual void TurretPassive(EnemiesTemp enemy)
    {
        switch (kindOfTurret)
        {
            case KindOfTurret.SniperTower:
                {
                    // inflicts % of the target's max hp per attack

                    float damageBonusBaseOnHP = basePassiveParameters / 100;

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
                    // Enemies that suffer 5 attacks are burned burned enemies suffer 1 point of damage

                    enemy.nbrOfAtqSuffed += Mathf.Clamp(1,0,5);

                    if (enemy.nbrOfAtqSuffed >= capPassive/*5*/)
                    {
                        float burnDuration = 5.0f;
                        float damage = basePassiveParameters/*1*/;
                        float damageBasedOnMaxHealth = maxPassiveParameters;

                        if (!enemy.isBurning)
                        {
                            enemy.StartCoroutine(enemy.Burn(burnDuration, damage, true, maxPassiveParameters/*1*/));
                        }

                        //float explosionRange = 1;

                        foreach (var enemies in GameManager.Instance.enemies)
                        {
                            if (enemies == enemy)
                                return;

                            Vector2 objPos = enemies.transform.position;

                            float distance = Vector2.Distance(enemy.transform.position, objPos);

                            bool isInside = distance < explosionRange;

                            if (isInside)
                            {
                                if (currentLevel >= maxLevel)
                                {
                                    enemies.Burn(burnDuration, damage, true, maxPassiveParameters/*1*/);
                                }
                                else //Is not max level
                                {
                                    enemies.Burn(burnDuration, damage, false, 0);
                                }
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
        switch (kindOfTurret)
        {
            case KindOfTurret.Basic:
                {
                    //


                    break;
                }
            case KindOfTurret.SniperTower: // TO DO
                {
                    // the enemies are pushed back 2 squares
                    int pushHowFar = (int)maxPassiveParameters;
                    
                    break;
                }
            // case KindOfTurret.Furnace: Is in the basic passive
            case KindOfTurret.Zap: 
                {
                    // 


                    break;
                }
            default:

                break;
        }
    }

    public void Upgrade()
    {
        
    }

    #region Util Function
    public bool IsPointInsideCone(Vector3 point, Vector3 coneOrigin, Vector3 coneDirection, int maxAngle, int maxDistance)
    {
        var distanceToConeOrigin = (point - coneOrigin).magnitude;
        if (distanceToConeOrigin < maxDistance)
        {
            var pointDirection = point - coneOrigin;
            var angle = Vector3.Angle(coneDirection, pointDirection);
            if (angle < maxAngle)
                return true;
        }
        return false;
    }

    public static void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    public EnemiesTemp ChooseTargetClosestToTruck()
    {
        // Choose the target closest to get to the truck
        EnemiesTemp currentTarget = targets[0];

        foreach (var target in targets)
        {
            if (target == currentTarget)
                continue;

            if (currentTarget.pathVectorList.Count > target.pathVectorList.Count)
            {
                currentTarget = target;
            }
            else if (currentTarget.pathVectorList.Count == target.pathVectorList.Count
                && currentTarget.distanceToNextTarget > target.distanceToNextTarget)
            {
                currentTarget = target;
            }
        }

        return currentTarget;
    }

    //TriCocktail
    private void SortListClosestToTruckAndDoesntBurn(List<EnemiesTemp> list, int index = 1, bool hasSwap = false)
    {
        for (int i = 0; i < list.Count - index; i++)
        {
            if (list[i].isBurning || list[i + 1].isBurning)
            {
                if (list[i].isBurning && !list[i + 1].isBurning)
                {
                    Swap<EnemiesTemp>(list, i, i + 1);

                    hasSwap = true;
                    continue;
                }

                continue;
            }
            else if (list[i].pathVectorList.Count > list[i + 1].pathVectorList.Count)
            {
                Swap<EnemiesTemp>(list, i, i + 1);

                hasSwap = true;
            }
            else if (list[i].pathVectorList.Count == list[i + 1].pathVectorList.Count
            && list[i].distanceToNextTarget > list[i + 1].distanceToNextTarget)
            {
                Swap<EnemiesTemp>(list, i, i + 1);

                hasSwap = true;
            }
        }

        for (int i = list.Count - index - 1; i > 0; --i)
        {
            if (list[i].isBurning || list[i - 1].isBurning)
            {
                if (!list[i].isBurning && list[i - 1].isBurning)
                {
                    Swap<EnemiesTemp>(list, i, i - 1);

                    hasSwap = true;
                    continue;
                }

                continue;
            }
            
            else if (list[i].pathVectorList.Count < list[i - 1].pathVectorList.Count)
            {
                Swap<EnemiesTemp>(list, i, i - 1);

                hasSwap = true;
            }
            else if (list[i].pathVectorList.Count == list[i - 1].pathVectorList.Count
            && list[i].distanceToNextTarget < list[i - 1].distanceToNextTarget)
            {
                Swap<EnemiesTemp>(list, i, i - 1);

                hasSwap = true;
            }
        }

        if (hasSwap)
        {
            SortListClosestToTruckAndDoesntBurn(list, ++index);
        }
    }

    private  void SortListClosestToTruck(List<EnemiesTemp> list, int index = 1, bool hasSwap = false)
    {

        for (int i = 0; i < list.Count - index; i++)
        {
            if (list[i].pathVectorList.Count > list[i + 1].pathVectorList.Count)
            {
                Swap<EnemiesTemp>(list, i, i + 1);

                hasSwap = true;
            }
            else if (list[i].pathVectorList.Count == list[i + 1].pathVectorList.Count
            && list[i].distanceToNextTarget > list[i + 1].distanceToNextTarget)
            {
                Swap<EnemiesTemp>(list, i, i + 1);

                hasSwap = true;
            }
        }

        for (int i = list.Count - index - 1; i > 0; --i)
        {
            if (list[i].pathVectorList.Count < list[i - 1].pathVectorList.Count)
            {
                Swap<EnemiesTemp>(list, i, i - 1);

                hasSwap = true;
            }
            else if (list[i].pathVectorList.Count == list[i - 1].pathVectorList.Count
            && list[i].distanceToNextTarget < list[i + 1].distanceToNextTarget)
            {
                Swap<EnemiesTemp>(list, i, i - 1);

                hasSwap = true;
            }
        }

        if (hasSwap)
        {
            SortListClosestToTruck(list, ++index);
        }
    }
    #endregion
}

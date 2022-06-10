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
    private TurretData turretData;

    [SerializeField]
    private GameManager gameManager;

    private GameObject rangeSprite;
    private Vector3 localScale;

    public SpriteRenderer spriteRenderer;

    [Range(1, 180)]
    private int fireAngle = 20;

    private bool doOnce = false;

    #region Turret Stats

    public string label { get; private set; }
    public KindOfTurret kindOfTurret { get; private set; }

    //Turret Stats
    public float maxHealthPoint { get; private set; }
    public float currentHP;
    [Range(0, 10)]
    public float range;
    public int turretPrice { get; private set; }
    public int nbrOfTarget { get; private set; }
    public TargetType targetType { get; private set; }
    [Range(1, 3)]
    public int currentLevel = 1;
    public int maxLevel { get; private set; } = 3;
    public bool isMaxLevel { get; private set; } = false;

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
    private EnemiesTemp currentTarget = null;

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
            Debug.LogWarning("There is no GameManager in the scene");
        }

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        #region Show the range of the turret
        rangeSprite = Instantiate(gameManager.rangeSprite, this.transform.position, this.transform.rotation, this.transform);

        localScale = rangeSprite.transform.localScale;
        
        #endregion

        InitTurretData(BuildManager.Instance.turretToBuild);

        rangeSprite.transform.localScale = new Vector3(range * 2, range * 2, 0);//1.265
    }

    public void Start()
    {
        if (turretDatabase == null)
        {
            Debug.LogWarning("This turret doens't have a TurretScriptable attach to it : " + this);
            return;
        }

        gameManager = GameManager.Instance;

        gameManager.allTurret.Add(this);

        spriteRenderer.sprite = inGameDesign;
    }

    public void InitTurretData(KindOfTurret type)
    {
        turretData = gameManager.GetStatsKindOfTurret(type);

        if (turretData == null)
        {
            Debug.LogWarning("There is TurretData of the type : " + type);
            return;
        }

        #region CheckStats

        if (turretData.healthPoints <= 0)
            Debug.LogWarning("Shouldn't this turret.healthPoints be : 0" + this);
        if (turretData.range <= 0)
            Debug.LogWarning("Shouldn't this turret.range be : 0" + this);
        if (turretData.nbrOfTarget <= 0)
            Debug.LogWarning("Shouldn't this turret.nbrOfTarget be : 0" + this);
        if (turretData.turretPrice <= 0)
            Debug.LogWarning("Shouldn't this turret.turretPrice be : 0" + this);
        if (turretData.targetType == TargetType.DefaultDoNotUseIt)
            Debug.LogWarning("Shouldn't this turret.targetType be : DEFAULT " + this);
        
        if (turretData.atqPoints <= 0)
            Debug.LogWarning("Shouldn't this turret.atqPoints be : 0" + this);
        if (turretData.fireRate <= 0)
            Debug.LogWarning("Shouldn't this turret.fireRate be : 0" + this);

        /*if (turretData.inGameDesign == null)
            Debug.LogError(this + "Doesn't have a sprite");
        if (turretData.UIDesign == null)
            Debug.LogError(this + "Doesn't have a sprite");*/
        #endregion

        #region Initialize Stats

        label = turretData.label;
        kindOfTurret = turretData.kindOfTurret;

        maxHealthPoint = turretData.healthPoints;
        currentHP = turretData.healthPoints;
        range = RangeConvertion(range, true);

        nbrOfTarget = turretData.nbrOfTarget;
        targetType = turretData.targetType;
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

        currentLevel = 1;
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

        BuildManager.Instance.shop.DisplayCurrentTurretStats();
    }

    private void Update()
    {
        // Change the visibility range // need opti
        rangeSprite.transform.localScale = new Vector3(range * 2, range * 2, 0);

        Vector3 origin = transform.position;

        ChooseTarget(origin);

        // Passive ON all the time
        switch (kindOfTurret)
        {
            case KindOfTurret.Anti_Aerial:
            case KindOfTurret.Central:
            case KindOfTurret.Furnace:
            case KindOfTurret.Viktor:
                if (currentLevel >= maxLevel)
                    PassiveLevelmax(currentTarget);
                break;
            case KindOfTurret.Channelizer:
                TurretPassive(currentTarget = null);
                break;
            default:
                break;
        }

        float rage = (localScale.x + (3 * 2) * localScale.x) / 2;

        rage = (localScale.x - ((3 / 2)/ localScale.x)) *2;

        Debug.Log(rage);
    }

    private void OnDrawGizmos()
    {
        //Handles.DrawWireDisc(transform.position, transform.forward, range);
    }

    public virtual void ChooseTarget(Vector3 origin)
    {
        if (gameManager.enemies.Count <= 0)
        {
            currentTarget = null;
            return;
        }
            
        foreach (var enemy in gameManager.enemies)
        {
            if (enemy == null)
            {
                Debug.LogWarning("There is a null error");
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

        currentTarget = targets[0];

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
        // Passive ON when shooting
        switch (kindOfTurret)
        {
            case KindOfTurret.Basic:
            case KindOfTurret.Mortar:
            case KindOfTurret.Discord:
            case KindOfTurret.SniperTower:
            case KindOfTurret.Channelizer:
            case KindOfTurret.Immobilizer:
            case KindOfTurret.Zap:
            case KindOfTurret.Teleporter:
            //case KindOfTurret.Viktor: not sure
                {
                    TurretPassive(currentTarget);
                    if (currentLevel >= maxLevel)
                        PassiveLevelmax(currentTarget);
                }
                break;
            default:
                break;
        }

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

    public virtual void TurretPassive(EnemiesTemp enemy = null)
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

                    foreach (var target in targets)
                    {
                        Vector3 vectorToTarget = currentTarget.transform.position - this.transform.position;

                        bool isInsideCone = IsPointInsideCone(target.transform.position, this.transform.position, vectorToTarget, fireAngle, range);

                        if (isInsideCone)
                        {
                            target.nbrOfAtqSuffed += Mathf.Clamp(1, 0, 5);

                            if (target.nbrOfAtqSuffed >= capPassive/*5*/)
                            {
                                float burnDuration = 105.0f;
                                float damage = basePassiveParameters/*1*/;
                                float damageBasedOnMaxHealth = maxPassiveParameters;

                                if (!target.isBurning)
                                {
                                    bool isMaxLevel = currentLevel >= maxLevel;

                                    target.StartCoroutine(target.Burn(burnDuration, 1, isMaxLevel, maxPassiveParameters));
                                }
                            }
                        }

                        #region test angle
                        /*if (isInsideCone)
                        {
                            target.GetComponent<SpriteRenderer>().color = Color.red;

                            foreach (var target in gameManager.enemies)
                            {
                                target.GetComponent<SpriteRenderer>().enabled = true;

                                target.GetComponent<SpriteRenderer>().color = Color.blue;
                            }
                        }*/
                        #endregion
                    }
                    break;
                }
            case KindOfTurret.Discord:
                {
                   /* if (enemy.ConfuseCombo <= 10)
                    {
                        enemy.ConfuseCombo += Mathf.Clamp(1,0,10);
                    }
                    StartCoroutine(ConfusionTimer(enemy));*/
                    break;
                }
            case KindOfTurret.Immobilizer:
                {
                    StartCoroutine(StopSpeedTimer(enemy));
                    break;
                }
            case KindOfTurret.Zap:
                {
                    StartCoroutine(IncreaseAttackSpeed(enemy));
                    break;
                }

            default:
                
                break;
        }
    }

    public virtual void PassiveLevelmax(EnemiesTemp enemy)
    {
        //Debug.LogError("PassiveLevelmax");

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
            case KindOfTurret.Furnace: 
                {
                    if (doOnce)
                        return;

                    float burnDuration = 105.0f;
                    float damage = basePassiveParameters/*1*/;
                    float damageBasedOnMaxHealth = maxPassiveParameters;

                    foreach (var enemies in gameManager.enemies)
                    {
                        if (enemies.isBurning)
                        {
                            // Peut causer des problemes 2 Co routine en meme temps
                            enemies.StartCoroutine(enemies.Burn(burnDuration, basePassiveParameters, true, maxPassiveParameters/*1*/));
                        }
                    }

                    doOnce = true;

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
    }

    /*private IEnumerator ConfusionTimer(EnemiesTemp target)
    {
        target.isConfuse = true;
        yield return new WaitForSeconds(1.0f); // A check
        //Check si il se refait toucher 
        if (!target.getTouch)
        {
            target.ConfuseCombo = 0;
            target.isConfuse = false;
            target.getTouch = false;
        }
        else
        {
            StartCoroutine(ConfusionTimer(target));
        }
    }*/

    private IEnumerator StopSpeedTimer(EnemiesTemp target)
    {
        target.currentSpeed = 0;
        yield return new WaitForSeconds(0.5f);
        target.currentSpeed = target.baseSpeed;
    }

    private IEnumerator IncreaseAttackSpeed(EnemiesTemp target)
    {
        if (this.fireRate < 6)
        {
            this.fireRate += 0.1f;
        }
        yield return new WaitForSeconds(1f);
        if (target.getTouch)
        {
            StartCoroutine(IncreaseAttackSpeed(target));
        }
        else
        {
            this.fireRate = 1f;
        }

    }

    public void Upgrade()
    {
        if (isMaxLevel == true)
            return;

        float maxHP = gameManager.GetStatsKindOfTurret(kindOfTurret).healthPoints;
        int atq = gameManager.GetStatsKindOfTurret(kindOfTurret).atqPoints;

        switch (currentLevel)
        {
            case 1:
                if (gameManager.truck.gold < turretPrice + 50)
                {
                    return;
                }  
                else
                {
                    gameManager.truck.gold -= turretPrice + 50;
                }
                currentHP += maxHP;
                maxHealthPoint += maxHP;
                atqPoints += atq;
                break;
            case 2:
                if (gameManager.truck.gold < turretPrice + 75)
                {
                    return;
                }
                else
                {
                    gameManager.truck.gold -= turretPrice + 75;
                }
                currentHP += maxHP;
                maxHealthPoint += maxHP;
                atqPoints += atq;
                break;
            default:
                break;
        }

        currentLevel += Mathf.Clamp(1, 0, maxLevel - currentLevel);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if(currentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Pathfinding.Instance.GetGrid().GetXY(transform.position, out int x, out int y);
        Pathfinding.Instance.GetNode(x, y).isTurret = null;
        Pathfinding.Instance.GetNode(x, y).isUsed = false;
        Pathfinding.Instance.mapHasChanged = true;
        Destroy(this.gameObject.transform.parent.gameObject);
        gameManager.allTurret.Remove(this);
    }

    #region Util Function
    public bool IsPointInsideCone(Vector3 point, Vector3 coneOrigin, Vector3 coneDirection, int maxAngle, float maxDistance)
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

    public float RangeConvertion(float range, bool rangeToInGameRange)
    {
        if (rangeToInGameRange)
        {
            range = (localScale.x + (turretData.range * 2) * localScale.x) / 2;
        }
        else // in Game Range to Range in the LD
        {
            /*range = range * 2;
            range = range - localScale.x;
            range = range / localScale.x;
            range = range / 2;*/

            range = ((range * 2 - localScale.x) / localScale.x) / 2;
        }

        return range;
    }
    #endregion
}

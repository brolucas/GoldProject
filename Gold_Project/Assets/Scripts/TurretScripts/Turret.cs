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

    [SerializeField]
    private ParticuleManager particuleManager;
    private GameObject particleShoot;

    private GameObject rangeSprite;
    private GameObject unableRange;
    private SpriteRenderer rangeSpriteSR;
    private SpriteRenderer unableRangeSR;
    private Vector3 localScale;
    public AnimationCurve curve;

    [Header("Sprite")]
    public SpriteRenderer spriteRenderer;
    [SerializeField]
    private GameObject barrelGO;
    [SerializeField]
    private Transform particleSpawnPoint;
    private GameObject bullet = null;
    private Vector3 difference;
    private float rotZ;
    public Animator anim;

    [Header("Other")]

    [Range(1, 180)]
    private int fireAngle = 12;

    private int atqBonusStack = 0;
    private int previousIDTarget = 0;

    private bool doOnce = false;

    private bool hasGeneratorBuffActived;


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
    public float atqPointsBuffGenerator { get; private set; }
    public float atqPtsBonusPassive { get; set; }
    
    public float baseFireRate;
    public float fireRate { get; private set; }
    private float fireRateBonus;
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
    public EnemiesTemp currentTarget { get; set; } = null;
    public EnemiesTemp secondCurrentTarget { get; set; } = null;

    public float fireCountDown { get; protected set; } = 0.0f;
    public float countDown { get; protected set; } = 0.0f;

    public bool canBeMoved = false;

    public List<EnemiesTemp> inRangeEnemies = new List<EnemiesTemp>();

    public List<EnemiesTemp> targetList = new List<EnemiesTemp>();

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

        if (particuleManager == null)
        {
            particuleManager = GetComponent<ParticuleManager>();
        }

        #region Show the range of the turret
        rangeSprite = Instantiate(gameManager.rangeSprite, this.transform.position, this.transform.rotation, this.transform);

        localScale = rangeSprite.transform.localScale;
        
        #endregion


        InitTurretData(BuildManager.Instance.turretToBuild);


        rangeSprite.transform.localScale = new Vector3(range * 2, range * 2, 0);//1.265

        rangeSpriteSR = rangeSprite.GetComponent<SpriteRenderer>();

        StartCoroutine(FadeAttackRange(rangeSpriteSR));

        #region Mortar Range Handle 
        // Do this part of the code only for the Mortar turret
        if (kindOfTurret == KindOfTurret.Mortar)
        {
            unableRange = Instantiate(gameManager.rangeSprite, this.transform.position, this.transform.rotation, this.transform);

            float newRange = RangeConvertion(1, true);

            unableRange.transform.localScale = new Vector3(newRange * 2, newRange * 2, 0);//1.265
            unableRangeSR = unableRange.GetComponent<SpriteRenderer>();
            unableRangeSR.color = new Color(0, 0, 0, 0.17f);

            StartCoroutine(FadeAttackRange(unableRangeSR));
        }
        #endregion
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

        particleShoot = particuleManager.KotToParticules[kindOfTurret];
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
        range = RangeConvertion(turretData.range, true);

        nbrOfTarget = turretData.nbrOfTarget;
        targetType = turretData.targetType;
        turretPrice = turretData.turretPrice;

        atqPoints = turretData.atqPoints;
        atqPtsBonusPassive = 0;
        baseFireRate = turretData.fireRate;
        fireRate = turretData.fireRate;
        fireRateBonus = 0;
        maxAtqPoints = turretData.maxAtqPoints;
        isAtqCap = turretData.activeAtqCap;

        inGameDesign = turretData.inGameDesign;
        UIDesign = turretData.UIDesign;

        barrelGO.GetComponent<SpriteRenderer>().sprite = turretData.barrel;

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
        BuildManager.Instance.shop.selectedItemInGame = this.gameObject;

        BuildManager.Instance.shop.DisplayCurrentTurretStats();

        rangeSpriteSR.color = new Color(rangeSpriteSR.color.r, rangeSpriteSR.color.g, rangeSpriteSR.color.b, 0.0588235294f);

        StartCoroutine(FadeAttackRange(rangeSpriteSR, 5.0f));

        if (kindOfTurret == KindOfTurret.Mortar)
        {
            unableRangeSR.color = new Color(unableRangeSR.color.r, unableRangeSR.color.g, unableRangeSR.color.b, 0.17f);

            StartCoroutine(FadeAttackRange(unableRangeSR, 5.0f));
        }
    }

    private void Update()
    {
        // Change the visibility range // need opti
        rangeSprite.transform.localScale = new Vector3(range * 2, range * 2, 0);

        Vector3 origin = transform.position;

        ChooseTarget(origin);


        if (targetList.Count > 0)
        {
            // Delay between shot
            if (fireCountDown <= 0f)
            {
                if (nbrOfTarget == 1)
                {
                    difference = targetList[0].transform.position - transform.position;
                    rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

                    Shoot(targetList[0]);
                }
                if (nbrOfTarget > 1)
                {
                    foreach (var target in targetList)
                    {
                        difference = target.transform.position - transform.position;
                        rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

                        Shoot(target);
                    }
                }

                fireCountDown = 1 / (fireRate + fireRateBonus);
            }

            fireCountDown -= Time.deltaTime;

            if (bullet != null)
            {
                // Follow the rotation target even after being instantiate 
                switch (kindOfTurret)
                {
                    case KindOfTurret.Spliter:
                        {
                            bullet.GetComponent<SpliterRay>().posOrigin = this.transform.position;
                            break;
                        }
                    case KindOfTurret.Basic:
                    case KindOfTurret.SniperTower:
                    case KindOfTurret.Furnace:
                    case KindOfTurret.Immobilizer:
                    case KindOfTurret.Zap:
                        {
                            bullet.transform.rotation = Quaternion.Euler(-rotZ, 90, 180);
                            break;
                        }
                    case KindOfTurret.Channelizer:
                    case KindOfTurret.Generator:
                    case KindOfTurret.Discord:
                        {
                            bullet.transform.rotation = Quaternion.Euler(0, 0, 90 + rotZ);
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        // Max Level Passive and Passive that are turn ON all the time
        switch (kindOfTurret)
        {
            case KindOfTurret.Basic:
            case KindOfTurret.Generator:
            case KindOfTurret.SniperTower:
                if (currentLevel >= maxLevel)
                    PassiveLevelmax(currentTarget);
                break;
            case KindOfTurret.Channelizer:
                if (inRangeEnemies.Count <= 0)
                    TurretPassive(currentTarget = null);
                break;
            default:
                break;
        }
    }

    #region Turret Attack Method

    public void ChooseTarget(Vector3 origin)
    {
        if (gameManager.enemies.Count <= 0)
        {
            currentTarget = null;
            return;
        }
        
        // Check the distance between this turret and all enemy in game 
        // Add the enemies in a list if they are in the range 
        foreach (var enemy in gameManager.enemies)
        {
            if (enemy == null)
            {
                Debug.LogWarning("There is a null error");
                return;
            }

            Vector2 objPos = enemy.transform.position;

            float distance = Vector2.Distance(objPos, origin);

            if (kindOfTurret == KindOfTurret.Mortar)
            {
                isInside = distance < range && distance > RangeConvertion(1, true);
            }
            else
            {
                isInside = distance < range;
            }
            
            if (isInside)
            {
                if (!inRangeEnemies.Contains(enemy))
                {
                    inRangeEnemies.Add(enemy);
                }
            }
            else
            {
                enemy.attackingTurret.Remove(this);
                targetList.Remove(enemy);
                inRangeEnemies.Remove(enemy);
            }
        }

        if (kindOfTurret == KindOfTurret.Spliter)
        {
            if (inRangeEnemies.Count > 0)
            {
                if (bullet == null)
                {
                    bullet = Instantiate(particleShoot, particleSpawnPoint.transform.position, Quaternion.Euler(-rotZ, 90, 180), this.transform);
                }
            }
            else
            {
                Destroy(bullet);
                bullet = null;
            }

            if (bullet != null)
            {
                bullet.GetComponent<LineRenderer>().SetPosition(1, this.transform.position);
            }
        }

        if (inRangeEnemies.Count <= 0)
            return;

        //targetList is usefull when nbrOfTarget > 1
        targetList.Clear();

        if (kindOfTurret == KindOfTurret.Spliter)
        {
            // currentTarget

            if (currentTarget == null)
            {
                currentTarget = ChooseTargetFarestToTruck();
                targetList.Add(currentTarget);
            }

            if (!inRangeEnemies.Contains(currentTarget))
            {
                currentTarget = ChooseTargetFarestToTruck();
                targetList.Add(currentTarget);
            }
        }
        else
        {
            //Default enemy in case it bug 
            currentTarget = ChooseTargetClosestToTruck();
        }

        switch (kindOfTurret)
        {
            case KindOfTurret.Mortar:
                {
                    // Closest to Truck within 1 to 3 range 
                    // function is a if else in the foreach (var enemy in gameManager.enemies) above


                    break;
                }
            case KindOfTurret.SniperTower:
                {
                    //Search for the target with the highest maxHealth
                    foreach (EnemiesTemp enemy in inRangeEnemies)
                    {
                        if (enemy.startingHealth > currentTarget.startingHealth)
                        {
                            currentTarget = enemy;
                        }
                    }
                    break;
                }
            case KindOfTurret.Furnace:
                {
                    // Sort the list of enemies in range in the order of the enemy closest to the Truck that does not burn.
                    SortListClosestToTruckAndDoesntBurn(inRangeEnemies);

                    bool allTargetAreBurning = false;

                    //Find if all target are burning if yes focus the closest to the Truck
                    // if not the first target in the sorted list above 
                    foreach (EnemiesTemp enemy in inRangeEnemies)
                    {
                        if (enemy.isBurning)
                        {
                            allTargetAreBurning = true;
                        }
                        else
                        {
                            enemy.isBurning = false;
                            break;
                        }
                    }
                    if (allTargetAreBurning)
                    {
                        currentTarget = ChooseTargetClosestToTruck();
                    }
                    else
                    {
                        currentTarget = inRangeEnemies[0];
                    }
                    //Else focus target[0]
                    break;
                }
            case KindOfTurret.Immobilizer:
                {
                    SortListClosestToTruck(inRangeEnemies);

                    int nbrOfTargetFocused = Mathf.Clamp(inRangeEnemies.Count, 1, nbrOfTarget);

                    // Add to the target list the enemies with the highest % of missing health 
                    for (int i = 0; i < nbrOfTargetFocused; i++)
                    {
                        EnemiesTemp enemy = null;

                        if (inRangeEnemies.Count <= 0)
                            return;

                        foreach (var enemies in inRangeEnemies)
                        {
                            if (!targetList.Contains(enemies))
                            {
                                enemy = enemies;
                                break;
                            }
                        }

                        float enemyMissingHealth = (enemy.startingHealth - enemy.currentHealth) / enemy.startingHealth * 100;

                        for (int j = 0; j < inRangeEnemies.Count; j++)
                        {
                            float missingHealth = (inRangeEnemies[j].startingHealth - inRangeEnemies[j].currentHealth) / inRangeEnemies[j].startingHealth * 100;

                            if (targetList.Contains(inRangeEnemies[j]) || enemy == inRangeEnemies[j])
                            {
                                continue;
                            }
                            if (enemyMissingHealth < missingHealth)
                            {
                                enemy = inRangeEnemies[j];
                            }
                        }

                        targetList.Add(enemy);
                    }

                    break;
                }
            case KindOfTurret.Spliter:
                {
                    if (currentLevel >= maxLevel)
                    {
                        if (inRangeEnemies.Count < 2)
                            break;

                        // secondCurrentTarget

                        //First time || Each time the target die or goes out of range
                        if (secondCurrentTarget == null || !inRangeEnemies.Contains(secondCurrentTarget))
                        {
                            ChooseSecondTarget();
                        }

                        targetList.Add(currentTarget);

                        while (secondCurrentTarget == currentTarget)
                        {
                            ChooseSecondTarget();
                        }

                        targetList.Add(secondCurrentTarget);
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


        // Add the only target if it's one target turret and the line above didn't add one already
        if (targetList.Count == 0)
        {
            targetList.Add(currentTarget);
        }

        difference = currentTarget.transform.position - transform.position;
        rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        switch (kindOfTurret)
        {
            case KindOfTurret.Basic:
            case KindOfTurret.Discord:
            case KindOfTurret.SniperTower:
            case KindOfTurret.Furnace:
            case KindOfTurret.Channelizer:
            case KindOfTurret.Generator:
                {
                    barrelGO.transform.localRotation = Quaternion.Euler(0, 0, rotZ);

                    break;
                }
            default:
                break;
        }

        // just a visual debug draw line only visible in the editor
        foreach (var target in targetList)
        {
            #region RayToTarget
            Vector3 rayToTarget = target.transform.position - origin;
            Debug.DrawLine(origin, origin + rayToTarget, Color.red);
            #endregion
        }

        if (kindOfTurret == KindOfTurret.Spliter)
        {
            SpliterRay raySpliterRay = bullet.GetComponent<SpliterRay>();
            raySpliterRay.target = currentTarget.transform;

            if (currentLevel >= maxLevel)
            {
                raySpliterRay.doubleTarget = true;
                raySpliterRay.target2 = secondCurrentTarget.transform;
            }
        }
    }

    public void Shoot(EnemiesTemp enemy)
    {
        // Passive ON when shooting
        switch (kindOfTurret)
        {
            //don't need Level max passive here
            case KindOfTurret.Generator:
                {
                    TurretPassive(enemy);
                    break;
                }
            case KindOfTurret.Basic:
            case KindOfTurret.SniperTower:
            case KindOfTurret.Mortar: 
            case KindOfTurret.Discord:
            case KindOfTurret.Furnace:
            case KindOfTurret.Channelizer:
            case KindOfTurret.Immobilizer:
            case KindOfTurret.Zap:
            case KindOfTurret.Spliter:
                {
                    TurretPassive(enemy);
                    if (currentLevel >= maxLevel)
                        PassiveLevelmax(enemy);
                }
                break;
            default:
                break;
        }

        //Need to be in the shoot not in the passive but it's a passive
        if (kindOfTurret == KindOfTurret.Channelizer)
        {
            if (currentLevel >= maxLevel)
            {
                float damageBaseOnCurrentHealth = enemy.currentHealth * maxPassiveParameters /*4*/ / 100;

                atqPtsBonusPassive = (atqPoints * atqBonusStack) + damageBaseOnCurrentHealth;
            }
            else // not level max
            {
                atqPtsBonusPassive = atqPoints * atqBonusStack;
            }
        }

        float damage = atqPoints + atqPtsBonusPassive + atqPointsBuffGenerator;

        Debug.Log(
            "atqPoints : " + atqPoints + " / " +
            "atqPtsBonusPassive : " + atqPtsBonusPassive + " / " + 
            "atqPointsBuffGenerator : " + atqPointsBuffGenerator);

        if (isAtqCap)
        {
             damage = Mathf.Clamp(atqPoints + atqPtsBonusPassive, 0, maxAtqPoints);
        }

        switch (kindOfTurret)
        {
            // if com don't change it 
            //case KindOfTurret.Spliter:
            case KindOfTurret.Basic:
            case KindOfTurret.SniperTower:
            case KindOfTurret.Immobilizer:
                {
                    bullet = Instantiate(particleShoot, particleSpawnPoint.transform.position, Quaternion.Euler(-rotZ, 90, 180), this.transform);
                    //bullet.GetComponent<rangeLifeTimeParticles>().target = targetList[0].transform;
                    break;
                }
            case KindOfTurret.Furnace:
            case KindOfTurret.Zap:
            
                {
                    bullet = Instantiate(particleShoot, particleSpawnPoint.transform.position, Quaternion.Euler(-rotZ, 90, 180), this.transform);
                    break;
                }
            case KindOfTurret.Mortar:
                {
                    bullet = Instantiate(particleShoot, enemy.transform.position, transform.rotation, this.transform);
                    break;
                }
            case KindOfTurret.Channelizer:
            case KindOfTurret.Generator:
            case KindOfTurret.Discord:
                {
                    bullet = Instantiate(particleShoot, particleSpawnPoint.transform.position, Quaternion.Euler(0, 0, (90 + rotZ)));
                    break;
                }
            default:
                break;
        }

        enemy.TakeDamage(damage);

        if (kindOfTurret == KindOfTurret.Channelizer)
        {
            atqBonusStack = 0;
            atqPtsBonusPassive = 0;
        }

        //Add this turret to the attacking turret of the ennemy
        if (!enemy.attackingTurret.Contains(this))
        {
            enemy.attackingTurret.Add(this);
        }

        if (enemy == null)
            inRangeEnemies.Remove(enemy);
    }

    public void TurretPassive(EnemiesTemp enemy = null)
    {
        switch (kindOfTurret)
        {
            case KindOfTurret.Basic:
                {
                    if (doOnce)
                    {
                        fireRate -= baseFireRate;

                        doOnce = false;
                    }
                    else
                    {
                        fireRate += baseFireRate;

                        doOnce = true;
                    }
                    break;
                }
            case KindOfTurret.Mortar:
                {
                    // Enemies 1-2 range away from the target suffer a 50% slowdown

                    foreach (var enemyAround in gameManager.enemies)
                    {
                        Vector2 objPos = enemyAround.transform.position;

                        float distance = Vector2.Distance(objPos, enemy.transform.position);

                        //Range of the explosion that is going to slow enemies inside 
                        int rangeOfExplosion = 1;

                        if (currentLevel >= maxLevel)
                            rangeOfExplosion = 2;

                        isInside = distance < RangeConvertion(rangeOfExplosion, true);
        
                        if (isInside)
                        {
                            enemyAround.StartCoroutine(enemyAround.HandleSlowingDebuff(1, basePassiveParameters/*50*/));
                        }
                    }
                    break;
                }
            case KindOfTurret.SniperTower:
                {
                    // inflicts % of the target's max hp per attack

                    float damageBonusBaseOnHP = basePassiveParameters / 100; //10

                    if (!isAtqCap)
                    {
                        atqPtsBonusPassive = enemy.startingHealth * damageBonusBaseOnHP; // no cap so may be 9999
                    }
                    else
                    {
                        atqPtsBonusPassive = enemy.startingHealth * damageBonusBaseOnHP;
                        atqPtsBonusPassive = Mathf.Clamp(atqPtsBonusPassive, 0, maxAtqPoints);
                    }
                    break;
                }
            case KindOfTurret.Furnace:
                {
                    // Enemies that suffer 5 attacks are burned burned enemies suffer 1 point of damage

                    foreach (var target in inRangeEnemies)
                    {
                        Vector3 vectorToTarget = currentTarget.transform.position - this.transform.position;

                        bool isInsideCone = IsPointInsideCone(target.transform.position, this.transform.position, vectorToTarget, fireAngle, range);

                        if (isInsideCone)
                        {
                            target.nbrOfAtqSuffed++;
                            target.nbrOfAtqSuffed = Mathf.Clamp(target.nbrOfAtqSuffed, 0, 5);

                            if (target.nbrOfAtqSuffed >= capPassive/*5*/)
                            {
                                float burnDuration = 5.0f;
                                float damage = basePassiveParameters/*1*/;

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
                    if (currentTarget == null)
                    {
                        Debug.LogWarning("currentTarget == null so turret discord will not work as planned");
                        return;
                    }

                    int currentIDTarget = -currentTarget.GetInstanceID();

                    #region check if it's the same target
                    if (previousIDTarget == 0)
                        previousIDTarget = currentIDTarget;

                    if (previousIDTarget == currentIDTarget)
                    {
                        atqBonusStack++;
                        atqBonusStack = Mathf.Clamp(atqBonusStack, 0, 10);
                    }
                    else
                    {
                        atqBonusStack = 0;
                        previousIDTarget = currentIDTarget;
                    }
                    #endregion

                    // the turret increases its damage by 5% per hit on the same target. Cumulative 10 times
                    atqPtsBonusPassive = atqPoints * 5 / 100 * atqBonusStack;

                    //Enemies take 5 damage on impact per discord stack effect(max: 10)
                    if (currentLevel >= maxLevel)
                        atqPtsBonusPassive += 5 * atqBonusStack;

                    break;
                }
            case KindOfTurret.Spliter:
                {
                    if (currentTarget == null)
                    {
                        Debug.LogWarning("currentTarget == null so turret discord will not work as planned");
                        return;
                    }

                    int currentIDTarget = -currentTarget.GetInstanceID();

                    #region check if it's the same target
                    if (previousIDTarget == 0)
                        previousIDTarget = currentIDTarget;

                    if (previousIDTarget == currentIDTarget)
                    {
                        atqBonusStack++;
                        atqBonusStack = Mathf.Clamp(atqBonusStack, 0, 10);
                    }
                    else
                    {
                        atqBonusStack = 0;
                        previousIDTarget = currentIDTarget;
                    }
                    #endregion

                    // Doubles its damage each time the target is identical to the previous one. Cumulative 10 times
                    atqPtsBonusPassive = DoubleAtq(atqBonusStack); 

                    break;
                }
            case KindOfTurret.Immobilizer:
                {
                    StartCoroutine(enemy.StopSpeedTimer());
                    break;
                }
            case KindOfTurret.Zap:
                {
                    StartCoroutine(IncreaseAttackSpeed(enemy));
                    break;
                }
            case KindOfTurret.Generator:
                {
                    gameManager.truck.gold += (int)(atqPoints * basePassiveParameters /*50*/ / 100);
                    break;
                }
            case KindOfTurret.Channelizer:
                {
                    // enter this when shoot "Link to Shoot"
                    if (inRangeEnemies.Count > 0)
                    {
                        return;
                    }

                    //enter this when doesn't shoot "Link to Update"
                    if (countDown <= 0f)
                    {
                        if (atqBonusStack < 5)
                        {
                            atqBonusStack++;
                        }
                        else if (atqBonusStack > 5)
                        {
                            atqBonusStack = Mathf.Clamp(atqBonusStack, 0, 5);
                        }

                        countDown = 1 / (fireRate + fireRateBonus);
                    }

                    countDown -= Time.deltaTime;

                    // -- There is a part of this passive in the Shoot Method -- //

                    break;
                }

            default:
                
                break;
        }
    }

    public void PassiveLevelmax(EnemiesTemp enemy)
    {
        //case KindOfTurret.Mortar: in Normal Passive
        //case KindOfTurret.Furnace: in Normal Passive
        //case KindOfTurret.Discord: in Shoot 

        switch (kindOfTurret)
        {
            case KindOfTurret.SniperTower:
                {
                    if (!doOnce)
                    {
                        range = RangeConvertion(range, false);

                        range++;

                        range = RangeConvertion(range, true);

                        doOnce = true;
                    }

                    break;
                }
            case KindOfTurret.Immobilizer: 
                {
                    // inflicts 8% of missing hp per attack
                    atqPtsBonusPassive = (enemy.startingHealth - enemy.currentHealth) * (maxPassiveParameters / 100);

                    break;
                }
            case KindOfTurret.Zap: 
                {
                    // 


                    break;
                }
            case KindOfTurret.Generator:
                {
                    foreach (var turretAround in gameManager.allTurret)
                    {
                        Vector2 objPos = turretAround.transform.position;

                        float distance = Vector2.Distance(objPos, this.transform.position);

                        //Range of the explosion that is going to slow enemies inside 
                        int buffRange = 1;

                        isInside = distance < RangeConvertion(buffRange, true);

                        if (isInside)
                        {
                            if (!turretAround.hasGeneratorBuffActived)
                            {
                                turretAround.atqPointsBuffGenerator += 15;
                                turretAround.fireRateBonus += turretAround.baseFireRate * maxPassiveParameters / 100;
                                turretAround.hasGeneratorBuffActived = true;
                            }
                        }
                    }
                    break;
                }
            case KindOfTurret.Spliter:
                {
                    if (!doOnce)
                    {
                        nbrOfTarget = 2;
                        doOnce = true;
                    }
                    break;
                }
            default:

                break;
        }
    }

    #endregion

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
        if (currentHP <= 0)
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

    private void OnDestroy()
    {
        if (kindOfTurret == KindOfTurret.Generator)
        {
            foreach (var turretAround in gameManager.allTurret)
            {
                if (turretAround.hasGeneratorBuffActived)
                {
                    turretAround.atqPointsBuffGenerator -= 15;
                    turretAround.fireRateBonus -= 0.15f;
                    turretAround.fireRateBonus = Mathf.Clamp(turretAround.fireRateBonus, 0, 10);
                    turretAround.hasGeneratorBuffActived = false;
                }
            }
        }
    }

    public float DoubleAtq(int howManyTime)
    {
        float atqBonus = atqPoints;

        for (int i = 0; i < howManyTime; i++)
        {
            atqBonus *= 2;
        }

        return atqBonus;
    }

    public void ChooseSecondTarget()
    {
        inRangeEnemies.Remove(currentTarget);

        secondCurrentTarget = ChooseTargetFarestToTruck();

        inRangeEnemies.Add(currentTarget);
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

    private IEnumerator FadeAttackRange(SpriteRenderer rangeSpriteSRFade, float howMuchTimeToFade = 1.5f)
    {
        yield return new WaitForSeconds(howMuchTimeToFade);

        float t = 1f;

        while (t > 0)
        {
            t -= Time.deltaTime;
            float a = curve.Evaluate(t * 0.15f);
            rangeSpriteSRFade.color = new Color(rangeSpriteSRFade.color.r, rangeSpriteSRFade.color.g, rangeSpriteSRFade.color.b, a);
            yield return 0;
        }
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
        // Choose the target closest to get to the Truck
        EnemiesTemp currentTarget = inRangeEnemies[0];

        foreach (var target in inRangeEnemies)
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

    public EnemiesTemp ChooseTargetFarestToTruck(EnemiesTemp firstTarget = null)
{
        // Choose the target closest to get to the Truck
        EnemiesTemp currentTarget = inRangeEnemies[0];

        foreach (var target in inRangeEnemies)
        {
            if (target == currentTarget || target == firstTarget)
                continue;

            if (currentTarget.pathVectorList.Count < target.pathVectorList.Count)
            {
                currentTarget = target;
            }
            else if (currentTarget.pathVectorList.Count == target.pathVectorList.Count
                && currentTarget.distanceToNextTarget < target.distanceToNextTarget)
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

    private void SortListClosestToTruck(List<EnemiesTemp> list, int index = 1, bool hasSwap = false)
    {
        // Check the list 0 > end 
        for (int i = 0; i < list.Count - index; i++)
        {
            // handle when it's easy and pathVectorList.Count is strictly superior
            if (list[i].pathVectorList.Count > list[i + 1].pathVectorList.Count)
            {
                Swap<EnemiesTemp>(list, i, i + 1);

                hasSwap = true;
            }
            // handle pathVectorList.Count equality with a check distance to next target
            else if (list[i].pathVectorList.Count == list[i + 1].pathVectorList.Count
            && list[i].distanceToNextTarget > list[i + 1].distanceToNextTarget)
            {
                Swap<EnemiesTemp>(list, i, i + 1);

                hasSwap = true;
            }
        }

        // Check the list end > 0
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

        // re call the method if has swap until it does not swap anymore
        if (hasSwap)
        {
            SortListClosestToTruck(list, ++index);
        }
    }

    public float RangeConvertion(float range, bool rangeToInGameRange)
    {
        if (rangeToInGameRange)
        {
            range = (localScale.x + (range * 2) * localScale.x) / 2;
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

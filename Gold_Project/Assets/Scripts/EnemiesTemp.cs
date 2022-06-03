using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesTemp : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float startingHealth = 2500.0f;
    public float currentHealth = 0.0f;
    public int speed = 3;

    public bool isFlying;

    public float goldValue;

    //Furnace
    public bool isBurning = false;
    public int nbrOfAtqSuffed = 0;

    //Flash on damage
    private bool isInvisible = false;

    public float damagePerSeconds = 0.0f;

    public List<Turret> attackingTurret = new List<Turret>();

    public int currentPathIndex;
    public List<Vector3> pathVectorList;
    public Transform endPoint;

    public void Start()
    {
        //GameManager.Instance.enemies.Add(this);
        SetTargetPosition(endPoint.transform.position);
        currentHealth = startingHealth;

        StartCoroutine(DamagePerSeconds());
        //this.GetComponent<Rigidbody2D>().AddForce(new Vector2(500 * speed * Time.deltaTime, 0));
    }

    public void Update()
    {
        HandleMovement();
        if (pathVectorList != null)
        {
            SetTargetPosition(endPoint.transform.position);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= Mathf.Clamp(damage, 0, currentHealth);

        StartCoroutine(FlashOnDamage());

        if (currentHealth <= 0)
        {
            //Let this here bc should give us gold when hit the truck
            truck.gold += this.goldValue;
            StartCoroutine(Die());
        }
    }

    public IEnumerator FlashOnDamage()
    {
        if (isInvisible == false)
        {
            isInvisible = true;

            Color tmp = this.GetComponent<SpriteRenderer>().color;
            tmp.a = 0f;
            this.GetComponent<SpriteRenderer>().color = tmp;

            yield return new WaitForSeconds(0.1f);

            tmp.a = 255f;
            this.GetComponent<SpriteRenderer>().color = tmp;

            isInvisible = false;
        }
    }

    public IEnumerator Burn(float duration, float damage, bool isMaxLevelPassiveActive = false, float maxPassiveParameters = 0)
    {
        isBurning = true;

        while (duration > 0)
        {
            TakeDamage(damage);

            if (isMaxLevelPassiveActive)
            {
                TakeDamage(startingHealth * maxPassiveParameters);
            }

            yield return new WaitForSeconds(1.0f);
            duration--;
        }

        isBurning = false;
    }

    public IEnumerator DamagePerSeconds()
    {
        while (currentHealth > 0)
        {
            float baseHealth = currentHealth;

            yield return new WaitForSeconds(1f);

            baseHealth -= currentHealth;

            damagePerSeconds = baseHealth;
        }
    }

    /*public void OnDestroy()
    {
        // Just in case take it off if optimization 
        // If the target isn't clear off the turrets will bug
        foreach (var turret in GameManager.Instance.allTurret)
        {
            turret.targets.Remove(this);
        }
    }*/

    public IEnumerator Die()
    {
        foreach (var turret in attackingTurret)
        {
            Turret turretAttacking = turret.GetComponent<Turret>();

            turretAttacking.targets.Remove(this);
        }

        attackingTurret.Clear();
        GameManager.Instance.enemies.Remove(this);

        WaveSpawner.enemyAlive--;

        //Wait until this enemy have been erased from all list before destroying it
        yield return new WaitUntil(() => !GameManager.Instance.enemies.Contains(this));
        
        Destroy(gameObject);
    }

    private void HandleMovement()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                //float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count)
                {
                    pathVectorList = null;
                }
            }
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(this.transform.position, targetPosition);

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }
    }

    public void SetPath(List<Vector3> newPath)
    {
        currentPathIndex = 0;
        pathVectorList = newPath;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    GROUND,
    FLY,
    TERRORIST
};

public class EnemiesTemp : MonoBehaviour
{
    public float startingHealth = 2500.0f;
    public float currentHealth = 0.0f;
    public EnemyType type;

    public int speed = 3;

    public float goldValue;

    public bool isBurning = false;
    private bool isInvisible = false;

    public float damagePerSeconds = 0.0f;

    public List<Turret> attackingTurret = new List<Turret>();

    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    public GameObject endPoint;
    //public float startTime = 0.0f;

    public void Start()
    {
        GameManager.Instance.enemies.Add(this);

        currentHealth = startingHealth;

        StartCoroutine(DamagePerSeconds());
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(500 * speed * Time.deltaTime, 0));
    }

    public void Update()
    {
        HandleMovement();
        if(pathVectorList != null)
        {
            SetTargetPosition(endPoint.transform.position);
        }
    }
    private void HandleMovement()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 1f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
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
    public IEnumerator SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(this.transform.position, targetPosition);

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }
        yield return new WaitForSeconds(0.1f);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= Mathf.Clamp(damage, 0, currentHealth);

        StartCoroutine(FlashOnDamage());

        if (currentHealth <= 0)
        {
            truck.gold += this.goldValue;
            Die();
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

    public void OnDestroy()
    {
        
    }

    public IEnumerator Burn()
    {
        isBurning = true;

        while (currentHealth > 0)
        {
            TakeDamage(startingHealth * 0.01f);
            yield return new WaitForSeconds(1.0f);
        }
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
    public void Die()
    {
        // remove the enemy from the lists
        foreach (var turret in GameManager.Instance.allTurret)
        {
            GameManager.Instance.enemies.Remove(this);

            if (turret.targets.Contains(this))
            {
                Turret turretAttacking = turret.GetComponent<Turret>();

                turretAttacking.targets.Remove(this);
            }
        }

        WaveSpawner.enemyAlive--;

        Destroy(gameObject);
    }

    public void SetPath(List<Vector3> newPath)
    {
        currentPathIndex = 0;
        pathVectorList = newPath;
    }
}

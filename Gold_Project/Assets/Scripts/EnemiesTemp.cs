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

    public void Start()
    {
        GameManager.Instance.enemies.Add(this);

        currentHealth = startingHealth;

        StartCoroutine(DamagePerSeconds());
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(500 * speed * Time.deltaTime, 0));
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= Mathf.Clamp(damage, 0, currentHealth);

        StartCoroutine(FlashOnDamage());

        if (currentHealth <= 0)
        {
            //Let this here bc should give us gold when hit the truck
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

    public void OnDestroy()
    {
        // Just in case take it off if optimization 
        // If the target isn't clear off the turrets will bug
        foreach (var turret in GameManager.Instance.allTurret)
        {
            turret.targets.Remove(this);
        }
    }

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
}

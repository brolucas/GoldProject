using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesTemp : MonoBehaviour
{
    public float startingHealth = 2500.0f;
    public float currentHealth = 0.0f;

    public int speed = 3;

    public bool isFlying;

    public float goldValue;

    public bool isBurning = false;
    private bool isInvisible = false;

    public float damagePerSeconds = 0.0f;

    public List<Turret> attackingTurret = new List<Turret>();

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
        
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= Mathf.Clamp(damage, 0, currentHealth);

        StartCoroutine(FlashOnDamage());

        if (currentHealth <= 0)
        {
            foreach (var turret in attackingTurret)
            {
                Turret turretAttacking = turret.GetComponent<Turret>();

                turretAttacking.targets.Remove(this);

                //turretAttacking.fireCountDown = 0;

                GameManager.Instance.enemies.Remove(this);

            }
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
        WaveSpawner.enemyAlive--;
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesTemp : MonoBehaviour
{
    public float health = 2500;

    public List<RadialTrigger> attackingTurret = new List<RadialTrigger>();

    public float startTime = 0.0f;

    public void Awake()
    {
        GameManager.Instance.enemies.Add(this);
    }

    public float durationToDie = 0.0f;

    public void TakeDamage(float damage)
    {
        health -= Mathf.Clamp(damage, 0, health);

        if (health <= 0)
        {
            foreach (var turret in attackingTurret)
            {
                RadialTrigger turretAttacking = turret.GetComponent<RadialTrigger>();

                turretAttacking.targets.Remove(this);

                turretAttacking.nextActionTime = 0;

                GameManager.Instance.enemies.Remove(this); 
            }

            Debug.Log(durationToDie);

            Destroy(this.gameObject);
        }
    }

}

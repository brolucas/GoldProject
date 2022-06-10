using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesTemp : MonoBehaviour
{
	[Header("Enemy Stats")]
	public float startingHealth = 2500.0f;
	public float currentHealth = 0.0f;
	public int baseSpeed = 3;
	public int currentSpeed = 3;
	public int damage;
	public float fireRate;

	public bool isFlying;

	public float goldValue;

	public bool getTouch = false;
	public bool getZapped = false;

	//Discord 
	public bool isConfuse = false;
	public int ConfuseCombo = 0;

	//Furnace
	public bool isBurning = false;
	public int nbrOfAtqSuffed = 0;

	public float timeToDie = 0;
	public bool tookDamage = false;

	//Flash on damage
	private bool isInvisible = false;

	public float damagePerSeconds = 0.0f;

	public List<Turret> attackingTurret = new List<Turret>();

	public float distanceToNextTarget;
	public int currentPathIndex;
	public List<Vector3> pathVectorList;
	public Transform endPoint;

	[Header("Pathfinding variables")]
	private bool hasArrived;
	private bool canMove;

	public void Start()
	{
		endPoint = Pathfinding.Instance.endPoint;
		GameManager.Instance.enemies.Add(this);
		SetTargetPosition(endPoint.position);
		currentHealth = startingHealth;

		StartCoroutine(DamagePerSeconds());
		//this.GetComponent<Rigidbody2D>().AddForce(new Vector2(500 * speed * Time.deltaTime, 0));

		hasArrived = false;
		canMove = true;
	}

	public void Update()
	{
        if(canMove)
		{
			HandleMovement();
        }
		if (pathVectorList.Count > 1)
		{
			Pathfinding.Instance.GetGrid().GetXY(pathVectorList[0], out int x, out int y);
			PathNode node = Pathfinding.Instance.GetNode(x, y);
			if(node.isTurret != null || node.isBarricade != null)
			{
				canMove = false;
				if (fireRate <=0)
                {
					if (node.isTurret != null)
                    {
						node.isTurret.GetComponentInChildren<Turret>().takeDamage(damage);
                    }
					else
					{
						//node.isBarricade.takeDamage();
					}
					fireRate = 1;
                }

			}
			else
			{
				canMove = true;
			}
		}
		if (pathVectorList != null && (hasArrived || Pathfinding.Instance.mapHasChanged))
		{
			SetTargetPosition(endPoint.position);
			Pathfinding.Instance.mapHasChanged = false;
			if (hasArrived) hasArrived = false;
		}

		if (tookDamage)
		{
			TimeToDie(Time.deltaTime);
		}

		fireRate -= Time.deltaTime;
	}

	public void TakeDamage(float damage)
	{
		if (!tookDamage)
		{
			getZapped = true;
			getTouch = true;
			if (isConfuse)
			{
				damage += damage * (0.05f * ConfuseCombo);
			}

			tookDamage = true;
			timeToDie = 1;
		}

		currentHealth -= Mathf.Clamp(damage, 0, currentHealth);

		//StartCoroutine(FlashOnDamage());

		getTouch = false;

		if (currentHealth <= 0)
		{
			//Let this here bc should give us gold when hit the truck
			GameManager.Instance.truck.gold += this.goldValue;
			StartCoroutine(Die());
		}
	}

	public void TimeToDie(float delta)
	{
		if (currentHealth <= 0)
			return;

		timeToDie += delta;
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

		if (isMaxLevelPassiveActive == false)
		{
			foreach (var turret in attackingTurret)
			{
				if (turret.kindOfTurret == KindOfTurret.Furnace)
				{
					if (turret.currentLevel >= turret.maxLevel)
					{
						isMaxLevelPassiveActive = true;
					}
				}
			}
		}

		while (duration > 0)
		{
			TakeDamage(damage);

			Debug.Log(damage);

			if (isMaxLevelPassiveActive)
			{
				TakeDamage(startingHealth * maxPassiveParameters / 100);
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

	private void HandleMovement()
	{
		if (pathVectorList != null)
		{
			Vector3 targetPosition = pathVectorList[currentPathIndex];
			distanceToNextTarget = Vector3.Distance(transform.position, targetPosition);
			if (distanceToNextTarget > 0.1f)
			{
				Vector3 moveDir = (targetPosition - transform.position).normalized;
				

				//float distanceBefore = Vector3.Distance(transform.position, targetPosition);
				transform.position = transform.position + moveDir * currentSpeed * Time.deltaTime;
			}
			else
			{
				currentPathIndex++;
				hasArrived = true;
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
		Debug.Log(targetPosition);
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

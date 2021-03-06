using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemiesTemp : MonoBehaviour
{
	[Header("Enemy Stats")]
	public float startingHealth = 2500.0f;
	public float currentHealth = 0.0f;
	public float baseSpeed = 3;
	public float currentSpeed = 3;
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

	// allows to remove this enemy from the target list when he dies 
	public List<Turret> attackingTurret = new List<Turret>();

	public float distanceToNextTarget;
	public int currentPathIndex;
	public List<Vector3> pathVectorList;
	public Transform endPoint;

	[Header("Pathfinding variables")]
	private bool hasArrived;
	private bool canMove;
	public bool isKamikaze;
	private GameObject turretTarget;
	private System.Random alea;

	//feedback dammaged
	private Animator anim;

	[SerializeField] private ParticleSystem part1, part2, part3;

	public void Start()
	{
		endPoint = Pathfinding.Instance.endPoint;
		GameManager.Instance.enemies.Add(this);
		SetTargetPosition(endPoint.position);
		currentHealth = startingHealth;

		StartCoroutine(DamagePerSeconds());

		hasArrived = false;
		canMove = true;
		turretTarget = null;
		alea = new System.Random();

		anim = GetComponent<Animator>();
	}

	public void Update()
	{
        if(canMove)
		{
			HandleMovement();
        }
		if (pathVectorList.Count > 0)
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
						node.isTurret.GetComponentInChildren<Turret>().TakeDamage(damage);
                    }
					else if(node.isBarricade != null)
					{
						node.isBarricade.GetComponent<Baricade>().takeDamage(damage);
					}
					fireRate = 1;
					if (isKamikaze) TakeDamage(10000);
                }

			}
			else
			{
				canMove = true;
			}

            if (isKamikaze)
            {
                if (turretTarget == null)
                {
					List<GameObject> possibleTarget = new List<GameObject>();
					foreach(Vector3 pos in pathVectorList)
					{
						Pathfinding.Instance.GetGrid().GetXY(pos, out int currentX, out int currentY);

						foreach(PathNode possibleNode in Pathfinding.Instance.GetNeighbourList(Pathfinding.Instance.GetNode(currentX, currentY)))
						{
							if(possibleNode.isTurret != null && !possibleTarget.Contains(possibleNode.isTurret))
							{
								possibleTarget.Add(possibleNode.isTurret);
							}
						}
					}
					if(possibleTarget.Count > 0)
					{
						int eventAlea = alea.Next(possibleTarget.Count-1);
						turretTarget = possibleTarget[eventAlea];
					}
                }
                else
                {
					SetTargetPosition(turretTarget.transform.position);
				}
            }
		}
		if (pathVectorList != null && !turretTarget && (hasArrived || Pathfinding.Instance.mapHasChanged))
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
            
			//Let this here bc should give us gold when hit the Truck
			GameManager.Instance.truck.gold += this.goldValue;
			StartCoroutine(Die());
		}
		else
        {
			anim.SetBool("dmg", true);
			StartCoroutine(AnimFeedbackDmg());
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

		nbrOfAtqSuffed = 0;

		isBurning = false;
	}

	public IEnumerator StopSpeedTimer()
	{
		currentSpeed = 0;
		yield return new WaitForSeconds(0.5f);
		currentSpeed = baseSpeed;
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
	IEnumerator AnimFeedbackDmg()
    {
		yield return new WaitForSeconds(.1f);
		anim.SetBool("dmg", false);
			
	}

	public void OnDestroy()
	{
		// If the target isn't clear off the turrets will bug
		foreach (var turret in GameManager.Instance.allTurret)
		{
			turret.inRangeEnemies.Remove(this);
			turret.targetList.Remove(this);
		}
	}

	public IEnumerator HandleSlowingDebuff(int duration, float newDebuffPercent)
    {
		float currentDebuffPercent = (baseSpeed - currentSpeed) / baseSpeed * 100;//ex 0% or 55%

		// currentSpeed != 0 bc we don't want to cancel the immobilize effect
		if (newDebuffPercent > currentDebuffPercent && currentSpeed != 0)
        {
			currentSpeed = baseSpeed;
		}
        else
        {
			yield break;
        }

		float speedDebuff = baseSpeed * newDebuffPercent / 100;

		currentSpeed -= speedDebuff;

		yield return new WaitForSeconds(duration);

		currentSpeed += speedDebuff;
	}

	public IEnumerator Die()
	{

		foreach (var turret in attackingTurret)
		{
			turret.targetList.Remove(this);
			turret.inRangeEnemies.Remove(this);
		}

		attackingTurret.Clear();
		GameManager.Instance.enemies.Remove(this);


		//Wait until this enemy have been erased from all list before destroying it
		yield return new WaitUntil(() => !GameManager.Instance.enemies.Contains(this));
        if (SceneManager.GetActiveScene().name == "Level Tuto" || SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
        {
            WaveSpawner.enemyAlive--;

        }

        if (SceneManager.GetActiveScene().name == "Level 4" || SceneManager.GetActiveScene().name == "Level 5" || SceneManager.GetActiveScene().name == "Level 6")
        {
            WaveSpawner2.enemyAlive--;

        }
        if (SceneManager.GetActiveScene().name == "Level 7" || SceneManager.GetActiveScene().name == "Level 8" || SceneManager.GetActiveScene().name == "Level 9")
        {
            WaveSpawner3.enemyAlive--;

        }

		Instantiate(part1, transform.position, Quaternion.Euler(0, 0, 0));
		Instantiate(part2, transform.position, Quaternion.Euler(0, 0, 0));
		Instantiate(part3, transform.position, Quaternion.Euler(0, 0, 0));
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

				float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

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

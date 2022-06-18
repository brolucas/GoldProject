using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
	public GameObject turretPrefab;

	public Shop shop;

	public KindOfTurret turretToBuild;

	private Deck deck;

    public AudioSource buildSound;

	#region Singleton
	private static BuildManager instance = null;

	public GameObject barricadeToBuild;

	// Game Instance Singleton
	public static BuildManager Instance
	{
		get
		{
			if (instance == null)
				instance = FindObjectOfType<BuildManager>();

				return instance;
		}
	}

	private void Awake()
	{
		// if the singleton hasn't been initialized yet
		if (instance != null)
		{
			Destroy(this.gameObject);
		}

		instance = this;

		Debug.Log(instance);

	}
	#endregion

	public void Start()
	{
		if (turretPrefab == null)
		{
			turretPrefab = (GameObject)Resources.Load("Assets/Prefab/TurretPrefab.prefab", typeof(GameObject));
		}

		// == empty
		turretToBuild = KindOfTurret.DefaultDoNotUseIt;

		deck = GetComponent<Deck>();

		shop = FindObjectOfType<Shop>();
	}

	public GameObject GetTurretToBuild()
	{
		return turretPrefab;
	}

	public void SetTurretToBuild(KindOfTurret kindOfTurretSelected)
	{
		turretToBuild = kindOfTurretSelected;
		barricadeToBuild = null;
	}

	public void SetBarricadeToBuild(GameObject barricade)
	{
		barricadeToBuild = barricade;
		turretToBuild = KindOfTurret.DefaultDoNotUseIt;
	}
	public GameObject GetBarricadeToBuild()
	{
		return barricadeToBuild;
	}

	public void CreateTurret(Vector3 position, float cellSize = 0)
	{
		if (turretToBuild == KindOfTurret.DefaultDoNotUseIt)
			return;

		TurretData turretData = GameManager.Instance.GetStatsKindOfTurret(turretToBuild);

		Pathfinding.Instance.GetGrid().GetXY(position, out int x, out int y);

		if (GameManager.Instance.truck.gold >= turretData.turretPrice && !Pathfinding.Instance.GetNode(x,y).isUsed) 
		{
			if (turretToBuild == KindOfTurret.DefaultDoNotUseIt)
			{
				Debug.LogWarning("You are trying to create a turret but there is no turret selected !");
				return;
			}

            StartCoroutine(BuildSound(buildSound));
			GameObject newTurret = Instantiate(turretPrefab, position, Quaternion.identity);
			Pathfinding.Instance.GetNode(x, y).isTurret = newTurret;
			Pathfinding.Instance.GetNode(x, y).isUsed = true;
			Pathfinding.Instance.mapHasChanged = true;

			if (cellSize > 0)
			{
				newTurret.transform.GetChild(1).localScale = new Vector3(cellSize, cellSize, cellSize);
			}

			turretToBuild = KindOfTurret.DefaultDoNotUseIt;

			GameManager.Instance.truck.gold -= turretData.turretPrice;
		}

	}

    private IEnumerator BuildSound(AudioSource buildSound)
    {
		buildSound.Play();
        yield return new WaitForSeconds(2);
    }

	public void CreateBarricade(Vector3 position)
	{
		if (barricadeToBuild == null) return;

		Pathfinding.Instance.GetGrid().GetXY(position, out int x, out int y);
		if (GameManager.Instance.truck.gold >= barricadeToBuild.GetComponent<Baricade>().price && !Pathfinding.Instance.GetNode(x, y).isUsed)
        {
            StartCoroutine(BuildSound(buildSound));
			GameObject newBarricade = Instantiate(barricadeToBuild, position, Quaternion.identity);
			Pathfinding.Instance.GetNode(x, y).isBarricade = newBarricade;
			Pathfinding.Instance.GetNode(x, y).isUsed = true;
			Pathfinding.Instance.mapHasChanged = true;

			barricadeToBuild = null;

			GameManager.Instance.truck.gold -= newBarricade.GetComponent<Baricade>().price;
		}
	}
}

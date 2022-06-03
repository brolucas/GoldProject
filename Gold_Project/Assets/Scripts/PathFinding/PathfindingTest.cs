using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTest : MonoBehaviour
{
	//public EnemiesTemp enemy;
	private Pathfinding pathfinding;
	public GameObject spawn, end;
	public float cellSize;
	BuildManager buildManager;

	// Start is called before the first frame update
	void Start()
	{
		buildManager = BuildManager.Instance;
		pathfinding = new Pathfinding(13, 7, cellSize, this.transform);
		pathfinding.endPoint = end.transform;

		//enemy.endPoint = end.transform;
	}

	// Update is called once per frame
	void Update()
	{
		/*if (Input.GetMouseButtonDown(0))
		{
			EnemiesTemp enemyTest = Instantiate(enemy, spawn.transform.position, spawn.transform.rotation);
		}

		if (Input.GetMouseButtonDown(1))
		{
			Debug.Log("clic");
			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
			pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
		}*/

		if (buildManager.GetTurretToBuild() == null)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			pathfinding.GetGrid().SetTurret(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}
	}
}

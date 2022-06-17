using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTest : MonoBehaviour
{
	//public EnemiesTemp enemy;
	private Pathfinding pathfinding;
	public GameObject spawn, end;
	public float cellSize;
	public GameObject decorPrefab;
	public LevelDataBase levelDataBase;
	public LevelLabel levelLabel;
	private LevelData levelData;

	// Start is called before the first frame update
	void Start()
	{
		levelData = levelDataBase.levels.Find(LevelData => LevelData.levelLabel == levelLabel);
		pathfinding = new Pathfinding(13, 7, cellSize, this.transform, end.transform, decorPrefab, levelData);

		pathfinding.GetGrid().GetXY(spawn.transform.position, out int spawnX, out int spawnY);
		pathfinding.GetNode(spawnX, spawnY).isUsed = true;
		pathfinding.GetGrid().GetXY(end.transform.position, out int endX, out int endY);
		pathfinding.GetNode(endX, endY).isUsed = true;
		pathfinding.GetNode(endX, endY+1).isUsed = true;
		pathfinding.GetNode(endX, endY-1).isUsed = true;

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


		if (Input.GetMouseButtonDown(0))
		{
			if (BuildManager.Instance.GetTurretToBuild() != null)
				pathfinding.GetGrid().SetTurret(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			if(BuildManager.Instance.GetBarricadeToBuild() != null)
				pathfinding.GetGrid().SetBarricade(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}
	}
}

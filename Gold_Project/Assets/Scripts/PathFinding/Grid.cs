using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	private int width;
	private int height;
	private float cellSize;
	private Vector3 originPos;
	private int[,] gridArray;
	private Cell[,] cellArray;

	public Grid(int width, int height, float cellSize, Vector3 originPos)
	{
		this.width = width;
		this.height = height;
		this.cellSize = cellSize;
		this.originPos = originPos;

		gridArray = new int[width, height];
		cellArray = new Cell[width, height];

		for(int x = 0; x<gridArray.GetLength(0); x++)
		{
			for(int y = 0; y < gridArray.GetLength(1); y++)
			{
				cellArray[x, y] = new Cell();
				Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
				Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f); 
			}
			Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
			Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
		}
	}

	private Vector3 GetWorldPosition(int x, int y)
	{
		return new Vector3(x, y) * cellSize + originPos;
	}

	private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
		x = Mathf.FloorToInt((worldPosition - originPos).x / cellSize);
		y = Mathf.FloorToInt((worldPosition - originPos).y / cellSize);
    }

	public void SetTurret(int x, int y)
	{
		if(x >= 0 && y >= 0 && x < width && y < height)
		{
			cellArray[x, y].isTurret = !cellArray[x, y].isTurret;

			if (BuildManager.Instance.GetTurretToBuild() == null)
				Debug.LogError("turret to build is null");

			Vector3 position = GetWorldPosition(x, y);

			GameObject turret = Instantiate(BuildManager.Instance.GetTurretToBuild(), new Vector3(position.x + cellSize / 2, position.y + cellSize / 2, 0), Quaternion.identity);

			turret.transform.localScale = new Vector3(cellSize, cellSize, cellSize);

			BuildManager.Instance.turretToBuild = null;

			Debug.Log("cell status : turret:" + cellArray[x, y].isTurret + " barricade:" + cellArray[x, y].isBarricade + " Event" + cellArray[x, y].isEvent);
		}
	}

	public void SetTurret(Vector3 worldPosition)
    {
		int x, y;
		GetXY(worldPosition, out x, out y);
		Debug.Log(x + ", " + y);
		SetTurret(x, y);
    }
}

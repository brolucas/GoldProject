using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridObject> : MonoBehaviour
{
	public int width;
	public int height;
	public float cellSize;
	private Vector3 originPos;
	private TGridObject[,] gridArray;
	private Cell[,] cellArray;

	public Grid(int width, int height, float cellSize, Vector3 originPos, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
	{
		this.width = width;
		this.height = height;
		this.cellSize = cellSize;
		this.originPos = originPos;

		gridArray = new TGridObject[width, height];
		cellArray = new Cell[width, height];

		for (int x = 0; x < gridArray.GetLength(0); x++)
		{
			for (int y = 0; y < gridArray.GetLength(1); y++)
			{
				gridArray[x, y] = createGridObject(this, x, y);
			}
		}

		for (int x = 0; x<gridArray.GetLength(0); x++)
		{
			for(int y = 0; y < gridArray.GetLength(1); y++)
			{
				cellArray[x, y] = new Cell(null, x, y);
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
		if (x >= 0 && y >= 0 && x < width && y < height)
		{
			cellArray[x, y].isTurret = !cellArray[x, y].isTurret;

			if (BuildManager.Instance.GetTurretToBuild() == null)
				Debug.LogError("turret to build is null");

			Vector3 position = GetWorldPosition(x, y);

			BuildManager.Instance.CreateTurret(new Vector3(position.x + cellSize / 2, position.y + cellSize / 2));

			//Debug.Log("cell status : turret:" + cellArray[x, y].isTurret + " barricade:" + cellArray[x, y].isBarricade + " Event" + cellArray[x, y].isEvent);
		}
	}
	public void SetTurret(Vector3 worldPosition)
    {
		int x, y;
		GetXY(worldPosition, out x, out y);
		Debug.Log(x + ", " + y);
		SetTurret(x, y);
    }
	public int GetWidth()
	{
		return width;
	}

	public int GetHeight()
	{
		return height;
	}

	public float GetCellSize()
	{
		return cellSize;
	}

	public TGridObject GetGridObject(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < width && y < height)
		{
			return gridArray[x, y];
		}
		else
		{
			return default(TGridObject);
		}
	}

	public TGridObject GetGridObject(Vector3 worldPosition)
	{
		int x, y;
		GetXY(worldPosition, out x, out y);
		return GetGridObject(x, y);
	}
}

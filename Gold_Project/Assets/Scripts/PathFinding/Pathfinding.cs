using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding:MonoBehaviour
{
	private const int MOVE_STRAIGHT_COST = 10;
	private const int MOVE_DIAGONAL_COST = 14;

	public static Pathfinding Instance { get; private set; }
	public Transform endPoint;
	public bool mapHasChanged;

	private Grid<PathNode> grid;
	private List<PathNode> openList;
	private List<PathNode> closedList;

	public Pathfinding(int width, int height, float cellSize, Transform transf, Transform end,GameObject decorPrefab, LevelData levelData)
	{
		Instance = this;
		grid = new Grid<PathNode>(width, height, cellSize, new Vector3(transf.position.x, transf.position.y), (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
		endPoint = end;
		mapHasChanged = false;
		foreach(Decor decor in levelData.decors)
        {
			GetNode(decor.x, decor.y).SetIsWalkable(false);
			GetNode(decor.x, decor.y).isDecor = decor;
			Vector3 position = grid.GetWorldPosition(decor.x, decor.y);
			position = new Vector3(position.x + cellSize / 2, position.y + cellSize / 2);
			GameObject newDecor = Instantiate(decorPrefab, position, Quaternion.identity);
			newDecor.GetComponent<SpriteRenderer>().sprite = decor.image;
		}
	}

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
	{
		grid.GetXY(startWorldPosition, out int startX, out int startY);
		grid.GetXY(endWorldPosition, out int endX, out int endY);

		List<PathNode> path = FindPath(startX, startY, endX, endY);
		if (path == null)
		{
			return null;
		}
		else
		{
			List<Vector3> vectorPath = new List<Vector3>();
			foreach (PathNode pathNode in path)
			{
				vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
			}
			return vectorPath;
		}
	}

	public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
	{
		PathNode startNode = grid.GetGridObject(startX, startY);
		PathNode endNode = grid.GetGridObject(endX, endY);

		openList = new List<PathNode> { startNode };
		closedList = new List<PathNode>();

		for (int x = 0; x < grid.GetWidth(); x++)
		{
			for (int y = 0; y < grid.GetHeight(); y++)
			{
				PathNode pathNode = grid.GetGridObject(x, y);
				pathNode.gCost = int.MaxValue;
				pathNode.CalculateFCost();
				pathNode.cameFromNode = null;
			}
		}

		startNode.gCost = 0;
		startNode.hCost = CalculateDistanceCost(startNode, endNode);
		startNode.CalculateFCost();

		while (openList.Count > 0)
		{
			PathNode currentNode = GetLowestFCostNode(openList);
			if (currentNode == endNode) 
			{
				return CalculatePath(endNode);
			}

			openList.Remove(currentNode);
			closedList.Add(currentNode);

			foreach(PathNode neighbourNode in GetNeighbourList(currentNode))
			{
				if (closedList.Contains(neighbourNode)) continue;
				if(!neighbourNode.isWalkable || neighbourNode.isEvent != null)
				{
					closedList.Add(neighbourNode);
						
					continue;
				}

				int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
				if (neighbourNode.isBarricade) tentativeGCost += 150;
				if (neighbourNode.isTurret) tentativeGCost += 300;

				if (tentativeGCost < neighbourNode.gCost)
				{
					neighbourNode.cameFromNode = currentNode;
					neighbourNode.gCost = tentativeGCost;
					neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
					neighbourNode.CalculateFCost();
				}

				if (!openList.Contains(neighbourNode)){
					openList.Add(neighbourNode);
				}
			}
		}
		return null;
	}

	private List<PathNode> GetNeighbourList(PathNode currentNode)
	{
		List<PathNode> neighbourList = new List<PathNode>();

		// Left
		if (currentNode.x - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
		// right
		if (currentNode.x + 1 < grid.GetWidth()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
		// Down
		if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
		// Up
		if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

		return neighbourList;
	}

	private int CalculateDistanceCost(PathNode a, PathNode b)
	{
		int xDistance = Mathf.Abs(a.x - b.x);
		int yDistance = Mathf.Abs(a.y - b.y);
		int remaining = Mathf.Abs(xDistance - yDistance);
		return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
	}
	private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
	{
		PathNode lowestFCostNode = pathNodeList[0];
		for (int i = 1; i < pathNodeList.Count; i++)
		{
			if (pathNodeList[i].fCost < lowestFCostNode.fCost)
			{
				lowestFCostNode = pathNodeList[i];
			}
		}
		return lowestFCostNode;
	}

	private List<PathNode> CalculatePath(PathNode endNode)
	{
		List<PathNode> path = new List<PathNode>();
		path.Add(endNode);
		PathNode currentNode = endNode;
		while(currentNode.cameFromNode != null)
		{
			path.Add(currentNode.cameFromNode);
			currentNode = currentNode.cameFromNode;
		}
		path.Reverse();
		return path;
	}

	public PathNode GetNode(int x, int y)
	{
		return grid.GetGridObject(x, y);
	}

	public Grid<PathNode> GetGrid()
	{
		return this.grid;
	}

}

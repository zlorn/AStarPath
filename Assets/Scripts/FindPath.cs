using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindPath : MonoBehaviour 
{
	private Map map;
	private Vector3 lastStart = Vector3.zero;
	private Vector3 lastEnd = Vector3.zero;

	void Start () 
	{
		map = GetComponent<Map>();
	}
	
	void Update () 
	{
		FindingPath(map.posA.position, map.posB.position);
	}

	/// <summary>
	/// A Star 寻路
	/// </summary>
	void FindingPath(Vector3 start, Vector3 end) 
	{
		if (lastStart.Equals(start) && lastEnd.Equals(end))
		{
			return;
		}
		lastStart = start;
		lastEnd = end;

		Map.Cell startCell = map.GetCell(start);
		Map.Cell endCell = map.GetCell(end);

		/// <summary>
		/// 所有被考虑来寻找最短路径的格子
		/// </summary>
		List<Map.Cell> openList = new List<Map.Cell>();
		/// <summary>
		/// 不会再被考虑的格子
		/// </summary>
		HashSet<Map.Cell> closeSet = new HashSet<Map.Cell>();
		openList.Add(startCell);

		while (openList.Count > 0)
		{
			Map.Cell curCell = openList[0];

			for (int i = 0; i < openList.Count; i++) 
			{
				// 从 openList 中找出 f 最小的格子
				if (openList[i].f < curCell.f) 
				{
					curCell = openList[i];
				}
			}

			openList.Remove(curCell);
			closeSet.Add(curCell);

			// 寻路完成
			if (curCell == endCell)
			{
				GeneratePath(startCell, endCell);
				return;
			}

			// 判断周围格子，选择一个最优的格子
			foreach (var cell in map.GetNeighbours(curCell)) 
			{
				if (cell.isWall || closeSet.Contains(cell))
					continue;

				int g = curCell.g + GetG(curCell, cell);
				// 如果不在列表中，则加入列表
				if (!openList.Contains(cell))
				{
					cell.g = g;
					cell.h = GetH(cell, endCell);
					cell.parent = curCell;
					openList.Add(cell);
				}
				else
				{
					// 如果已经在列表中，并且 g 更小，则更新 g 和 parent
					if (g < cell.g)
					{
						cell.g = g;
						cell.parent = curCell;
					}
				}
			}
		}
		GeneratePath (startCell, null);
	}

	/// <summary>
	/// 生成路径
	/// </summary>
	void GeneratePath(Map.Cell startCell, Map.Cell endCell) 
	{
		List<Map.Cell> path = new List<Map.Cell>();
		if (endCell != null) 
		{
			Map.Cell temp = endCell;
			while (temp != startCell) 
			{
				path.Add(temp);
				temp = temp.parent;
			}
			// 反转路径
			path.Reverse();
		}
		// 更新路径
		map.UpdatePath(path);
	}

	/// <summary>
	/// 相邻格子的 G 值
	/// 横纵移动代价为 10 ，对角线移动代价为 14
	/// </summary>
	int GetG(Map.Cell a, Map.Cell b)
	{
		if (a.x == b.x || a.y == b.y)
		{
			return 10;
		}
		else 
		{
			return 14;
		}
	}

	/// <summary>
	/// 使用曼哈顿算法获取两个格子之间的 H 值
	/// </summary>
	int GetH(Map.Cell a, Map.Cell b) 
	{
		int w = Mathf.Abs(a.x - b.x);
		int h = Mathf.Abs(a.y - b.y);
		return w + h;
	}

	/// <summary>
	/// 使几何算法获取两个格子之间的 H 值
	/// </summary>
	int GetH2(Map.Cell a, Map.Cell b) 
	{
		int w = Mathf.Abs(a.x - b.x);
		int h = Mathf.Abs(a.y - b.y);
		return Mathf.RoundToInt(Mathf.Sqrt(w * w + h * h));
	}

	/// <summary>
	/// 使用对角算法获取两个格子之间的 H 值
	/// 横纵移动代价为 10 ，对角线移动代价为 14
	/// </summary>
	int GetH3(Map.Cell a, Map.Cell b) 
	{
		int w = Mathf.Abs(a.x - b.x);
		int h = Mathf.Abs(a.y - b.y);
		// 判断到底是那个轴相差的距离更远
		if (w > h) 
		{
			return 14 * h + 10 * (w - h);
		} 
		else 
		{
			return 14 * w + 10 * (h - w);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindPath : MonoBehaviour 
{

	enum HType
	{
		/// <summary>
		/// 曼哈顿算法
		/// </summary>
		Manhattan,
		/// <summary>
		/// 几何算法（欧几里得算法）
		/// </summary>
		Euclidean,
		/// <summary>
		/// 对角算法
		/// </summary>
		Diagonal
	}

	private Map map;

	void Start () 
	{
		map = GetComponent<Map>();
	}

	public void ManhattanFind()
	{
		StartCoroutine(FindingPath(map.posA.position, map.posB.position, HType.Manhattan));
	}

	public void EuclideanFind()
	{
		StartCoroutine(FindingPath(map.posA.position, map.posB.position, HType.Euclidean));
	}

	public void DiagonalFind()
	{
		StartCoroutine(FindingPath(map.posA.position, map.posB.position, HType.Diagonal));
	}

	/// <summary>
	/// A Star 寻路
	/// </summary>
	IEnumerator FindingPath(Vector3 start, Vector3 end, HType hType) 
	{
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

		GeneratePath(startCell, null);
		map.ClearProgress();
		map.UpdateProgress(startCell);

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
				yield break;
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
					cell.h = GetH(cell, endCell, hType);
					cell.parent = curCell;
					openList.Add(cell);

					map.UpdateProgress(cell);
					yield return new WaitForSeconds(0.05f);
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
		GeneratePath(startCell, null);
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
		map.GeneratePath(path);
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
	/// 获取两个格子之间的 H 值
	/// </summary>
	int GetH(Map.Cell a, Map.Cell b, HType hType)
	{
		if (hType == HType.Manhattan)
		{
			return GetHByManhattan(a, b);
		}
		else if (hType == HType.Euclidean)
		{
			return GetHByEuclidean(a, b);
		}
		else
		{
			return GetHByDiagonal(a, b);
		}
	}

	/// <summary>
	/// 使用曼哈顿算法获取两个格子之间的 H 值
	/// 横纵移动代价为 10 ，对角线移动代价为 14
	/// </summary>
	int GetHByManhattan(Map.Cell a, Map.Cell b) 
	{
		int w = Mathf.Abs(a.x - b.x);
		int h = Mathf.Abs(a.y - b.y);
		return (w + h) * 10;
	}

	/// <summary>
	/// 使几何算法获取两个格子之间的 H 值
	/// 横纵移动代价为 10 ，对角线移动代价为 14
	/// </summary>
	int GetHByEuclidean(Map.Cell a, Map.Cell b) 
	{
		int w = Mathf.Abs(a.x - b.x);
		int h = Mathf.Abs(a.y - b.y);
		return Mathf.RoundToInt(Mathf.Sqrt(w * w + h * h)) * 10;
	}

	/// <summary>
	/// 使用对角算法获取两个格子之间的 H 值
	/// 横纵移动代价为 10 ，对角线移动代价为 14
	/// </summary>
	int GetHByDiagonal(Map.Cell a, Map.Cell b) 
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

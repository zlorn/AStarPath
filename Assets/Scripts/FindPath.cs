using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindPath : MonoBehaviour 
{
	private Grid grid;
	private Vector3 lastStart = Vector3.zero;
	private Vector3 lastEnd = Vector3.zero;

	void Start () 
	{
		grid = GetComponent<Grid> ();
	}
	
	void Update () 
	{
		FindingPath(grid.player.position, grid.destPos.position);
	}

	// A Star 寻路
	void FindingPath(Vector3 start, Vector3 end) 
	{
		if (lastStart.Equals(start) && lastEnd.Equals(end))
		{
			return;
		}
		lastStart = start;
		lastEnd = end;

		Grid.NodeItem startNode = grid.GetItem(start);
		Grid.NodeItem endNode = grid.GetItem(end);

		/// <summary>
		/// 所有被考虑来寻找最短路径的格子
		/// </summary>
		List<Grid.NodeItem> openList = new List<Grid.NodeItem> ();
		/// <summary>
		/// 不会再被考虑的格子
		/// </summary>
		HashSet<Grid.NodeItem> closeSet = new HashSet<Grid.NodeItem>();
		openList.Add(startNode);

		while (openList.Count > 0) 
		{
			Grid.NodeItem curNode = openList [0];

			// 从 openList 中找出 fCost 最小的节点
			for (int i = 0; i < openList.Count; i++) 
			{
				if (openList[i].fCost < curNode.fCost) 
				{
					curNode = openList[i];
				}
			}

			openList.Remove(curNode);
			closeSet.Add(curNode);

			// 查找完成
			if (curNode == endNode)
			{
				GeneratePath(startNode, endNode);
				return;
			}

			// 判断周围节点，选择一个最优的节点
			foreach (var item in grid.GetNeibourhood(curNode)) 
			{
				// 如果是墙或者已经在关闭列表中
				if (item.isWall || closeSet.Contains(item))
					continue;

				item.hCost = GetHCost(item, endNode);
				int gCost = curNode.gCost + GetGCost(curNode, item);
				// 如果不在列表中，则加入列表
				if (!openList.Contains(item))
				{
					item.gCost = gCost;
					item.parent = curNode;
					openList.Add(item);
				}
				else
				{
					// 如果已经在列表中，并且 gCost 更小，则更新 gCost 和 parent
					if (gCost < item.gCost)
					{
						item.gCost = gCost;
						item.parent = curNode;
					}
				}
			}
		}
		GeneratePath (startNode, null);
	}

	/// <summary>
	/// 生成路径
	/// </summary>
	void GeneratePath(Grid.NodeItem startNode, Grid.NodeItem endNode) 
	{
		List<Grid.NodeItem> path = new List<Grid.NodeItem>();
		if (endNode != null) 
		{
			Grid.NodeItem temp = endNode;
			while (temp != startNode) 
			{
				path.Add (temp);
				temp = temp.parent;
			}
			// 反转路径
			path.Reverse ();
		}
		// 更新路径
		grid.UpdatePath(path);
	}

	/// <summary>
	/// 相邻节点的 gCost
	/// 横纵移动代价为 10 ，对角线移动代价为 14
	/// </summary>
	int GetGCost(Grid.NodeItem a, Grid.NodeItem b)
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
	/// 使用对角算法获取两个节点之间的 hCost 距离
	/// 横纵移动代价为 10 ，对角线移动代价为 14
	/// </summary>
	int GetHCost(Grid.NodeItem a, Grid.NodeItem b) 
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

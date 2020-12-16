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

			for (int i = 0, max = openList.Count; i < max; i++) 
			{
				if (openList [i].fCost <= curNode.fCost && openList [i].hCost < curNode.hCost) 
				{
					curNode = openList [i];
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
				// 计算当前相邻节点到开始节点距离
				int gCostNew = curNode.gCost + GetNodeDistance(curNode, item);
				// 如果距离更小，或者原来不在开始列表中
				if (gCostNew < item.gCost || !openList.Contains(item)) 
				{
					// 更新与开始节点的距离
					item.gCost = gCostNew;
					// 更新与终点的距离
					item.hCost = GetNodeDistance(item, endNode);
					// 更新父节点为当前选定的节点
					item.parent = curNode;
					// 如果节点是新加入的，将它加入打开列表中
					if (!openList.Contains (item)) 
					{
						openList.Add (item);
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
	/// 使用对角算法获取两个节点之间的距离
	/// </summary>
	int GetNodeDistance(Grid.NodeItem a, Grid.NodeItem b) 
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

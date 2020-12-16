using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour 
{
	public GameObject nodeWall;
	public GameObject node;

	/// <summary>
	/// 节点半径
	/// </summary>
	public float nodeRadius = 0.25f;
	/// <summary>
	/// 过滤墙体所在的层
	/// </summary>
	public LayerMask wallLayer;

	/// <summary>
	/// 玩家
	/// </summary>
	public Transform player;
	/// <summary>
	/// 目标点
	/// </summary>
	public Transform destPos;


	/// <summary>
	/// 寻路节点
	/// </summary>
	public class NodeItem 
	{
		/// <summary>
		/// 是否是墙
		/// </summary>
		public bool isWall;
		// 位置
		public Vector3 pos;
		// 格子坐标
		public int x, y;

		/// <summary>
		/// 距离起点的长度 
		/// </summary>
		public int gCost;
		/// <summary>
		/// 距离目标点的长度
		/// </summary>
		public int hCost;

		/// <summary>
		/// 总的路径长度
		/// </summary>
		public int fCost 
		{
			get { return gCost + hCost; }
		}

		/// <summary>
		/// 父节点
		/// </summary>
		public NodeItem parent;

		public NodeItem(bool isWall, Vector3 pos, int x, int y) 
		{
			this.isWall = isWall;
			this.pos = pos;
			this.x = x;
			this.y = y;
		}
	}

	private NodeItem[,] grid;
	private int w, h;

	private GameObject wallRange, pathRange;
	private List<GameObject> pathObjs = new List<GameObject>();

	void Awake() 
	{
		// 初始化格子
		w = Mathf.RoundToInt(transform.localScale.x * 2);
		h = Mathf.RoundToInt(transform.localScale.y * 2);
		grid = new NodeItem[w, h];

		wallRange = new GameObject ("WallRange");
		pathRange = new GameObject ("PathRange");

		// 将墙的信息写入格子中
		for (int x = 0; x < w; x++) 
		{
			for (int y = 0; y < h; y++) 
			{
				Vector3 pos = new Vector3 (x*0.5f, y*0.5f, -0.25f);
				// 通过节点中心发射圆形射线，检测当前位置是否可以行走
				bool isWall = Physics.CheckSphere (pos, nodeRadius, wallLayer);
				// 构建一个节点
				grid[x, y] = new NodeItem (isWall, pos, x, y);
				// 如果是墙体，则画出不可行走的区域
				if (isWall) 
				{
					GameObject obj = GameObject.Instantiate (nodeWall, pos, Quaternion.identity) as GameObject;
					obj.transform.SetParent (wallRange.transform);
				}
			}
		}
	}

	/// <summary>
	/// 根据坐标获得节点
	/// </summary>
	public NodeItem GetItem(Vector3 position) 
	{
		int x = Mathf.RoundToInt (position.x) * 2;
		int y = Mathf.RoundToInt (position.y) * 2;
		x = Mathf.Clamp (x, 0, w - 1);
		y = Mathf.Clamp (y, 0, h - 1);
		return grid [x, y];
	}

	/// <summary>
	/// 取得周围的节点
	/// </summary>
	public List<NodeItem> GetNeibourhood(NodeItem node) 
	{
		List<NodeItem> list = new List<NodeItem>();
		for (int i = -1; i <= 1; i++) 
		{
			for (int j = -1; j <= 1; j++) 
			{
				// 如果是自己，则跳过
				if (i == 0 && j == 0)
					continue;
				int x = node.x + i;
				int y = node.y + j;
				// 判断是否越界，如果没有，加到列表中
				if (x < w && x >= 0 && y < h && y >= 0)
					list.Add(grid[x, y]);
			}
		}
		return list;
	}

	/// <summary>
	/// 更新路径
	/// </summary>
	public void UpdatePath(List<NodeItem> lines) {
		int curListSize = pathObjs.Count;
		for (int i = 0, max = lines.Count; i < max; i++) 
		{
			if (i < curListSize) 
			{
				pathObjs [i].transform.position = lines [i].pos;
				pathObjs [i].SetActive(true);
			} 
			else 
			{
				GameObject obj = GameObject.Instantiate(node, lines [i].pos, Quaternion.identity) as GameObject;
				obj.transform.SetParent(pathRange.transform);
				pathObjs.Add(obj);
			}
		}
		for (int i = lines.Count; i < curListSize; i++) 
		{
			pathObjs [i].SetActive(false);
		}
	}
}

using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour 
{
	public GameObject wall;
	public GameObject pathPoint;
	public GameObject progressPoint;

	public LayerMask wallLayer;

	public Transform posA;

	public Transform posB;


	public class Cell 
	{
		public bool isWall;
		public Vector3 pos;
		public int x;
		public int y;

		/// <summary>
		/// 距离起点的长度 
		/// </summary>
		public int g;
		
		/// <summary>
		/// 距离目标点的长度
		/// </summary>
		public int h;

		/// <summary>
		/// 总的路径长度
		/// </summary>
		public int f 
		{
			get { return g + h; }
		}

		/// <summary>
		/// 父节点
		/// </summary>
		public Cell parent;

		public Cell(bool isWall, Vector3 pos, int x, int y) 
		{
			this.isWall = isWall;
			this.pos = pos;
			this.x = x;
			this.y = y;
		}
	}

	private Cell[,] cells;
	private int w, h;

	private GameObject wallRoot;
	private GameObject pathRoot;
	private GameObject progressRoot;
	private List<GameObject> pathObjs = new List<GameObject>();
	private List<GameObject> progressObjs = new List<GameObject>();

	void Awake() 
	{
		w = Mathf.RoundToInt(transform.localScale.x);
		h = Mathf.RoundToInt(transform.localScale.y);
		cells = new Cell[w, h];

		wallRoot = new GameObject ("WallRoot");
		pathRoot = new GameObject ("PathRoot");
		progressRoot = new GameObject("ProgressRoot");

		for (int x = 0; x < w; x++) 
		{
			for (int y = 0; y < h; y++) 
			{
				Vector3 pos = new Vector3 (x, y, -0.2f);
				bool isWall = Physics.CheckSphere (pos, 0.5f, wallLayer);
				cells[x, y] = new Cell (isWall, pos, x, y);
				if (isWall) 
				{
					GameObject obj = GameObject.Instantiate (wall, pos, Quaternion.identity) as GameObject;
					obj.transform.SetParent (wallRoot.transform);
				}
			}
		}
	}

	/// <summary>
	/// 根据坐标获得格子
	/// </summary>
	public Cell GetCell(Vector3 position) 
	{
		int x = Mathf.RoundToInt (position.x);
		int y = Mathf.RoundToInt (position.y);
		x = Mathf.Clamp (x, 0, w - 1);
		y = Mathf.Clamp (y, 0, h - 1);
		return cells[x, y];
	}

	/// <summary>
	/// 取得周围的格子
	/// </summary>
	public List<Cell> GetNeighbours(Cell cell) 
	{
		List<Cell> list = new List<Cell>();
		for (int i = -1; i <= 1; i++) 
		{
			for (int j = -1; j <= 1; j++) 
			{
				if (i == 0 && j == 0)
					continue;
				int x = cell.x + i;
				int y = cell.y + j;
				// 判断是否越界，如果没有，加到列表中
				if (x < w && x >= 0 && y < h && y >= 0)
					list.Add(cells[x, y]);
			}
		}
		return list;
	}

	/// <summary>
	/// 生成路径
	/// </summary>
	public void GeneratePath(List<Cell> lines) 
	{
		for (int i = pathObjs.Count - 1; i >= 0; i --)
		{
			Destroy(pathObjs[i]);
			pathObjs.RemoveAt(i);
		}

		for (int i = 0; i < lines.Count; i++) 
		{
			GameObject obj = GameObject.Instantiate(pathPoint, lines[i].pos, Quaternion.identity);
			obj.transform.SetParent(pathRoot.transform);
			pathObjs.Add(obj);
		}
	}

	public void ClearProgress()
	{
		for (int i = progressObjs.Count - 1; i >= 0; i--)
		{
			Destroy(progressObjs[i]);
			progressObjs.RemoveAt(i);
		}
	}

	public void UpdateProgress(Cell cell)
	{
		GameObject obj = GameObject.Instantiate(progressPoint, cell.pos + new Vector3(0, 0, 0.1f), Quaternion.identity);
		obj.transform.SetParent(progressRoot.transform);
		progressObjs.Add(obj);
	}
}

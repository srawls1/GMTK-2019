using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : IComparable<PathNode>
{
	private Vector2 m_position;
	private int m_rowColCode;
	private NavTerrainTypes m_terrainType;

	public Vector2 position
	{
		get { return m_position; }
		private set { m_position = value; }
	}
	public int rowColCode
	{
		get { return m_rowColCode; }
		private set { m_rowColCode = value; }
	}
	public NavTerrainTypes terrainType
	{
		get { return m_terrainType; }
		private set { m_terrainType = value; }
	}

	public PathNode pathParent;
	public PathNode hashTableNext;
	public bool isClosed;
	public float knownCost;
	public float pathRemainderEstimate;

	public PathNode(Vector2 pos, int rcc, NavTerrainTypes terrain)
	{
		position = pos;
		rowColCode = rcc;
		terrainType = terrain;
		//Debug.DrawLine(position, position + 0.1f * Vector2.right, terrain == NavTerrainTypes.Floor ? Color.white : Color.red, 0.5f);
	}

	public int CompareTo(PathNode other)
	{
		float costDif = knownCost + pathRemainderEstimate - (other.knownCost + other.pathRemainderEstimate);
		if (costDif < 0f) return -1;
		if (costDif > 0f) return 1;
		return 0;
	}
}

public class NodePool
{
	private PathNode[] hashTable;
	private float xOffset;
	private float yOffset;
	private float scale;
	private float dist;
	private int width;
	private int height;

	public NodePool(float minX, float maxX, float minY, float maxY, float distBetweenNodes)
	{
		hashTable = new PathNode[127];
		xOffset = minX;
		yOffset = minY;
		dist = distBetweenNodes;
		scale = 1f / dist;
		width = Mathf.CeilToInt((maxX - minX) * scale);
		height = Mathf.CeilToInt((maxY - minY) * scale);
	}

	public PathNode GetAt(Vector2 position)
	{
		int normalizedX = Mathf.RoundToInt((position.x - xOffset) * scale);
		int normalizedY = Mathf.RoundToInt((position.y - yOffset) * scale);
		int rowColCode = normalizedY * width + normalizedX;

		//Debug.Log("rowColCode: " + rowColCode + ", rowColCode % length: " + rowColCode % hashTable.Length);
		PathNode node = hashTable[rowColCode % hashTable.Length];
		while (node != null && node.rowColCode != rowColCode)
		{
			node = node.hashTableNext;
		}

		if (node != null)
		{
			return node;
		}
		else
		{
			node = new PathNode(position, rowColCode, GetTerrainType(position));
			InsertIntoTable(node);
			return node;
		}
	}

	public IEnumerable<PathNode> GetAdjacentNodes(PathNode node)
	{
		if (node.rowColCode < 0)
		{
			int normalizedX = Mathf.FloorToInt((node.position.x - xOffset) * scale);
			int normalizedY = Mathf.FloorToInt((node.position.y - yOffset) * scale);

			float lowerX = normalizedX * dist + xOffset;
			float lowerY = normalizedY * dist + yOffset;
			float upperX = lowerX + dist;
			float upperY = lowerY + dist;

			yield return GetAt(new Vector2(lowerX, lowerY));
			yield return GetAt(new Vector2(lowerX, upperY));
			yield return GetAt(new Vector2(upperX, lowerY));
			yield return GetAt(new Vector2(upperX, upperY));
		}
		else
		{
			int normalizedX = node.rowColCode % width;
			int normalizedY = node.rowColCode / width;

			if (normalizedX > 0) yield return GetAt(new Vector2(node.position.x - dist, node.position.y));
			if (normalizedY > 0) yield return GetAt(new Vector2(node.position.x, node.position.y - dist));
			if (normalizedX < width) yield return GetAt(new Vector2(node.position.x + dist, node.position.y));
			if (normalizedY < height) yield return GetAt(new Vector2(node.position.x, node.position.y + dist));
			if (normalizedX > 0 && normalizedY > 0) yield return GetAt(new Vector2(node.position.x - dist, node.position.y - dist));
			if (normalizedX < width && normalizedY > 0) yield return GetAt(new Vector2(node.position.x + dist, node.position.y - dist));
			if (normalizedY > 0 && normalizedY < height) yield return GetAt(new Vector2(node.position.x - dist, node.position.y + dist));
			if (normalizedY < width && normalizedY < height) yield return GetAt(new Vector2(node.position.x + dist, node.position.y + dist));
		}
	}

	private Collider2D[] overlapCheckBuffer = new Collider2D[8];

	private NavTerrainTypes GetTerrainType(Vector2 point)
	{
		for (int i = 0; i < overlapCheckBuffer.Length; ++i)
		{
			overlapCheckBuffer[i] = null;
		}
		Physics2D.OverlapPointNonAlloc(point, overlapCheckBuffer);

		for (int i = 0; i < overlapCheckBuffer.Length && overlapCheckBuffer[i] != null; ++i)
		{
			NavObstacle obstacle = overlapCheckBuffer[i].GetComponent<NavObstacle>();
			if (obstacle != null)
			{
				return obstacle.terrainType;
			}
		}

		return NavTerrainTypes.Floor;
	}

	private void InsertIntoTable(PathNode node)
	{
		PathNode iter = hashTable[node.rowColCode % hashTable.Length];
		if (iter == null)
		{
			hashTable[node.rowColCode % hashTable.Length] = node;
		}
		else
		{
			while (iter.hashTableNext != null)
			{
				iter = iter.hashTableNext;
			}
			iter.hashTableNext = node;
		}
	}
}

using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NavTerrainTypes : int
{
	Floor = 1,
	Pit = 2,
	ThinWall = 4,
	ThickWall = 8,
	RatTunnel = 16,
	Door = 32
}

public class NavMesh : MonoBehaviour
{
	private static NavMesh m_instance;
	public static NavMesh instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = FindObjectOfType<NavMesh>();
			}

			return m_instance;
		}
	}

	void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}
		else if (m_instance != this)
		{
			Destroy(this);
		}
	}

	[SerializeField] private float minX;
	[SerializeField] private float maxX;
	[SerializeField] private float minY;
	[SerializeField] private float maxY;
	[SerializeField] private float distBetweenNodes;

	public List<Vector2> GetClosestPath(Vector2 start, Vector2 end, NavTerrainTypes linkTypeMask)
	{
		PathNode node = FindReversePath(start, end, linkTypeMask);
		List<Vector2> path = new List<Vector2>();
		while (node != null)
		{
			path.Add(node.position);
			node = node.pathParent;
		}
		
		return path;
	}

	private PathNode FindReversePath(Vector2 start, Vector2 end, NavTerrainTypes linkTypeMask)
	{
		NodePool pool = new NodePool(minX, maxX, minY, maxY, distBetweenNodes);
		MinHeap<PathNode> openNodes = new MinHeap<PathNode>();

		PathNode endNode = pool.GetAt(start);
		PathNode startNode = new PathNode(end, -1, NavTerrainTypes.Floor);
		startNode.pathRemainderEstimate = Vector2.Distance(start, end);
		openNodes.Add(startNode);


		while (openNodes.size > 0)
		{
			PathNode node = openNodes.Pop();
			if (node == endNode)
			{
				return endNode;
			}
			node.isClosed = true;

			foreach (PathNode adjacent in pool.GetAdjacentNodes(node))
			{
				if (adjacent.isClosed || (linkTypeMask & adjacent.terrainType) == 0)
				{
					continue;
				}

				float cost = node.knownCost + Vector2.Distance(node.position, adjacent.position);
				
				if (!openNodes.Contains(adjacent))
				{
					adjacent.knownCost = cost;
					adjacent.pathRemainderEstimate = Vector2.Distance(adjacent.position, endNode.position);
					openNodes.Add(adjacent);
					adjacent.pathParent = node;
				}
				else if (cost < adjacent.knownCost)
				{
					adjacent.knownCost = cost;
					adjacent.pathParent = node;
					openNodes.PriorityLowered(adjacent);
				}
			}
		}
		
		return endNode;
	}
}

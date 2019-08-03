using System;
using System.Collections.Generic;
using UnityEngine;

public class NavObstacle : MonoBehaviour
{
	[SerializeField] private NavTerrainTypes m_navTerrainType;

	public NavTerrainTypes terrainType
	{
		get { return m_navTerrainType; }
		set { m_navTerrainType = value; }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager
{

	private GameObject StraightRoadPrefab;
	private GameObject TurnRoadPrefab;
	private float size;


	public void Initialize(GameObject StraightRoadPrefab, GameObject TurnRoadPrefab)
	{
		this.StraightRoadPrefab = StraightRoadPrefab;
		this.TurnRoadPrefab = TurnRoadPrefab;
		size = StraightRoadPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
	}
	public float GetRoadSize()
	{
		return size;
	}
	public GameObject NextRoadDirection(Vector2 ResultantDirectionOfNeighbours)
	{
		GameObject temp = GetRoadType(ResultantDirectionOfNeighbours);
		temp.transform.rotation = GetTureTurnRoadDirection(ResultantDirectionOfNeighbours);

		return temp;
	}
	public Quaternion GetRoadDirection(Vector2 ResultantDirectionOfNeighbours, Vector2 Center, Vector2 Next)
	{
		if (ResultantDirectionOfNeighbours == Vector2.zero)
		{
			return GetTrueStraightRoadDirection(Center, Next);
		}
		return GetTureTurnRoadDirection(ResultantDirectionOfNeighbours);
	}
	public Quaternion GetTrueStraightRoadDirection(Vector2 Center, Vector2 Next)
	{
		if (Center.x == Next.x)
		{
			if (Center.y > Next.y)
				return Quaternion.Euler(0, 0, 180);
			else
				return Quaternion.Euler(0, 0, 0);
		}
		if (Center.x > Next.x)
			return Quaternion.Euler(0, 0, 90);
		else
			return Quaternion.Euler(0, 0, -90);
	}
	public Quaternion GetTureTurnRoadDirection(Vector2 ResultantDirectionOfNeighbours)
	{
		if (ResultantDirectionOfNeighbours == Vector2.zero)
		{
			Debug.Log("Resultant vector is zero, with no addtional paramters");
			return Quaternion.Euler(-1, -1, -1);
		}
		//0
		//0-0
		if (ResultantDirectionOfNeighbours == new Vector2(1, 1))
		{
			return Quaternion.Euler(0, 0, 90);
		}
		//0-0
		//0
		if (ResultantDirectionOfNeighbours == new Vector2(1, -1))
		{
			return Quaternion.Euler(0, 0, 0);
		}
		//0-0
		//  0
		if (ResultantDirectionOfNeighbours == new Vector2(-1, -1))
		{
			return Quaternion.Euler(0, 0, -90);
		}
		//  0
		//0-0
		if (ResultantDirectionOfNeighbours == new Vector2(-1, 1))
		{
			return Quaternion.Euler(0, 0, 180);
		}
		//0-0-0
		return Quaternion.Euler(-1, -1, -1);
	}
	public GameObject GetRoadType(Vector2 ResultantDirectionOfNeighbours)
	{
		if (ResultantDirectionOfNeighbours == new Vector2(0, 0))
			return MonoBehaviour.Instantiate(StraightRoadPrefab);
		return MonoBehaviour.Instantiate(TurnRoadPrefab);
	}


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionsBank : MonoBehaviour {

	[SerializeField] private Vector2[] positions2;
	[SerializeField] private Vector3[] positions3;
	[SerializeField] private bool OfAllChildren;


	private int amountOfChildren;
	void Start () {
		if (OfAllChildren)
		{
			amountOfChildren = gameObject.transform.childCount;
			positions3 = new Vector3[amountOfChildren];
			for(int i = 0; i < amountOfChildren; i++)
			{
				positions3[i] = gameObject.transform.GetChild(i).localPosition;				
			}
		}
	}

	public Vector3[] GetPositions3()
	{
		return positions3;
	}
	public Vector2[] GetPositions2()
	{
		return positions2;
	}

}

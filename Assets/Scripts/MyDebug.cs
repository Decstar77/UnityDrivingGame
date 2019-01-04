using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDebug : MonoBehaviour {

	[SerializeField] GameObject ActiveVert;
	[SerializeField] GameObject InActiveVert;

	VBuilds builds;

	void Start () {
		builds = new VBuilds();
		builds.Init();
		for (int i = 0; i < 5; i++)
		{
			builds.NextVert();
		}
		for(int i = 0; i < builds.GetStatusVertAmount(false); i++)
		{
			GameObject temp;
			VBuilds.Vertices vert = builds.GetVert(i);
			if (vert.active)
			{
				temp = Instantiate(ActiveVert);
			}
			else
			{
				temp = Instantiate(InActiveVert);
			}
			temp.transform.position = builds.GetVert(i).pos;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

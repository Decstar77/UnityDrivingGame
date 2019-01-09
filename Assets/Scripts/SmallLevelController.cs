using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallLevelController : MonoBehaviour {

	[SerializeField] private GameObject PlayerCar;
	[SerializeField] private GameObject StraightRoadPrefab;
	[SerializeField] private GameObject TurnRoadPrefab;
	[SerializeField] private GameObject StartRoad;
	[SerializeField] private GameObject EndRoad;
	[SerializeField] private GameObject Dirt;
	[SerializeField] private GameObject Grass;
	[SerializeField] private GameObject GrassDirt1;
	[SerializeField] private GameObject GrassDirt2;
	[SerializeField] private GameObject GrassDirt3;

	private VBuilds vBuilds;
	private RoadManager roadManager;
	private GameObject playerCar;
	private GameObject root;
	private GameObject tail;
	private float roadSize;
	private float pathLength;
	private bool Inited = false;

	private void SetUpCar()
	{
		playerCar = Instantiate(PlayerCar);
		playerCar.transform.position = new Vector3(root.transform.position.x, root.transform.position.y, -1);
		playerCar.transform.rotation = root.transform.rotation;

	}
	private void SetUpRootRoad(Vector2[] positions)
	{
		root = Instantiate(StartRoad);
		root.name = "Root";
		root.transform.position = positions[0] * roadSize;
		root.transform.rotation = roadManager.GetTrueStraightRoadDirection(positions[0], positions[1]);
	}
	private void SetUpTailRoad(Vector2[] positions)
	{
		int endIndex = positions.Length - 1;
		tail = Instantiate(EndRoad);
		tail.name = "Tail";
		tail.transform.position = positions[endIndex] * roadSize;
		tail.transform.rotation = roadManager.GetTrueStraightRoadDirection(positions[endIndex], positions[endIndex - 1]);
	}
	private void SetUpSingleTrack(Vector2[] positions)
	{
		for (int i = 1; i < positions.Length - 1; i++)
		{
			Vector2 Relative1 = vBuilds.GetRealativePosition(positions[i], positions[i - 1]);
			Vector2 Relative2 = vBuilds.GetRealativePosition(positions[i], positions[i + 1]);
			Vector2 ResultantDirection = Relative1 + Relative2;
			GameObject temp = roadManager.GetRoadType(ResultantDirection);
			temp.transform.rotation = roadManager.GetRoadDirection(ResultantDirection, positions[i], positions[i - 1]);
			temp.transform.position = positions[i] * roadSize;
			temp.transform.parent = root.transform;
		}
	}
	private void SetUpFiller(Vector2[] positions)
	{

	}
	private void SetUpGrass(Vector2[] positions)
	{
		for (int i = 0; i < positions.Length; i++)
		{
			GameObject temp = Instantiate(Grass);
			temp.transform.position = new Vector3(positions[i].x, positions[i].y, StraightRoadPrefab.transform.position.z + 1) * roadSize;
			temp.transform.parent = root.transform;
		}
	}
	private void SetUpDirt(Vector2[] positions)
	{
		bool changed = true;
		//Parse one
		while (changed)
		{
			changed = false;
			for (int i = 0; i < positions.Length; i++)
			{
				int activeAmount = vBuilds.GetVert(positions[i]).activeEdges;
				GameObject temp = null;
				switch (activeAmount)
				{
					case 3: temp = Instantiate(Grass); temp.name = "AdditionalGrass"; vBuilds.ActivateVert(positions[i], false, false); changed = true; break;
					case 4: temp = Instantiate(Grass); temp.name = "AdditionalGrass"; vBuilds.ActivateVert(positions[i], false, false); changed = true; break;
					default: break;
				}
				if (temp == null)
					continue;
				temp.transform.position = new Vector3(positions[i].x, positions[i].y, StraightRoadPrefab.transform.position.z + 2) * roadSize;
				temp.transform.parent = root.transform;
				if (changed)
					positions = vBuilds.GetPositionOfInActiveVerts();
				
			}
		}
		
		//Parse two
		List<Vector2> store = new List<Vector2>();
		for (int i = 0; i < positions.Length; i++)
		{
			int activeAmount = vBuilds.GetVert(positions[i]).activeEdges;
			GameObject temp = null;
			VBuilds.Vertices centerVert = new VBuilds.Vertices();
			VBuilds.Vertices[] adjacentVert = null;
			Vector2 dir = Vector2.zero;
			switch (activeAmount)
			{
				case 1:
					{
						centerVert = vBuilds.GetVert(positions[i]);
						adjacentVert = vBuilds.GetActiveAdjacentVerts(centerVert);

						for (int ii = 0; ii < adjacentVert.Length; ii++)
						{
							dir += vBuilds.GetRealativePosition(centerVert, adjacentVert[ii]);
						}
						temp = Instantiate(GrassDirt1);
						temp.transform.parent = root.transform;
						temp.transform.rotation = roadManager.GetTrueStraightRoadDirection(dir);
						break;
					}
				case 2:
					centerVert = vBuilds.GetVert(positions[i]);
					adjacentVert = vBuilds.GetActiveAdjacentVerts(centerVert);
					for (int ii = 0; ii < adjacentVert.Length; ii++)
					{
						dir += vBuilds.GetRealativePosition(centerVert, adjacentVert[ii]);
					}
					temp = Instantiate(GrassDirt2);					
					temp.transform.rotation = roadManager.GetTrueTurnRoadDirectionInverse(dir);
					break;
			}
			if (temp != null)
			{
				store.Add(positions[i]);
				temp.transform.position = new Vector3(positions[i].x, positions[i].y, StraightRoadPrefab.transform.position.z + 2) * roadSize;
			}

		}
		for (int i = 0; i < store.Count; i++)
		{
			
			vBuilds.ActivateVert(store[i], false, false);
		}
		positions = vBuilds.GetPositionOfInActiveVerts();
		
		//Parse Three
		for (int i = 0; i < positions.Length; i++)
		{
			GameObject temp = null;
			int activeAmount = vBuilds.GetVert(positions[i]).activeEdges;
			if (activeAmount == 2)
			{
				temp = Instantiate(GrassDirt3);				
				VBuilds.Vertices centerVert = vBuilds.GetVert(positions[i]);
				VBuilds.Vertices[] adjacentVert = vBuilds.GetActiveAdjacentVerts(centerVert);
				Vector2 dir = Vector2.zero;
				for (int ii = 0; ii < adjacentVert.Length; ii++)
				{
					dir += vBuilds.GetRealativePosition(centerVert, adjacentVert[ii]);
				}
				temp.transform.parent = root.transform;
				temp.transform.rotation = roadManager.GetTrueTurnRoadDirection(dir);
			}
			else
			{
				temp = Instantiate(Dirt);
				temp.transform.parent = root.transform;
			}
			if (temp != null)
				temp.transform.position = new Vector3(positions[i].x, positions[i].y, StraightRoadPrefab.transform.position.z + 2) * roadSize;
		}
		
	}


	public void Initialize(uint width, uint height, uint pathLength)
	{
		//Set up RoadManager and VBuilds
		root = null;
		tail = null;
		playerCar = null;
		if (PlayerPrefs.HasKey("seed"))
			print("Seed: " + PlayerPrefs.GetString("seed"));
		else
			print("no key");
		vBuilds = new VBuilds();
		vBuilds.Init(height, width, pathLength, PlayerPrefs.GetString("seed"));
		this.pathLength = pathLength;
		roadManager = new RoadManager();
		roadManager.Initialize(StraightRoadPrefab, TurnRoadPrefab);
		roadSize = roadManager.GetRoadSize();
		Inited = true;
	}
	public void CreateSmallLevel()
	{
		if (!Inited)
		{
			print("SmallLevelController has not been inited");
			return;
		}
		for (int i = 0; i < pathLength; i++)
		{
			vBuilds.NextVert();
		}
		Vector2[] positions = vBuilds.GetPositionOfPathVerts();
		SetUpRootRoad(positions);
		SetUpSingleTrack(positions);
		SetUpTailRoad(positions);
		SetUpCar();
		positions = vBuilds.GetPositionOfActiveVerts();
		SetUpGrass(positions);
		positions = vBuilds.GetPositionOfInActiveVerts();
		SetUpDirt(positions);



	}
	public void LoadResources()
	{
		PlayerCar = Resources.Load("SmallLevel/PlayerCarSmall") as GameObject;
		StraightRoadPrefab = Resources.Load("SmallLevel/TarRoad_SmallStraight_Resource") as GameObject;
		TurnRoadPrefab = Resources.Load("SmallLevel/TarRoad_SmallTurn_Resource") as GameObject;
		StartRoad = Resources.Load("SmallLevel/TarRoad_SmallStart_Resource") as GameObject;
		EndRoad = Resources.Load("SmallLevel/TarRoad_SmallEnd_Resource") as GameObject;
		Dirt = Resources.Load("SmallLevel/Land_SmallDirt") as GameObject;
		Grass = Resources.Load("SmallLevel/Land_SmallGrass") as GameObject;
		GrassDirt1 = Resources.Load("SmallLevel/Land_SmallGrassDirt1") as GameObject;
		GrassDirt2 = Resources.Load("SmallLevel/Land_SmallGrassDirt2") as GameObject;
		GrassDirt3 = Resources.Load("SmallLevel/Land_SmallGrassDirt3") as GameObject;
		if (PlayerCar == null || StraightRoadPrefab == null || TurnRoadPrefab == null || StartRoad == null || EndRoad == null || Dirt == null || Grass == null)
			print("Something didn't load ");
	}

	public GameObject GetPlayerCar()
	{
		return playerCar;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	[SerializeField] private GameObject PlayerCar;
	[SerializeField] private GameObject StraightRoadPrefab;
	[SerializeField] private GameObject TurnRoadPrefab;
	[SerializeField] private GameObject StartRoad;
	[SerializeField] private GameObject FinsihRoad;

	private CanvasController canvasController;
	private VBuilds vBuilds;
	private RoadManager roadManager;
	private GameObject playerCar;
	private GameObject root;
	private GameObject tail;
	private float roadSize;

	void Start() {
		root = null;
		tail = null;
		playerCar = null;
		if (PlayerPrefs.HasKey("seed"))
			print(PlayerPrefs.GetString("seed"));
		else
			print("no key");
		vBuilds = new VBuilds();
		vBuilds.Init(10, 10, 50, PlayerPrefs.GetString("seed"));

		roadManager = new RoadManager();
		roadManager.Initialize(StraightRoadPrefab, TurnRoadPrefab);
		roadSize = roadManager.GetRoadSize();

		for (int i = 0; i < 1000; i++)
		{
			vBuilds.NextVert();
		}
		Vector2[] positions = vBuilds.GetPositionOfPathVerts();
		SetUpRootRoad(positions);
		SetUpSingleTrack(positions);
		SetUpTailRoad(positions);
		SetUpCar();




		canvasController = FindObjectOfType<Canvas>().GetComponent<CanvasController>();
		if (canvasController == null) { print("Canvas is null"); }
		canvasController.SetGameManager(this);
	}

	private void CreateSmallTrack(int length)
	{

	}
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
		tail = Instantiate(FinsihRoad);
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
		}
	}


	public GameObject GetPlayerCar()
	{
		return playerCar;
	}
	public void AccelerateCar(bool isDown)
	{
		playerCar.GetComponent<CarController>().AccelerateFoward(isDown);
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


	private CanvasController canvasController;
	private SmallLevelController smallLevelController;

	void Start() {
		smallLevelController = gameObject.AddComponent<SmallLevelController>();
		smallLevelController.LoadResources();
		smallLevelController.Initialize(10, 10, 50);
		smallLevelController.CreateSmallLevel();




		canvasController = FindObjectOfType<Canvas>().GetComponent<CanvasController>();
		if (canvasController == null) { print("Canvas is null"); }
		canvasController.SetGameManager(this);
	}

	



	public GameObject GetPlayerCar()
	{
		if (smallLevelController != null)
			return smallLevelController.GetPlayerCar();
		return null;
	}


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour {

	private GameObject obj_fixedJoystick;
	private FixedJoystick scr_fixedJoystick;
	private GameManager gameManager;
	private CarController carController;

	void Start () {
		obj_fixedJoystick = transform.Find("Fixed Joystick").gameObject;
		if (obj_fixedJoystick == null)
		{
			print("Cannot find JoyStick");
			return;
		}
		scr_fixedJoystick = obj_fixedJoystick.GetComponent<FixedJoystick>();
	}

	void Update()
	{
		if (gameManager != null && carController != null)
		{
			carController.SetJoyStickDirection(scr_fixedJoystick.GetJoyStickDirection());
		}
	}

	public Vector3 GetJoyStickDirection()
	{
		return scr_fixedJoystick.GetJoyStickDirection();
	}
	public void SetGameManager(GameManager gameManager)
	{
		if (gameManager != null)
		{
			this.gameManager = gameManager;
			carController = gameManager.GetPlayerCar().GetComponent<CarController>();
		}
	}

}

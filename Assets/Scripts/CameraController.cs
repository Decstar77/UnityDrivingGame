using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField] private Transform target;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (target == null)
		{
			target = GetComponent<GameManager>().GetPlayerCar().transform;
		}
		else
		{
			transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
			//Quaternion rot = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, target.rotation.eulerAngles.z));
			//transform.rotation = rot;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] GameManager gameManager;
	private bool isDown;
	private float downTime;


	private void Start()
	{
		
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		this.isDown = true;
		this.downTime = Time.realtimeSinceStartup;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		this.isDown = false;
	}

	void Update()
	{ 
	}
}

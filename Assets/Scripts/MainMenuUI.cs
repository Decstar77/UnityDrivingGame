using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

	[SerializeField] private float transitionSpeed = 5f;
	[SerializeField] private Canvas[] canvases;
	private Canvas activeCanvas;
	private Canvas nextCanvas;
	private ArrayList childrenInActiveCanas;
	private bool moveCanvas = false;
	void Start()
	{	
		
		if (canvases.Length < 0)
			return;

		childrenInActiveCanas = new ArrayList();
		canvases[0].gameObject.SetActive(true);
		activeCanvas = canvases[0];
		GetChildrenOfActiveCanvas();
		for (int i = 1; i < canvases.Length; i++)
		{
			canvases[i].gameObject.SetActive(false);
		}

	}

	// Update is called once per frame
	void Update() {
		if (moveCanvas)
		{
			for (int i = 0; i < childrenInActiveCanas.Count; i++)
			{
				GameObject temp = ((Transform)childrenInActiveCanas[i]).gameObject;
				if (temp.tag == "Button")
				{
					temp.transform.position += Vector3.right * Time.deltaTime * transitionSpeed;
				}
				if (temp.tag == "Text")
				{
					temp.transform.position += Vector3.left * Time.deltaTime * transitionSpeed;
				}
			}
			if (!CheckBoundsOfChildren())
			{
				NextCanvas();
				moveCanvas = false;
			}
		}
	}
	private void GetChildrenOfActiveCanvas()
	{
		int childCount = activeCanvas.transform.childCount;
		childrenInActiveCanas.Clear();
		for (int i = 0; i < childCount; i++)
		{
			childrenInActiveCanas.Add(activeCanvas.transform.GetChild(i));
		}
	}
	private bool CheckBoundsOfChildren()
	{
		bool inBounds = true;
		for (int i = 0; i < childrenInActiveCanas.Count; i++)
		{
			GameObject temp = ((Transform)childrenInActiveCanas[i]).gameObject;
			if (temp.tag == "Untagged")
				continue;
			float canvasSize = activeCanvas.GetComponent<RectTransform>().sizeDelta.x;
			if (temp.transform.position.x < -canvasSize * 2)
			{
				inBounds = false;
				break;
			}
			if (temp.transform.position.x > canvasSize * 2)
			{
				inBounds = false;
				break;
			}
		}
		return inBounds;
	}
	private void NextCanvas()
	{
		activeCanvas.gameObject.SetActive(false);
		activeCanvas = nextCanvas;
		activeCanvas.gameObject.SetActive(true);
		GetChildrenOfActiveCanvas();
		//Optimize//
		SetCanvasDeufaults(activeCanvas);
		nextCanvas = null;
	}
	private void SetCanvasDeufaults(Canvas canvas)
	{
		PositionsBank bank = canvas.GetComponent<PositionsBank>();
		if (bank == null)
		{
			print("Error there are no positions for original canvas setup");
			return;
		}
		Vector3[] pos = bank.GetPositions3();
		if (pos.Length > canvas.transform.childCount)
		{
			print("Error: positions is greater that amount of children in the canvas");
			return;
		}
		for(int i = 0; i < pos.Length; i++)
		{
			GameObject temp = canvas.transform.GetChild(i).gameObject;
			temp.transform.localPosition = pos[i];
		}
	}
	public void MoveToNextCanvas(Canvas canvas)
	{
		moveCanvas = true;
		nextCanvas = canvas;
	}
	public void SetSeed(string seed)
	{
		PlayerPrefs.SetString("seed", seed);
		print(PlayerPrefs.GetString("seed"));
	}
	//public void MoveToNextScene(string scene)
	//{
	//	SceneManager.LoadScene(scene);
	//}
}

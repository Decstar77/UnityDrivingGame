using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InputFieldScript : MonoBehaviour {

	[SerializeField] private MainMenuUI mainMenuUI;
	InputField inputField;
	
	void Start () {
		inputField = GetComponent<InputField>();
		GameObject placeHolder = inputField.gameObject.transform.Find("Placeholder").gameObject;
		placeHolder.GetComponent<Text>().text = PlayerPrefs.GetString("seed");
	}
	
	
	void Update () {
		
	}


	public void ChangeSeed()
	{
		mainMenuUI.SetSeed(inputField.text);
	}
}

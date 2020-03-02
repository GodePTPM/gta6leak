using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour {

	private Image canvasFade;


	void Start()
	{
		canvasFade = GameObject.Find("fadeOut").GetComponent<Image>();
                canvasFade.CrossFadeAlpha(0f, 1.5f, false);
	}

	public void quitButton() {
		Application.Quit();
	}

	public void playButton() {
		SceneManager.LoadScene("testLevel", LoadSceneMode.Single);
	}

	void Update()
	{
	}
}

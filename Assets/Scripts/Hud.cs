using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud : MonoBehaviour {
	public Texture crossHair;

	void OnGUI() {
		GUI.DrawTexture(new Rect(Screen.width*0.475f, Screen.height*0.475f, Screen.width*0.05f, Screen.height*0.05f), crossHair);
	}
	void Start() {
	
	}

	void Update() {
	
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {
	public static NetworkManager networkManager = null;
	private Canvas canvas;

	// Set Dont Destory Network Window
	void Awake(){
		canvas = GetComponent<Canvas> ();

		// Create Dont Destroy On Load
		if (networkManager == null) {
			DontDestroyOnLoad (gameObject);
			networkManager = this;
		} else if (networkManager != this) {
			Destroy (gameObject);
		}
	}

	// Fixed Update
	void FixedUpdate(){
		if (canvas.worldCamera == null)
			canvas.worldCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
	}
}
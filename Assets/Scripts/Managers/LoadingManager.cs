using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour {
	public static LoadingManager loadingManager = null;
	private Canvas canvas;

	// Set Dont Destory Network Window
	void Awake(){
		canvas = GetComponent<Canvas> ();

		// Create Dont Destroy On Load
		if (loadingManager == null) {
			DontDestroyOnLoad (gameObject);
			loadingManager = this;
		} else if (loadingManager != this) {
			Destroy (gameObject);
		}
	}

	// Fixed Update
	void FixedUpdate(){
		if (canvas.worldCamera == null)
			canvas.worldCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
	}
}
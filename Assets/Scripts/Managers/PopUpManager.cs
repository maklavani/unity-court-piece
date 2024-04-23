using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour {
	private GameControl gameControl = null;
	public GameObject popup;
	public GameObject popupWindow;
	[HideInInspector]
	public List<PopUpData> windows;
	[HideInInspector]
	public string message;

	// Animation
	private bool hidePopupWindow = false;
	private bool showPopupWindow = false;
	private Vector3 topPosition = new Vector3 (0 , 120 , 0);
	private Vector3 middlePosition = new Vector3 (0 , 0 , 0);
	private Vector3 bottomPosition = new Vector3 (0 , -120 , 0);

	// Fixed Update
	void FixedUpdate(){
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();

		if (showPopupWindow || hidePopupWindow) {
			if (showPopupWindow && !V3Equal (popupWindow.transform.position, middlePosition)) {
				popupWindow.transform.position = Vector3.MoveTowards (popupWindow.transform.position, middlePosition, 250 * Time.deltaTime);
			} else if (hidePopupWindow && !V3Equal (popupWindow.transform.position, bottomPosition)) {
				popupWindow.transform.position = Vector3.MoveTowards (popupWindow.transform.position, bottomPosition, 500 * Time.deltaTime);
			}

			if (hidePopupWindow && V3Equal (popupWindow.transform.position, bottomPosition)) {
				popup.SetActive (false);
				hidePopupWindow = false;
			}
		}
	}

	// V3Equal
	public bool V3Equal(Vector3 a , Vector3 b){
		return Vector3.SqrMagnitude (a - b) < 0.0001;
	}

	// Show PopUp
	public void ShowPopUp(string state , string messageInput = ""){
		if (windows.Count > 0) {
			popup.SetActive (true);

			foreach (var window in windows)
				window.go.gameObject.SetActive (false);
	
			foreach (var window in windows)
				if (window.name == state) {
					if (messageInput != "")
						message = messageInput;

					popupWindow.transform.Find (window.name).gameObject.SetActive (true);
					popupWindow.transform.position = topPosition;

					showPopupWindow = true;
					hidePopupWindow = false;
					gameControl.translateLanguage = true;
					window.function.Invoke ();
				}
		}
	}

	// Close PopUp
	public void ClosePopUp(){
		showPopupWindow = false;
		hidePopupWindow = true;
	}
}
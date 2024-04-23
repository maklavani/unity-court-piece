using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileWindow : GenericWindow {
	public RequestManager requestManager;
	private GameControl gameControl = null;

	public ToggleGroup face;

	public override void Open () {
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();

		face.transform.Find (gameControl.face.ToString ()).GetComponent<Toggle> ().isOn = true;
	}

	// Change Face
	public void ChangeFace(){
		var actives = face.ActiveToggles();

		foreach (Toggle active in actives) {
			gameControl.face = int.Parse (active.name);
			requestManager.EnableRequest ("Send Face");
		}
	}

	// Send Face
	public void SendFace() {
		WWWForm data = new WWWForm();
		data.AddField ("face", gameControl.face);
		requestManager.SendData ("sendface", data, CheckSendFace, "Send Face");
	}

	// Check Send Face
	public void CheckSendFace() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True") {
			requestManager.DisableRequest ("Send Face");
			gameControl.Save ();
		}
	}
}
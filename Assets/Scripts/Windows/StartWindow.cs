using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartWindow : GenericWindow {
	public RequestManager requestManager;
	public PopUpManager popUpManager;
	private GameControl gameControl = null;

	public GameObject startWindow;
	public ToggleGroup face;
	public InputField username;
	public InputField password;
	public InputField reagent;

	// Open
	public override void Open () {
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();

		ShowProfile ();
	}

	// Update
	void Update() {
		if (username.text == "_NEW" && gameControl.username != "_NEW") {
			username.text = gameControl.username;
			password.text = gameControl.password;
		}
	}

	// Show Profile
	public void ShowProfile()
	{
		if (gameControl.changeUsername == false) {
			startWindow.transform.Find ("WindowUsername").gameObject.SetActive (true);
			startWindow.transform.Find ("WindowProfile").gameObject.SetActive (false);
			// username.text = gameControl.username;
			// password.text = gameControl.password;
		} else {
			startWindow.transform.Find ("WindowUsername").gameObject.SetActive (false);
			startWindow.transform.Find ("WindowProfile").gameObject.SetActive (true);
			face.transform.Find (gameControl.face.ToString ()).GetComponent<Toggle> ().isOn = true;
		}
	}

	// Change Face
	public void ChangeFace(){
		var actives = face.ActiveToggles();

		foreach (Toggle active in actives) {
			gameControl.face = int.Parse (active.name);
			gameControl.Save ();
			requestManager.EnableRequest ("Send Face Start");
		}
	}

	// Username
	public void Username(){
		if (gameControl.username != username.text || gameControl.password != password.text) {
			requestManager.EnableRequest ("Change Username Reagent");
		} else {
			popUpManager.message = "_CHANGE_USERNAME_OR_PASSWORD";
			popUpManager.ShowPopUp("Message");
		}
	}

	// Finish
	public void Finish(){
		gameControl.changeProfile = true;
		gameControl.Save ();
		manager.Open ((int)Windows.Home);
	}

	// Send Face
	public void SendFace() {
		WWWForm data = new WWWForm();
		data.AddField ("face", gameControl.face);
		requestManager.SendData ("sendface", data, CheckSendFace, "Send Face Start");
	}

	// Check Send Face
	public void CheckSendFace() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True")
			requestManager.DisableRequest ("Send Face Start");
	}

	// Change Username
	public void ChangeUsername() {
		WWWForm data = new WWWForm();
		data.AddField ("usernameChange", username.text);
		data.AddField ("passwordChange", password.text);
		data.AddField ("reagentChange", reagent.text);
		requestManager.SendData ("changeusername", data, CheckChangeUsername, "Change Username Reagent");
	}

	// Check Change Username
	public void CheckChangeUsername() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True" && result.ContainsKey ("username") && result.ContainsKey ("password")) {
			gameControl.username = result ["username"];
			gameControl.password = result ["password"];
			gameControl.changeUsername = true;
			gameControl.Save ();
			requestManager.DisableRequest ("Change Username Reagent");

			manager.Open ((int)Windows.Home);
		} else if (result.ContainsKey ("error")) {
			if (result ["error"] == "Choose Another Username.") {
				popUpManager.message = "_CHOOSE_USERNAME";
				popUpManager.ShowPopUp("Message");
				requestManager.DisableRequest ("Change Username");
			}

			if (result ["error"] == "Username or Password is empty.") {
				popUpManager.message = "_EMPTY_USERNAME_OR_PASSWORD";
				popUpManager.ShowPopUp("Message");
				requestManager.DisableRequest ("Change Username Reagent");
			}
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingWindow : GenericWindow {
	public LanguageManager languageManager;
	public RequestManager requestManager;
	public PopUpManager popUpManager;
	public HomeWindow homeWindow;
	private GameControl gameControl = null;

	public Slider music;
	public Slider sfx;
	public ToggleGroup language;
	public InputField username;
	public InputField password;
	public InputField mobile;
	public Button vertification;

	[HideInInspector]
	public string ScreenshotName = ""; // screenshot.png

	public override void Open () {
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();

		UpdateInformation ();
	}

	// Update Information
	public void UpdateInformation(){
		music.value = gameControl.music;
		sfx.value = gameControl.sfx;
		language.transform.Find (gameControl.language.ToString ()).GetComponent<Toggle> ().isOn = true;
		username.text = gameControl.username;
		password.text = gameControl.password;
		mobile.text = gameControl.mobile;
		SetSiblings ();

		if (gameControl.signUp) {
			vertification.GetComponent<Button> ().interactable = false;
			mobile.GetComponent<InputField> ().interactable = false;
		} else {
			vertification.GetComponent<Button> ().interactable = true;
			mobile.GetComponent<InputField> ().interactable = true;
		}
	}

	// Change Music
	public void ChangeMusic(){
		gameControl.music = music.value;
		gameControl.Save ();
	}

	// Change SFX
	public void ChangeSFX(){
		gameControl.sfx = sfx.value;
		gameControl.Save ();
	}

	// Change Language
	public void ChangeLanguage(){
		var actives = language.ActiveToggles();

		foreach (Toggle active in actives) {
			gameControl.language = (Languages)(System.Enum.Parse (typeof(Languages), active.name));
			gameControl.Save ();
			gameControl.translateLanguageAsNew = true;
			gameControl.translateLanguage = true;
			SetSiblings ();
		}
	}

	// Login
	public void Login(){
		gameControl.showLoginMessage = true;
		gameControl.previosUsername = gameControl.username;
		gameControl.previosPassword = gameControl.password;
		gameControl.username = username.text;
		gameControl.password = password.text;
		gameControl.Save ();
		requestManager.EnableRequest ("Check Login");
	}

	// Username
	public void Username(){
		if (gameControl.username != username.text || gameControl.password != password.text) {
			requestManager.EnableRequest ("Change Username");
		} else {
			popUpManager.message = "_CHANGE_USERNAME_OR_PASSWORD";
			popUpManager.ShowPopUp("Message");
		}
	}

	// Vertification
	public void Vertification(){
		if (mobile.text != "") {
			if (gameControl.mobile != mobile.text) {
				gameControl.mobile = mobile.text;
				gameControl.Save ();
				requestManager.EnableRequest ("Send SMS");
			}

			popUpManager.ShowPopUp ("Mobile");
		}
	}

	// Confirm
	public void Confirm(){
		requestManager.EnableRequest ("Confirm SMS");
	}

	// Set Siblings
	public void SetSiblings(){
		if (gameControl.language == Languages.Persian) {
			music.transform.SetSiblingIndex (0);
			sfx.transform.SetSiblingIndex (0);
			language.transform.SetSiblingIndex (0);

			music.transform.parent.Find ("Text").GetComponent<Text> ().alignment = TextAnchor.MiddleRight;
			sfx.transform.parent.Find ("Text").GetComponent<Text> ().alignment = TextAnchor.MiddleRight;
			language.transform.parent.Find ("Text").GetComponent<Text> ().alignment = TextAnchor.MiddleRight;
		} else {
			music.transform.SetSiblingIndex (1);
			sfx.transform.SetSiblingIndex (1);
			language.transform.SetSiblingIndex (1);

			music.transform.parent.Find ("Text").GetComponent<Text> ().alignment = TextAnchor.MiddleLeft;
			sfx.transform.parent.Find ("Text").GetComponent<Text> ().alignment = TextAnchor.MiddleLeft;
			language.transform.parent.Find ("Text").GetComponent<Text> ().alignment = TextAnchor.MiddleLeft;
		}
	}

	// Share
	public void Share(){
		//string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
		//if (File.Exists (screenShotPath))
		//	File.Delete (screenShotPath);

		//Application.CaptureScreenshot (ScreenshotName);

		//StartCoroutine (DelayedShare (screenShotPath, "Test"));
		NativeShare.Share (languageManager.GetTextConverted ("_SHARE_MESSAGE") + "\n" + languageManager.GetTextConverted ("_SHARE_REAGENT") + " " + gameControl.reagent + "\nhttp://finalvistor.com");
	}

	// Change Username
	public void ChangeUsername() {
		WWWForm data = new WWWForm();
		data.AddField ("usernameChange", username.text);
		data.AddField ("passwordChange", password.text);
		requestManager.SendData ("changeusername", data, CheckChangeUsername, "Change Username");
	}

	// Check Change Username
	public void CheckChangeUsername() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True" && result.ContainsKey ("username") && result.ContainsKey ("password")) {
			gameControl.username = result ["username"];
			gameControl.password = result ["password"];
			gameControl.changeUsername = true;
			gameControl.Save ();
			requestManager.DisableRequest ("Change Username");

			popUpManager.message = "_CHANGED_USERNAME_OR_PASSWORD";
			popUpManager.ShowPopUp("Message");
		} else if (result.ContainsKey ("error")) {
			if (result ["error"] == "Choose Another Username.") {
				popUpManager.message = "_CHOOSE_USERNAME";
				popUpManager.ShowPopUp("Message");
				requestManager.DisableRequest ("Change Username");
			}

			if (result ["error"] == "Username or Password is empty.") {
				popUpManager.message = "_EMPTY_USERNAME_OR_PASSWORD";
				popUpManager.ShowPopUp("Message");
				requestManager.DisableRequest ("Change Username");
			}
		}
	}

	// Send SMS
	public void SendSMS() {
		WWWForm data = new WWWForm();
		data.AddField ("mobile", gameControl.mobile);
		requestManager.SendData ("sendsms", data, CheckSendSMS, "Send SMS");
	}

	// Check Send SMS
	public void CheckSendSMS() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True") {
			requestManager.DisableRequest ("Send SMS");
		} else if (result.ContainsKey ("error")) {
			requestManager.DisableRequest ("Send SMS");
		}
	}

	// Confirm SMS
	public void ConfirmSMS() {
		var confirm = GameObject.Find ("PopUpWindow").transform.Find ("Mobile").Find ("ConfirmInput").GetComponent<InputField> ();
		WWWForm data = new WWWForm();
		data.AddField ("confirm", confirm.text);
		requestManager.SendData ("confirmsms", data, CheckConfirmSMS, "Confirm SMS");
	}

	// Check Confirm SMS
	public void CheckConfirmSMS() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True" && result.ContainsKey ("coin")) {
			gameControl.coin = int.Parse (result ["coin"]);
			gameControl.signUp = true;
			gameControl.Save ();

			popUpManager.message = "_SIGN_UPS";
			popUpManager.ShowPopUp("Message");
			requestManager.DisableRequest ("Confirm SMS");

			UpdateInformation ();
			homeWindow.UpdateInformation ();
			homeWindow.UpdateInformationOxygen ();
		} else if (result.ContainsKey ("error")) {
			requestManager.DisableRequest ("Confirm SMS");
		}
	}

	// Delayed Share
	IEnumerator DelayedShare(string screenShotPath, string text){
		while (!File.Exists (screenShotPath))
			yield return new WaitForSeconds (.05f);

		NativeShare.Share (text, screenShotPath, "", "", "image/png", true, "");
	}
}
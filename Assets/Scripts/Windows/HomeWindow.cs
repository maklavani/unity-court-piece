using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeWindow : GenericWindow {
	public RequestManager requestManager;
	public PopUpManager popUpManager;
	private GameControl gameControl = null;
	private AnimationManager animationManager;
	private SoundsManager soundsManager;

	public Text usernameObject;
	public Text levelObject;
	public Slider levelDegreeObject;
	public Text goldCoinObject;
	public Text coinObject;
	public Image faceObject;
	public GameObject oxygensObject;

	// Open
	public override void Open (){
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null) {
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();
			soundsManager = gameControl.GetComponent<SoundsManager> ();
			animationManager = gameControl.GetComponent<AnimationManager> ();
		}

		if (soundsManager.GetStatus ("Main") != true) {
			soundsManager.DisableAllSounds ();
			soundsManager.EnableSound ("Main", true);
		}

		requestManager.EnableRequest ("Check Login");
		requestManager.EnableRequest ("Get Oxygen");

		if (gameControl.changeUsername == false || gameControl.changeProfile == false)
			manager.Open ((int)Windows.Start);
	}

	public void UpdateInformation(){
		usernameObject.text = gameControl.username;
		levelObject.text = gameControl.level.ToString("C0").Replace("$" , "");
		levelDegreeObject.value = gameControl.levelDegree;
		goldCoinObject.text = gameControl.goldCoin.ToString("C0").Replace("$" , "");
		coinObject.text = gameControl.coin.ToString("C0").Replace("$" , "");
		faceObject.sprite = animationManager.faceImages [gameControl.face - 1];

		gameControl.translateLanguage = true;
	}

	public void UpdateInformationOxygen(){
		// Oxygen
		if (gameControl.oxygen < 3) {
			oxygensObject.transform.Find ("OxygenLineIconC").gameObject.SetActive (true);
			oxygensObject.transform.Find ("OxygenIconC").gameObject.SetActive (false);
		} else {
			oxygensObject.transform.Find ("OxygenLineIconC").gameObject.SetActive (false);
			oxygensObject.transform.Find ("OxygenIconC").gameObject.SetActive (true);
		}

		if (gameControl.oxygen < 2) {
			oxygensObject.transform.Find ("OxygenLineIconB").gameObject.SetActive (true);
			oxygensObject.transform.Find ("OxygenIconB").gameObject.SetActive (false);
		} else {
			oxygensObject.transform.Find ("OxygenLineIconB").gameObject.SetActive (false);
			oxygensObject.transform.Find ("OxygenIconB").gameObject.SetActive (true);
		}

		if (gameControl.oxygen < 1) {
			oxygensObject.transform.Find ("OxygenLineIconA").gameObject.SetActive (true);
			oxygensObject.transform.Find ("OxygenIconA").gameObject.SetActive (false);
		} else {
			oxygensObject.transform.Find ("OxygenLineIconA").gameObject.SetActive (false);
			oxygensObject.transform.Find ("OxygenIconA").gameObject.SetActive (true);
		}
	}

	// Check Login
	public void CheckLogin(){
		WWWForm data = new WWWForm();
		data.AddField("market" , gameControl.market);
		data.AddField("version" , gameControl.version);

		if (gameControl.previosUsername != "" && gameControl.previosPassword != "") {
			data.AddField("previosUsername" , gameControl.previosUsername);
			data.AddField("previosPassword" , gameControl.previosPassword);
		}
			
		requestManager.SendData ("login", data , SetSignUp , "Check Login");
	}

	// Set Sign Up
	public void SetSignUp (){
		Dictionary<string,string> result = requestManager.result;

		// Sign UP
		if (result ["statusRequest"] == "True" && result.ContainsKey ("username") && result.ContainsKey ("password")) {
			gameControl.username = result ["username"];
			gameControl.password = result ["password"];
		} else if (result.ContainsKey ("error")) {
			if (result ["error"] == "Username or Password is incorrect.") {
				// Open PopUp Error
				popUpManager.message = "_USERNAME_OR_PASSWORD_INCORRECT";
				popUpManager.ShowPopUp("Message");

				gameControl.username = "_NEW";
				gameControl.password = "_NEW";
				gameControl.Save ();
				requestManager.EnableRequest ("Check Login");
			}

			if (gameControl.previosUsername != "" && gameControl.previosPassword != "") {
				gameControl.username = gameControl.previosUsername;
				gameControl.password = gameControl.previosPassword;
				gameControl.Save ();
				gameControl.previosUsername = "";
				gameControl.previosPassword = "";
			}

			if (gameControl.games == 0) {
				gameControl.username = "_NEW";
				gameControl.password = "_NEW";
				gameControl.Save ();
				requestManager.EnableRequest ("Check Login");
			}
		}

		if (result.ContainsKey ("alias"))
			gameControl.alias = result ["alias"];
		if (result.ContainsKey ("reagent"))
			gameControl.reagent = result ["reagent"];
		if (result.ContainsKey ("mobile") && gameControl.mobile == "")
			gameControl.mobile = result ["mobile"];
		if (result.ContainsKey ("level"))
			gameControl.level = int.Parse(result ["level"]);
		if (result.ContainsKey ("levelDegree"))
			gameControl.levelDegree = float.Parse(result ["levelDegree"]);
		if (result.ContainsKey ("goldCoin"))
			gameControl.goldCoin = int.Parse(result ["goldCoin"]);
		if (result.ContainsKey ("coin"))
			gameControl.coin = int.Parse (result ["coin"]);
		if (result.ContainsKey ("games"))
			gameControl.games = int.Parse (result ["games"]);
		if (result.ContainsKey ("face"))
			gameControl.face = int.Parse (result ["face"]);
		if (result.ContainsKey ("win"))
			gameControl.win = int.Parse (result ["win"]);
		if (result.ContainsKey ("lose"))
			gameControl.lose = int.Parse (result ["lose"]);
		if (result.ContainsKey ("friends"))
			gameControl.friends = result ["friends"];
		if (result.ContainsKey ("messages"))
			gameControl.messages = result ["messages"];
		if (result.ContainsKey ("signUp"))
			gameControl.signUp = bool.Parse (result ["signUp"]);

		// Save
		if (result ["statusRequest"] == "True" && !result.ContainsKey ("error")) {
			if (gameControl.showLoginMessage) {
				popUpManager.message = "_LOGGED";
				popUpManager.ShowPopUp("Message");
				gameControl.showLoginMessage = false;
			}
	
			gameControl.Save ();
			UpdateInformation ();
		}
	}

	// Get Oxygen
	public void GetOxygen() {
		WWWForm data = new WWWForm();
		requestManager.SendData ("getoxygen", data , CheckGetOxygen , "Get Oxygen");
	}

	// Check Get Oxygen
	public void CheckGetOxygen() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True" && result.ContainsKey ("oxygen") && result.ContainsKey ("oxygenStatus") && result.ContainsKey ("oxygenTime")) {
			gameControl.oxygen = int.Parse (result ["oxygen"]);
			gameControl.oxygenStatus = int.Parse (result ["oxygenStatus"]);
			gameControl.oxygenTime = int.Parse (result ["oxygenTime"]);
			requestManager.DisableRequest ("Get Oxygen");

			UpdateInformationOxygen ();
		} else if (result.ContainsKey ("error") && result ["error"] == "Your request was not accepted.") {
			requestManager.DisableRequest ("Get Oxygen");
		}
	}

	// Charity Open
	public void CharityOpen(){
		popUpManager.ShowPopUp ("Charity");
	}
}
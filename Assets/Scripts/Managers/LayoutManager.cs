using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutManager : MonoBehaviour {
	[HideInInspector]
	public GameControl gameControl = null;
	public GameObject WindowManager;

	private WindowManager manager;
	private PopUpManager popUpManager;

	void Awake() {
		manager = WindowManager.GetComponent<WindowManager> ();
		popUpManager = GetComponent<PopUpManager> ();
	}

	// Fixed Update
	void FixedUpdate(){
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();
	}

	// Open Home
	public void OpenHome(){
		manager.Open ((int)Windows.Home);
	}

	// Open Store
	public void OpenStore(){
		manager.Open ((int)Windows.Store);
	}

	// Open Setting
	public void OpenSetting(){
		manager.Open ((int)Windows.Setting);
	}

	// Open Ranking
	public void OpenRanking(){
		manager.Open ((int)Windows.Ranking);
	}

	// Open Friends
	public void OpenFriends(){
		manager.Open ((int)Windows.Friends);
	}

	// Open New Game
	public void OpenNewGame(){
		if (gameControl.oxygen > 0)
			manager.Open ((int)Windows.NewGame);
		else {
			popUpManager.ShowPopUp ("Oxygen");
		}
	}

	// Open TV
	public void OpenTV(){
		manager.Open ((int)Windows.TV);
	}

	// Open Profile
	public void OpenProfile(){
		manager.Open ((int)Windows.Profile);
	}

	// Open Tournament
	public void OpenTournament(){
		//manager.Open ((int)Windows.Tournament - 1);
		popUpManager.message = "Level must be greater than 3";
		popUpManager.ShowPopUp ("Message");
	}

	// Open Final Victor
	public void OpenFinalVictor(){
		//manager.Open ((int)Windows.FinalVictor - 1);
		popUpManager.message = "Level must be greater than 3";
		popUpManager.ShowPopUp ("Message");
	}

	// Open Friend
	public void OpenFriend(){
		popUpManager.message = "Level must be greater than 3";
		popUpManager.ShowPopUp ("Message");
	}

	// Open Offline
	public void OpenOffline(){
		//manager.Open ((int)Windows.Offline - 1);
		popUpManager.message = "Level must be greater than 3";
		popUpManager.ShowPopUp ("Message");
	}

	// OxygenPopup
	public void OxygenPopup(){
		var oxygen = GameObject.Find ("PopUpWindow").transform.Find ("Oxygen").Find ("Buttons").Find ("Coin");

		if (gameControl.coin >= 250)
			oxygen.GetComponent<Button> ().interactable = true;
		else
			oxygen.GetComponent<Button> ().interactable = false;
	}

	// Mobile Popup
	public void MobilePopup() {
	}

	// Charity Popup
	public void CharityPopup() {
		List<GameObject> objs = new List<GameObject> ();
		objs.Add (GameObject.Find ("Charity").transform.Find ("Buttons").Find ("1").gameObject);
		objs.Add (GameObject.Find ("Charity").transform.Find ("Buttons").Find ("2").gameObject);
		objs.Add (GameObject.Find ("Charity").transform.Find ("Buttons").Find ("3").gameObject);

		if(objs.Count > 0)
			foreach(var obj in objs){
				var pm = obj.GetComponent<ProductManager> ();

				if (pm.coin <= gameControl.coin)
					pm.GetComponent<Button> ().interactable = true;
				else
					pm.GetComponent<Button> ().interactable = false;
			}
	}

	// Message Popup
	public void MessagePopup() {
		var text = GameObject.Find ("PopUpWindow").transform.Find ("Message").Find ("Text");
		text.GetComponent<Text> ().text = popUpManager.message;
	}
}
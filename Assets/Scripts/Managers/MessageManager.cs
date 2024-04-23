using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;

public class MessageManager : MonoBehaviour {
	private GameManager gameManager;

	public GameObject messagesObject;
	private bool showStatusA = false;
	private bool showStatusB = false;
	private bool showStatusC = false;
	private bool showStatusD = false;
	private int messageA = 0;
	private int messageB = 0;
	private int messageC = 0;
	private int messageD = 0;
	private float delayA = 0f;
	private float delayB = 0f;
	private float delayC = 0f;
	private float delayD = 0f;
	private int stepA = 0;
	private int stepB = 0;
	private int stepC = 0;
	private int stepD = 0;
	private List<int> messages;

	// Awake
	void Awake(){
		gameManager = GetComponent<GameManager> ();
	}

	// Update
	void Update(){
		if (showStatusA) {
			delayA += Time.deltaTime;

			if (stepA == 0) {
				gameManager.users [0].transform.Find("Dialog").gameObject.SetActive (false);
				stepA++;
			}

			if (stepA == 1 && delayA > 0.2f) {
				gameManager.users [0].transform.Find ("Dialog").gameObject.SetActive (true);
				gameManager.users [0].transform.Find ("Dialog").Find ("ImageParent").gameObject.SetActive (false);
				gameManager.users [0].transform.Find ("Dialog").Find ("Text").gameObject.SetActive (false);
				SetStepShow (gameManager.users [0] , messageA);
				stepA++;
			}

			if (stepA == 2 && delayA > 5.2f) {
				gameManager.users [0].transform.Find("Dialog").gameObject.SetActive (false);
				showStatusA = false;
			}
		} else if (showStatusB) {
			delayB += Time.deltaTime;

			if (stepB == 0) {
				gameManager.users [1].transform.Find("Dialog").gameObject.SetActive (false);
				stepB++;
			}

			if (stepB == 1 && delayB > 0.2f) {
				gameManager.users [1].transform.Find ("Dialog").gameObject.SetActive (true);
				gameManager.users [1].transform.Find ("Dialog").Find ("ImageParent").gameObject.SetActive (false);
				gameManager.users [1].transform.Find ("Dialog").Find ("Text").gameObject.SetActive (false);
				SetStepShow (gameManager.users [1] , messageB);
				stepB++;
			}

			if (stepB == 2 && delayB > 5.2f) {
				gameManager.users [1].transform.Find("Dialog").gameObject.SetActive (false);
				showStatusB = false;
			}
		} else if (showStatusC) {
			delayC += Time.deltaTime;

			if (stepC == 0) {
				gameManager.users [2].transform.Find("Dialog").gameObject.SetActive (false);
				stepC++;
			}

			if (stepC == 1 && delayC > 0.2f) {
				gameManager.users [2].transform.Find ("Dialog").gameObject.SetActive (true);
				gameManager.users [2].transform.Find ("Dialog").Find ("ImageParent").gameObject.SetActive (false);
				gameManager.users [2].transform.Find ("Dialog").Find ("Text").gameObject.SetActive (false);
				SetStepShow (gameManager.users [2] , messageC);
				stepC++;
			}

			if (stepC == 2 && delayC > 5.2f) {
				gameManager.users [2].transform.Find("Dialog").gameObject.SetActive (false);
				showStatusC = false;
			}
		} else if (showStatusD) {
			delayD += Time.deltaTime;

			if (stepD == 0) {
				gameManager.users [3].transform.Find("Dialog").gameObject.SetActive (false);
				stepD++;
			}

			if (stepD == 1 && delayD > 0.2f) {
				gameManager.users [3].transform.Find ("Dialog").gameObject.SetActive (true);
				gameManager.users [3].transform.Find ("Dialog").Find ("ImageParent").gameObject.SetActive (false);
				gameManager.users [3].transform.Find ("Dialog").Find ("Text").gameObject.SetActive (false);
				SetStepShow (gameManager.users [3] , messageD);
				stepD++;
			}

			if (stepD == 2 && delayD > 5.2f) {
				gameManager.users [3].transform.Find("Dialog").gameObject.SetActive (false);
				showStatusD = false;
			}
		}
	}

	// Get Item Name
	public void SetStepShow(GameObject go , int m){
		if (m < 10) {
			string item = "Item";

			if (m < 4)
				item += "123";
			else if (m < 7)
				item += "456";
			else
				item += "789";

			go.transform.Find ("Dialog").Find ("ImageParent").gameObject.SetActive (true);
			go.transform.Find ("Dialog").Find ("ImageParent").Find ("Image").GetComponent<Image> ().sprite = messagesObject.transform.Find ("Messages").Find ("Content").Find (item).Find (m.ToString ()).Find ("Image").GetComponent<Image> ().sprite;
		} else {
			go.transform.Find ("Dialog").Find ("Text").gameObject.SetActive (true);
			go.transform.Find ("Dialog").Find ("Text").GetComponent<Text> ().text = "_MESSAGE_" + (m - 9).ToString ();
			gameManager.gameControl.translateLanguage = true;
		}
	}

	// On Click
	public void OnClick(Button btn){
		gameManager.message = int.Parse (btn.gameObject.name);
		messagesObject.SetActive (false);
	}

	// Open Messages
	public void OpenMessages(){
		messagesObject.SetActive (true);
		gameManager.gameControl.translateLanguage = true;

		// Set Visible
		messages = ReadJSONCardsMessages (gameManager.gameControl.messages);

		for (var i = 17; i < 29; i++){
			Transform item = messagesObject.transform.Find ("Messages").Find ("Content").Find ("Item" + i);

			if (messages.Contains (i - 9))
				item.Find(i.ToString()).GetComponent<Button> ().interactable = true;
			else
				item.Find(i.ToString()).GetComponent<Button> ().interactable = false;
		}
	}

	// Close Messages
	public void CloseMessages(){
		messagesObject.SetActive (false);
	}

	// Show Message
	public void ShowMessage(string position , int messageIn) {
		if (position == "Bottom") {
			showStatusA = true;
			messageA = messageIn;
			delayA = 0f;
			stepA = 0;
		} else if (position == "Right") {
			showStatusB = true;
			messageB = messageIn;
			delayB = 0f;
			stepB = 0;
		} else if (position == "Top") {
			showStatusC = true;
			messageC = messageIn;
			delayC = 0f;
			stepC = 0;
		} else if (position == "Left") {
			showStatusD = true;
			messageD = messageIn;
			delayD = 0f;
			stepD = 0;
		}
	}

	// Read JSON Messages
	public List<int> ReadJSONCardsMessages(string input)
	{
		var json = (Dictionary<string, object>)Json.Deserialize (input);
		List<int> list = new List<int> ();

		if (json.Count > 0)
			foreach (object jsonItem in json.Keys) {
				var key = jsonItem.ToString ();
				list.Add (int.Parse (json [key].ToString ()));
			}

		return list;
	}
}
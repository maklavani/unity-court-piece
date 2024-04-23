using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;

public class StoreWindow : GenericWindow {
	public RequestManager requestManager;
	public PopUpManager popUpManager;
	public HomeWindow homeWindow;
	private GameControl gameControl = null;

	public GameObject day;
	public GameObject week;
	public GameObject month;
	public GameObject oxygen;
	public GameObject coin;
	public GameObject abundantCoin;
	[HideInInspector]
	public List<GameObject> messagesObject;

	private int typeRequest;
	private int coinRequest;
	private int goldCoinRequest;
	private int priceRequest;
	private int productMessage;
	private List<int> messages;

	public override void Open () {
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();

		UpdateInformation ();
	}

	// Update Information
	public void UpdateInformation(){
		messages = ReadJSONCardsMessages (gameControl.messages);

		if (gameControl.coin >= 250 && gameControl.oxygen == 0)
			oxygen.GetComponent<Button> ().interactable = true;
		else
			oxygen.GetComponent<Button> ().interactable = false;

		if(messagesObject.Count > 0)
			foreach(var obj in messagesObject){
				var pm = obj.GetComponent<ProductManager> ();

				if (!messages.Contains (pm.productMessage)) {
					if (pm.coin <= gameControl.coin)
						pm.GetComponent<Button> ().interactable = true;
					else
						pm.GetComponent<Button> ().interactable = false;
				} else {
					pm.GetComponent<Button> ().interactable = false;
					pm.transform.Find ("Price").gameObject.SetActive (false);
				}
			}
	}

	// Buy Product
	public void BuyProduct(Button btn){
		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();

		if(requestManager.GetStatus("Buy") == false){
			var product = btn.gameObject.GetComponent<ProductManager> ();

			typeRequest = product.type;
			coinRequest = product.coin;
			goldCoinRequest = product.goldCoin;
			priceRequest = product.price;
			productMessage = product.productMessage;

			requestManager.EnableRequest ("Buy");
		}
	}

	// Buy
	public void Buy() {
		WWWForm data = new WWWForm();
		data.AddField ("type", typeRequest);
		data.AddField ("coin", coinRequest);
		data.AddField ("goldCoin", goldCoinRequest);
		data.AddField ("price", priceRequest);
		data.AddField ("productMessage", productMessage);
		requestManager.SendData ("buy", data, CheckBuy, "Buy");
	}

	// Check Buy
	public void CheckBuy() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True") {
			requestManager.DisableRequest ("Buy");

			// Poopup Close
			popUpManager.ClosePopUp ();

			if (result.ContainsKey ("goldCoin"))
				gameControl.goldCoin = int.Parse(result ["goldCoin"]);
			if (result.ContainsKey ("coin"))
				gameControl.coin = int.Parse (result ["coin"]);
			if (result.ContainsKey ("messages"))
				gameControl.messages = result ["messages"];
			if (result.ContainsKey ("oxygen"))
				gameControl.oxygen = int.Parse (result ["oxygen"]);
			if (result.ContainsKey ("oxygenStatus"))
				gameControl.oxygenStatus = int.Parse (result ["oxygenStatus"]);
			if (result.ContainsKey ("oxygenTime"))
				gameControl.oxygenTime = int.Parse (result ["oxygenTime"]);

			UpdateInformation ();
			homeWindow.UpdateInformation ();
			homeWindow.UpdateInformationOxygen ();
		} else if (result.ContainsKey ("error")) {
			requestManager.DisableRequest ("Buy");
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
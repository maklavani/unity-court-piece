using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;

public class RankingWindow : GenericWindow {
	public RequestManager requestManager;
	public PopUpManager popUpManager;
	private GameControl gameControl = null;

	public GameObject items;
	public GameObject myRankItem;
	[HideInInspector]
	public string ranking;

	private List<RankingData> list = null;
	private RankingData myRank = null;

	// Open
	public override void Open () {
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();

		ResetView ();
		ranking = "all";
		requestManager.EnableRequest ("Ranking");
	}

	// Reset View
	public void ResetView() {
		for (var i = 0; i < 10; i++) {
			var item = items.transform.Find ((i + 1).ToString ()).gameObject;
			item.transform.Find ("Rank").gameObject.SetActive (false);
			item.transform.Find ("Username").gameObject.SetActive (false);
			item.transform.Find ("Level").gameObject.SetActive (false);
			item.transform.Find ("GoldCoin").gameObject.SetActive (false);
		}

		myRankItem.transform.Find ("Rank").Find ("Text").GetComponent<Text> ().text = "-";
		myRankItem.transform.Find ("Username").Find ("Text").GetComponent<Text> ().text = gameControl.username;
		myRankItem.transform.Find ("Level").Find ("Level").GetComponent<Text> ().text = gameControl.level.ToString("C0").Replace("$" , "");
		myRankItem.transform.Find ("GoldCoin").Find ("GoldCoin").GetComponent<Text> ().text = gameControl.goldCoin.ToString("C0").Replace("$" , "");
	}

	// Show List For View
	public void ShowListForView(){
		ResetView ();

		for (var i = 0; i < list.Count; i++) {
			var item = items.transform.Find ((i + 1).ToString ()).gameObject;

			item.transform.Find ("Rank").gameObject.SetActive (true);
			item.transform.Find ("Username").gameObject.SetActive (true);
			item.transform.Find ("Level").gameObject.SetActive (true);
			item.transform.Find ("GoldCoin").gameObject.SetActive (true);

			item.transform.Find ("Rank").Find ("Text").GetComponent<Text> ().text = list[i].rank.ToString("C0").Replace("$" , "");
			item.transform.Find ("Username").Find ("Text").GetComponent<Text> ().text = list[i].username;
			item.transform.Find ("Level").Find ("Level").GetComponent<Text> ().text = list[i].level.ToString("C0").Replace("$" , "");
			item.transform.Find ("GoldCoin").Find ("GoldCoin").GetComponent<Text> ().text = list[i].goldCoin.ToString("C0").Replace("$" , "");
		}

		if (myRank != null) {
			myRankItem.transform.Find ("Rank").Find ("Text").GetComponent<Text> ().text = myRank.rank.ToString("C0").Replace("$" , "");
			myRankItem.transform.Find ("Username").Find ("Text").GetComponent<Text> ().text = myRank.username;
			myRankItem.transform.Find ("Level").Find ("Level").GetComponent<Text> ().text = myRank.level.ToString ("C0").Replace ("$", "");
			myRankItem.transform.Find ("GoldCoin").Find ("GoldCoin").GetComponent<Text> ().text = myRank.goldCoin.ToString ("C0").Replace ("$", "");
		}

		gameControl.translateLanguage = true;
	}

	// Ranking
	public void Ranking() {
		WWWForm data = new WWWForm();
		data.AddField ("ranking", ranking);
		requestManager.SendData ("ranking", data, CheckRanking, "Ranking");
	}

	// Check Ranking
	public void CheckRanking() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True") {
			requestManager.DisableRequest ("Ranking");

			if (result.ContainsKey ("list"))
				list = ReadListRankInformation (result ["list"]);
			if (result.ContainsKey ("myRank"))
				myRank = ReadMyRankInformation (result ["myRank"]);

			ShowListForView ();
		} else if (result.ContainsKey ("error")) {
			requestManager.DisableRequest ("Ranking");
		}
	}

	// Read List Rank Information
	public List<RankingData> ReadListRankInformation(string listString){
		List<RankingData> output = new List<RankingData> ();
		var json = (Dictionary<string , object>)Json.Deserialize (listString);

		if(json.Count > 0)
			foreach (object jsonItem in json.Keys) {
				var key = jsonItem.ToString ();
				Dictionary<string , object> arr = null;
				RankingData rankingData = new RankingData ();

				if (json [key] != null)
					arr = (Dictionary<string , object>)json [key];

				foreach (object arrItem in arr.Keys) {
					var arrKey = arrItem.ToString ();

					if (arr [arrKey] != null) {
						var value = arr [arrKey].ToString ();

						if (arrKey == "rank")
							rankingData.rank = int.Parse(value);
						else if (arrKey == "username")
							rankingData.username = value;
						else if (arrKey == "level")
							rankingData.level = int.Parse (value);
						else if (arrKey == "goldCoin")
							rankingData.goldCoin = int.Parse (value);
					}
				}

				// Add To Table
				output.Add (rankingData);
			}

		return output;
	}

	// Read My Rank Information
	public RankingData ReadMyRankInformation(string listString){
		RankingData output = new RankingData ();
		var json = (Dictionary<string , object>)Json.Deserialize (listString);

		if(json.Count > 0)
			foreach (object jsonItem in json.Keys) {
				var key = jsonItem.ToString ();

				if (json [key] != null) {
					var value = json [key].ToString ();

					if (key == "rank")
						output.rank = int.Parse(value);
					else if (key == "username")
						output.username = value;
					else if (key == "level")
						output.level = int.Parse (value);
					else if (key == "goldCoin")
						output.goldCoin = int.Parse (value);
				}
			}

		return output;
	}
}
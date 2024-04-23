using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TVWindow : GenericWindow {
	public RequestManager requestManager;
	private GameControl gameControl = null;
	private SoundsManager soundsManager;

	public GameObject loadingObject;
	private bool loading = false;

	// Open
	public override void Open () {
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null) {
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();
			soundsManager = gameControl.GetComponent<SoundsManager> ();
		}
	
		soundsManager.DisableAllSounds ();
		soundsManager.EnableSound ("New Game", true);
	
		requestManager.EnableRequest ("TV");
	}

	// Close
	public override void Close () {
		requestManager.DisableRequest ("TV");
		base.Close ();
	}

	// Update
	void Update(){
		// Loading Aniamtion
		if (loading) {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 1f);
			loadingObject.transform.Rotate (Vector3.forward * -90 * (3f * Time.deltaTime));
		} else {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 0f);
		}
	}

	// TV
	public void TV(){
		loading = true;
		WWWForm data = new WWWForm();
		requestManager.SendData ("tv" , data , CheckTV , "TV");
	}

	// Check TV
	public void CheckTV (){
		Dictionary<string,string> result = requestManager.result;

		if (CheckPlayersExsits (result)) {
			// Disable
			loading = false;
			requestManager.DisableRequest ("TV");

			StartCoroutine (gotoGame ());
		} else if (result ["statusRequest"] == "True"){
			requestManager.EnableRequest ("New Game");
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result["error"]);

			if (result ["error"] == "Not Found Game.") {
				loading = false;
				requestManager.DisableRequest ("TV");
				manager.Open ((int)Windows.Home);
			}
		}
	}

	// Check Players Exsits
	private bool CheckPlayersExsits(Dictionary<string,string> result){
		if (result ["statusRequest"] == "True" && result.ContainsKey ("code") && result.ContainsKey ("gameTypeA") && result.ContainsKey ("gameTypeB") && 
			result.ContainsKey ("gameTypeC") && result.ContainsKey ("gameTypeD") && result.ContainsKey ("status") && result.ContainsKey ("userA") && 
			result.ContainsKey ("userB") && result.ContainsKey ("userC") && result.ContainsKey ("userD") && result.ContainsKey ("matchWinA") &&
			result.ContainsKey ("matchWinB") && result.ContainsKey ("turns") && result.ContainsKey ("turn") && result.ContainsKey ("lastTurn") && 
			result.ContainsKey ("firstTurn") && result.ContainsKey ("movements") && result.ContainsKey ("lastMovement") &&result.ContainsKey ("cards") && 
			result.ContainsKey ("cardsAnimation") && result.ContainsKey ("tableGame") && result.ContainsKey ("winner") && result.ContainsKey ("rounds") && 
			result.ContainsKey ("scoreTeamA") && result.ContainsKey ("scoreTeamB") && result.ContainsKey ("governor") && result.ContainsKey ("rule") && 
			result.ContainsKey ("aliasA") && result.ContainsKey ("aliasB") && result.ContainsKey ("aliasC") && result.ContainsKey ("aliasD") && 
			result.ContainsKey ("levelA") && result.ContainsKey ("levelB") && result.ContainsKey ("levelC") && result.ContainsKey ("levelD") && 
			result.ContainsKey ("levelDegreeA") && result.ContainsKey ("levelDegreeB") && result.ContainsKey ("levelDegreeC") && result.ContainsKey ("levelDegreeD") && 
			result.ContainsKey ("faceA") && result.ContainsKey ("faceB") && result.ContainsKey ("faceC") && result.ContainsKey ("faceD") && 
			result.ContainsKey ("calculate") && result.ContainsKey ("winnerCoin") && result.ContainsKey ("loserCoin") && result.ContainsKey ("maxDelay") && 
			result.ContainsKey ("calculate") && result.ContainsKey ("winnerGoldCoin") && result.ContainsKey ("loserGoldCoin") && 
			result ["status"] == "1" && result ["userB"] != "" && result ["userC"] != "" && result ["userD"] != "") {

			gameControl.code = result ["code"];
			gameControl.gameTypeA = result ["gameTypeA"];
			gameControl.gameTypeB = result ["gameTypeB"];
			gameControl.gameTypeC = result ["gameTypeC"];
			gameControl.gameTypeD = result ["gameTypeD"];
			gameControl.status = int.Parse (result ["status"]);
			gameControl.userA = result ["userA"];
			gameControl.userB = result ["userB"];
			gameControl.userC = result ["userC"];
			gameControl.userD = result ["userD"];
			gameControl.matchWinA = int.Parse (result ["matchWinA"]);
			gameControl.matchWinB = int.Parse (result ["matchWinB"]);
			gameControl.turns = int.Parse (result ["turns"]);
			gameControl.turn = int.Parse (result ["turn"]);
			gameControl.lastTurn = int.Parse (result ["lastTurn"]);
			gameControl.firstTurn = int.Parse (result ["firstTurn"]);
			gameControl.movements = result ["movements"];
			gameControl.lastMovement = result ["lastMovement"];
			gameControl.cards = result ["cards"];
			gameControl.cardsAnimation = result ["cardsAnimation"];
			gameControl.tableGame = result ["tableGame"];
			gameControl.winner = int.Parse (result ["winner"]);
			gameControl.rounds = int.Parse (result ["rounds"]);
			gameControl.scoreTeamA = int.Parse (result ["scoreTeamA"]);
			gameControl.scoreTeamB = int.Parse (result ["scoreTeamB"]);
			gameControl.governor = int.Parse (result ["governor"]);
			gameControl.rule = int.Parse (result ["rule"]);
			gameControl.aliasA = result ["aliasA"];
			gameControl.aliasB = result ["aliasB"];
			gameControl.aliasC = result ["aliasC"];
			gameControl.aliasD = result ["aliasD"];
			gameControl.levelA = int.Parse (result ["levelA"]);
			gameControl.levelB = int.Parse (result ["levelB"]);
			gameControl.levelC = int.Parse (result ["levelC"]);
			gameControl.levelD = int.Parse (result ["levelD"]);
			gameControl.levelDegreeA = float.Parse (result ["levelDegreeA"]);
			gameControl.levelDegreeB = float.Parse (result ["levelDegreeB"]);
			gameControl.levelDegreeC = float.Parse (result ["levelDegreeC"]);
			gameControl.levelDegreeD = float.Parse (result ["levelDegreeD"]);
			gameControl.faceA = int.Parse (result ["faceA"]);
			gameControl.faceB = int.Parse (result ["faceB"]);
			gameControl.faceC = int.Parse (result ["faceC"]);
			gameControl.faceD = int.Parse (result ["faceD"]);
			gameControl.messageA = int.Parse (result ["messageA"]);
			gameControl.messageB = int.Parse (result ["messageB"]);
			gameControl.messageC = int.Parse (result ["messageB"]);
			gameControl.messageD = int.Parse (result ["messageD"]);
			gameControl.maxDelay = int.Parse (result ["maxDelay"]);
			gameControl.calculate = int.Parse (result ["calculate"]) == 0 ? false : true;
			gameControl.winnerCoin = int.Parse (result ["winnerCoin"]);
			gameControl.loserCoin = int.Parse (result ["loserCoin"]);
			gameControl.winnerCoin = int.Parse (result ["winnerGoldCoin"]);
			gameControl.loserCoin = int.Parse (result ["loserGoldCoin"]);

			return true;
		}

		if (result.ContainsKey ("userA") && result.ContainsKey ("faceA") && result ["userA"] != "" && result ["faceA"] != "") {
			gameControl.userA = result ["userA"];
			gameControl.faceA = int.Parse (result ["faceA"]);
		}

		if (result.ContainsKey ("userB") && result.ContainsKey ("faceB") && result ["userB"] != "" && result ["faceB"] != "") {
			gameControl.userB = result ["userB"];
			gameControl.faceB = int.Parse (result ["faceB"]);
		}

		if (result.ContainsKey ("userC") && result.ContainsKey ("faceC") && result ["userC"] != "" && result ["faceC"] != "") {
			gameControl.userC = result ["userC"];
			gameControl.faceC = int.Parse (result ["faceC"]);
		}

		if (result.ContainsKey ("userD") && result.ContainsKey ("faceD") && result ["userD"] != "" && result ["faceD"] != "") {
			gameControl.userD = result ["userD"];
			gameControl.faceD = int.Parse (result ["faceD"]);
		}

		return false;
	}

	// GotoGame
	public IEnumerator gotoGame(){
		yield return new WaitForSeconds (1f);
		soundsManager.DisableAllSounds ();
		gameControl.LoadScene ("Hokm");
	}
}
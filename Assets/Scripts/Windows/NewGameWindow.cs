using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class NewGameWindow : GenericWindow {
	public RequestManager requestManager;
	private GameControl gameControl = null;
	private AnimationManager animationManager;
	private SoundsManager soundsManager;

	[HideInInspector]
	public List<GameObject> users;
	public GameObject loadingObject;

	private string usernameA = "";
	private string usernameB = "";
	private string usernameC = "";
	private string usernameD = "";
	private int rounds = 1;
	private bool loading = false;

	// Animation
	private float delay = 0.0f;
	private List<int> frames = new List<int> (){ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
	private List<int> userFrame = new List<int> ();
	private List<bool> statusFrame = new List<bool> ();

	// Open
	public override void Open (){
		base.Open ();

		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null) {
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();
			soundsManager = gameControl.GetComponent<SoundsManager> ();
			animationManager = gameControl.GetComponent<AnimationManager> ();
		}

		SetRandomFrame ();
		soundsManager.DisableAllSounds ();
		soundsManager.EnableSound ("New Game", true);

		requestManager.EnableRequest ("New Game");
	}

	// Update
	void Update(){
		CheckUsernameExist ();

		// Loading Aniamtion
		if (loading) {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 1f);
			loadingObject.transform.Rotate (Vector3.forward * -90 * (3f * Time.deltaTime));
		} else {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 0f);
		}

		// Animation
		if (statusFrame.Count > 0){
			delay += Time.deltaTime;

			if (delay > 0.075f) {
				delay = 0f;

				for (var i = 0; i < statusFrame.Count; i++)
					if (statusFrame [i] == true) {
						userFrame [i] = (userFrame [i] + 1) % frames.Count;
						SetImage (i, userFrame [i]);
					}
			}
		}
	}

	// Close
	public override void Close (){
		requestManager.DisableRequest ("New Game");
		base.Close ();
	}

	// Set Random Frame
	public void SetRandomFrame () {
		if (users.Count > 0)
			foreach (var user in users) {
				userFrame.Add (Random.Range(0 , frames.Count - 1));
				statusFrame.Add (true);
			}
	}

	// Set Image
	public void SetImage(int user , int frame){
		users [user].transform.Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [frames[frame] - 1];
	}

	// Check Username Exist
	public void CheckUsernameExist () {
		if (gameControl.userA != "" && usernameA == "") {
			usernameA = gameControl.userA;
			statusFrame [0] = false;
			users [0].transform.Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceA - 1];
			var username = users [0].transform.Find ("TextParent").Find ("Username").gameObject;
			username.SetActive (true);
			username.GetComponent<Text> ().text = usernameA;
		} else if (gameControl.userB != "" && usernameB == "") {
			usernameB = gameControl.userB;
			statusFrame [1] = false;
			users [1].transform.Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceB - 1];
			var username = users [1].transform.Find ("TextParent").Find ("Username").gameObject;
			username.SetActive (true);
			username.GetComponent<Text> ().text = usernameB;
		} else if (gameControl.userC != "" && usernameC == "") {
			usernameC = gameControl.userC;
			statusFrame [2] = false;
			users [2].transform.Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceC - 1];
			var username = users [2].transform.Find ("TextParent").Find ("Username").gameObject;
			username.SetActive (true);
			username.GetComponent<Text> ().text = usernameC;
		} else if (gameControl.userD != "" && usernameD == "") {
			usernameD = gameControl.userD;
			statusFrame [3] = false;
			users [3].transform.Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceD - 1];
			var username = users [3].transform.Find ("TextParent").Find ("Username").gameObject;
			username.SetActive (true);
			username.GetComponent<Text> ().text = usernameD;
		}
	}

	// New Game
	public void NewGame(){
		loading = true;

		WWWForm data = new WWWForm();
		data.AddField ("gameType" , "randomGame");
		data.AddField ("rounds" , rounds);
		requestManager.SendData ("newgame" , data , CheckNewGame , "New Game");
	}

	// Check New Game
	public void CheckNewGame (){
		Dictionary<string,string> result = requestManager.result;

		if (CheckPlayersExsits (result)) {
			// Disable
			loading = false;
			requestManager.DisableRequest ("New Game");

			StartCoroutine (gotoGame ());
		} else if (result ["statusRequest"] == "True"){
			requestManager.EnableRequest ("New Game");
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result["error"]);
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
			gameControl.messageC = int.Parse (result ["messageC"]);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;

public class GameManager : MonoBehaviour
{
	[HideInInspector]
	public GameControl gameControl = null;
	[HideInInspector]
	public SoundsManager soundsManager;
	[HideInInspector]
	public AnimationManager animationManager;
	public PopUpManager popUpManager;
	public CardManager cardManager;
	[HideInInspector]
	public RequestManager requestManager;
	[HideInInspector]
	public MessageManager messageManager;

	public GameObject scoreA;
	public GameObject scoreB;
	public GameObject loadingObject;
	public GameObject winnerCanvas;
	[HideInInspector]
	public List<GameObject> users;
	[HideInInspector]
	public List<GameObject> timer;

	// Game Information
	[HideInInspector]
	public int turns = 0;
	[HideInInspector]
	public int turn = 0;
	[HideInInspector]
	public int lastTurn = 0;
	[HideInInspector]
	public int firstTurn = 0;
	[HideInInspector]
	public TableGameData lastMovement = new TableGameData();
	[HideInInspector]
	public List<TableGameData> tableGame = new List<TableGameData>();
	[HideInInspector]
	public List<Card> cardsAnimation = new List<Card>();
	[HideInInspector]
	public List<Card> cards = new List<Card>();
	[HideInInspector]
	public int governor = 0;
	[HideInInspector]
	public int rule = 0;
	[HideInInspector]
	public int matchWinA = 0;
	[HideInInspector]
	public int matchWinB = 0;
	[HideInInspector]
	public int scoreTeamA = 0;
	[HideInInspector]
	public int scoreTeamB = 0;
	[HideInInspector]
	public int messageA;
	[HideInInspector]
	public int messageB;
	[HideInInspector]
	public int messageC;
	[HideInInspector]
	public int messageD;
	[HideInInspector]
	public int maxDelay;

	// Control
	[HideInInspector]
	public bool init = false;
	[HideInInspector]
	public int myTurn = 0;
	[HideInInspector]
	public bool tv = false;

	[HideInInspector]
	public bool gameEnded = false;
	[HideInInspector]
	public bool resetForNewSet = false;
	[HideInInspector]
	public bool loading = false;
	[HideInInspector]
	public bool showWinnerPage = false;
	[HideInInspector]
	public int stepSetGovernor = 0;
	[HideInInspector]
	public bool showCardForSelection = false;
	[HideInInspector]
	public int numberCardInCenter = 0;
	[HideInInspector]
	public bool destroyCardAtEndMatch = false;
	[HideInInspector]
	public bool disableGetMovement = false;

	[HideInInspector]
	public int message = 0;
	[HideInInspector]
	public int ruleNumber = 0;
	[HideInInspector]
	public int cardType = 0;
	[HideInInspector]
	public int cardNumber = 0;
	[HideInInspector]
	public string friend;

	// Timer
	[HideInInspector]
	public bool playTimeOutStatus = false;
	[HideInInspector]
	public float delay;
	[HideInInspector]
	public float timeElapsed;
	[HideInInspector]
	public Text timeOutText;

	// Awake
	void Awake(){
		requestManager = GetComponent<RequestManager> ();
		messageManager = GetComponent<MessageManager> ();
	}

	// Fixed Update
	void FixedUpdate(){
		if (gameControl == null && GameObject.Find ("GameControl") != null) {
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();
			soundsManager = gameControl.GetComponent<SoundsManager> ();
			animationManager = gameControl.GetComponent<AnimationManager> ();
		}

		// Initialize
		if (gameControl != null && !init) {
			// Set My Turn
			if (gameControl.username == gameControl.userA)
				myTurn = 1;
			else if (gameControl.username == gameControl.userB)
				myTurn = 2;
			else if (gameControl.username == gameControl.userC)
				myTurn = 3;
			else if (gameControl.username == gameControl.userD)
				myTurn = 4;
			else
				tv = true;

			// Set Information
			turns = gameControl.turns;
			matchWinA = gameControl.matchWinA;
			matchWinB = gameControl.matchWinB;
			scoreTeamA = gameControl.scoreTeamA;
			scoreTeamB = gameControl.scoreTeamB;
			rule = gameControl.rule;
			messageA = gameControl.messageA;
			messageB = gameControl.messageB;
			messageC = gameControl.messageC;
			messageD = gameControl.messageD;
	        
			UpdateInformation ();
			SetScores ();
			StartAnimation ();

			// Enable Alive or TV
			if (!tv) {
				// Disable Add to friend Button
				users [0].transform.Find ("ImageParent").GetComponent<Button> ().interactable = false;
				requestManager.EnableRequest ("Alive");
			} else {
				requestManager.EnableRequest ("Get Movement");
				users [0].transform.Find ("ImageParent").Find ("Background").gameObject.SetActive (true);
				users [0].transform.Find ("ImageParent").Find ("Image").gameObject.SetActive (true);
				users [0].transform.Find ("Username").gameObject.SetActive (true);
				GameObject.Find ("UserLayout").transform.Find ("Dialog").gameObject.SetActive (false);
				GameObject.Find ("TimerLayout").gameObject.SetActive (false);
			}
	  
			init = true;
		} else if (!gameEnded) {
			Manager ();
		} else if (gameEnded && !showWinnerPage) {
			StartCoroutine (ShowWinner ());
		}
	}

	// Update
	void Update(){
		// Loading Aniamtion
		if (loading) {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 1f);
			loadingObject.transform.Rotate (Vector3.forward * -90 * Time.deltaTime);
		} else {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 0f);
		}

		// Timer
		if (playTimeOutStatus) {
			delay += Time.deltaTime;

			if (delay > 1f) {
				delay = 0f;
				timeElapsed += 1f;

				if(timeElapsed == maxDelay + 1) {
					playTimeOutStatus = false;
					timeOutText.text = "";
					Debug.Log (rule);

					if (myTurn == turn) {
						if (rule != 0) {
							// Set Random
							List<MoveData> posibleMove = cardManager.GetPosibleMove (myTurn);

							if (posibleMove.Count > 0) {
								MoveData randomPosibleMove = posibleMove [Random.Range (0, posibleMove.Count - 1)];
								cardManager.cardClickStep = true;
								cardManager.cardClicked.type = randomPosibleMove.type;
								cardManager.cardClicked.number = randomPosibleMove.number;
								cardManager.CardClick (GameObject.Find ("Card_" + randomPosibleMove.type + "_" + randomPosibleMove.number).GetComponent<Button> ());
							}
						} else if (rule == 0 && governor == myTurn) {
							cardManager.setRule.SetActive (false);
							ruleNumber = Random.Range (1, 4);

							requestManager.EnableRequest ("Set Rule");
							requestManager.DisableRequest ("Send Data");
							requestManager.DisableRequest ("Check Movement");
						}
					}
				} else {
					if(maxDelay >= timeElapsed)
						timeOutText.text = (maxDelay - timeElapsed).ToString ();
					else
						timeOutText.text = "";
				}
			}
		}
	}

	// Update Information
	public void UpdateInformation()
	{
		turn = gameControl.turn;
		lastTurn = gameControl.lastTurn;
		firstTurn = gameControl.firstTurn;
	    cardsAnimation = ReadJSONCardsAnimation(gameControl.cardsAnimation);
	    cards = ReadJSONCardsAnimation(gameControl.cards);
	    governor = gameControl.governor;
		maxDelay = gameControl.maxDelay;
		ReadJSONTableGame(gameControl.tableGame);
		ReadJSONLastMovement(gameControl.lastMovement);
		UpdateInformationMessage ();

		// Set Username and Image
		var colorA = new Color (0.01f, 0.39f, 0.44f, 1f);
		var colorB = new Color (0.47f, 0f, 0.14f, 1f);

		for(var i = 1 ; i < 5;i++){
			int userPosition = cardManager.GetTurnPositionNumber (i);
			int faceImage = 1;
			string usernametext = "";

			if (i == 1) {
				faceImage = gameControl.faceA;
				usernametext = gameControl.userA;
			} else if (i == 2) {
				faceImage = gameControl.faceB;
				usernametext = gameControl.userB;
			} else if (i == 3) {
				faceImage = gameControl.faceC;
				usernametext = gameControl.userC;
			} else {
				faceImage = gameControl.faceD;
				usernametext = gameControl.userD;
			}

			users [userPosition - 1].transform.Find ("ImageParent").Find ("Background").GetComponent<Image> ().color = (i + myTurn % 2) % 2 == 1 ? colorA : colorB;
			users [userPosition - 1].transform.Find ("ImageParent").Find ("Image").GetComponent<Image> ().sprite =  animationManager.faceImages [faceImage - 1];
			users [userPosition - 1].transform.Find ("Username").GetComponent<Text> ().text = usernametext;
		}

		// Play Timrout
		DisablePlayTimeOut ();
		float delayTimeOutAnimation = 0f;
		StartCoroutine (PlayTimeOut (delayTimeOutAnimation));
		gameControl.translateLanguage = true;
	}

	public void UpdateInformationMessage(){
		if (gameControl.messageA != messageA && gameControl.messageA != 0) {
			Debug.Log ("message A Show" + messageA + " - " + cardManager.GetTurnPosition (1));
			messageA = gameControl.messageA;
			messageManager.ShowMessage (cardManager.GetTurnPosition (1), messageA);
		}

		if (gameControl.messageB != messageB && gameControl.messageB != 0) {
			Debug.Log ("message B Show" + messageB + " - " + cardManager.GetTurnPosition (2));
			messageB = gameControl.messageB;
			messageManager.ShowMessage (cardManager.GetTurnPosition (2), messageB);
		}

		if (gameControl.messageC != messageC && gameControl.messageC != 0) {
			Debug.Log ("message C Show" + messageC + " - " + cardManager.GetTurnPosition (3));
			messageC = gameControl.messageC;
			messageManager.ShowMessage (cardManager.GetTurnPosition (3), messageC);
		}

		if (gameControl.messageD != messageD && gameControl.messageD != 0) {
			Debug.Log ("message D Show" + messageD + " - " + cardManager.GetTurnPosition (4));
			messageD = gameControl.messageD;
			messageManager.ShowMessage (cardManager.GetTurnPosition (4), messageD);
		}
	}

	// Start ANimation
	public void StartAnimation(){
		disableGetMovement = true;

		if (rule == 0) {
			if (maxDelay > 9 && scoreTeamA == gameControl.scoreTeamA && scoreTeamB == gameControl.scoreTeamB && scoreTeamA == 0 && scoreTeamB == 0) {
				Debug.Log ("Animation For Set Governor");
				cardManager.initCards (cardsAnimation);
				StartCoroutine (cardManager.animationForSetGovernor ());
			} else {
				// Set Kardane Payane ANimation Governor
				cardManager.animationGovernoverEnd = true;

				cardManager.initCards (cards , false);
				cardManager.animationRuleStart = false;
				cardManager.animationsEnd = true;
				SetCrownIcon ();
			}
		} else {
			cardManager.initCards (cards, false);
			SetRuleIcon ();

			if (turns == 0) {
				stepSetGovernor = 2;
				cardManager.animationGovernoverEnd = true;
				StartCoroutine (cardManager.animationForSetRule (governor, 0, 12));
			} else {
				stepSetGovernor = 3;
				cardManager.animationGovernoverEnd = true;
				cardManager.animationRuleStart = true;
				cardManager.SetPositionUsesCard ();
				StartCoroutine (cardManager.animationForSetRule (governor, 0, 12, false));
				StartCoroutine (cardManager.animationForSort (false));
				StartCoroutine (cardManager.SetPositionCenterCard ());

				// Disable Get Movement
				Debug.Log("C");
				disableGetMovement = false;
			}
		}
	}

	// PLay Timeout
	public IEnumerator PlayTimeOut(float delayInput){
		yield return new WaitForSeconds(delayInput);
	
		int userPosition = cardManager.GetTurnPositionNumber (turn);

		if (tv && userPosition == 1)
			userPosition += 4;

		for (var i = 1; i < 6; i++)
			timer [i - 1].SetActive (false);

		timer [userPosition - 1].SetActive (true);
		timeOutText = timer [userPosition - 1].GetComponent<Text> ();
		timeOutText.text = maxDelay.ToString ();

		playTimeOutStatus = true;
		delay = 0f;
		timeElapsed = 0f;
		gameControl.translateLanguage = true;
	}

	// Disable Play TimeOut
	public void DisablePlayTimeOut(){
		playTimeOutStatus = false;
		timeElapsed = 0f;
		delay = 0f;
	}

	// Set Ruled Click
	public void SetRuleClick(int ruleIn){
		ruleNumber = ruleIn;
		cardManager.setRule.SetActive (false);
		requestManager.EnableRequest ("Set Rule");
	}

	// Set Crown Icon
	public void SetCrownIcon(){
		for (var i = 0; i < 4; i++)
			users [i].transform.Find ("UserImage").gameObject.SetActive (false);

		int governorPosition = cardManager.GetTurnPositionNumber (governor);

		Transform userImage;

		if (tv && governorPosition == 1)
			userImage = users [governorPosition - 1].transform.Find ("UserImageB");
		else
			userImage = users [governorPosition - 1].transform.Find ("UserImage");

		userImage.gameObject.SetActive (true);
		userImage.transform.Find ("Icon").GetComponent<Text> ().text = LanguageManager.GetIconConverted ("8");
		userImage.transform.Find ("Icon").GetComponent<Text> ().color = new Color (0.92f, 0.79f, 0.22f, 1f);

		gameControl.translateLanguage = true;
	}

	// Set Rule Icon
	public void SetRuleIcon(){
		for (var i = 0; i < 4; i++)
			users [i].transform.Find ("UserImage").gameObject.SetActive (false);

		int governorPosition = cardManager.GetTurnPositionNumber (governor);
		Transform userImage;

		if (tv && governorPosition == 1)
			userImage = users [governorPosition - 1].transform.Find ("UserImageB");
		else
			userImage = users [governorPosition - 1].transform.Find ("UserImage");

		userImage.gameObject.SetActive (true);
		userImage.transform.Find ("Icon").GetComponent<Text> ().text = LanguageManager.GetIconConverted ((rule + 3).ToString ());
		userImage.transform.Find ("Icon").GetComponent<Text> ().color = rule % 2 == 1 ? new Color (0.88f, 0f, 0f, 1f) : new Color (0f, 0f, 0f, 1f);
	}

	// Set Score
	public void SetScores(){
		var sta = scoreA.transform.Find ("ScoreTeamA");
		var stb = scoreB.transform.Find ("ScoreTeamB");
		var mwa = scoreA.transform.Find ("MatchWinA");
		var mwb = scoreB.transform.Find ("MatchWinB");

		sta.GetComponent<Text> ().text = scoreTeamA.ToString ();
		stb.GetComponent<Text> ().text = scoreTeamB.ToString ();
		mwa.GetComponent<Text> ().text = matchWinA.ToString ();
		mwb.GetComponent<Text> ().text = matchWinB.ToString ();

		var colorA = new Color (0.01f, 0.39f, 0.44f, 1f);
		var colorB = new Color (0.47f, 0f, 0.14f, 1f);
		var colorC = new Color (0.9f, 0.9f, 0.9f, 1f);

		sta.GetComponent<Text> ().color = myTurn % 2 == 0 ? colorA : colorB;
		stb.GetComponent<Text> ().color = myTurn % 2 == 0 ? colorB : colorA;
		mwa.GetComponent<Text> ().color = colorC;
		mwb.GetComponent<Text> ().color = colorC;

		gameControl.translateLanguage = true;
	}

	// Manager
	public void Manager()
	{
		// Show Winner Page
		if (gameControl.winner != 0 && gameControl.status == 2) {
			// For Show last Movement
			if (numberCardInCenter == 4) {
				gameEnded = true;
				StartCoroutine (cardManager.DestroyCards (1f, 1f));

				requestManager.DisableRequest ("Send Data");
				requestManager.DisableRequest ("Get Movement");
				requestManager.DisableRequest ("Alive");
				// Baraye Peyghame Akhare Bazi Faaal Shavad
				// requestManager.EnableRequest ("Alive");
			} else {
				cardManager.ShowLastMovement ();
			}
		} else if (resetForNewSet && destroyCardAtEndMatch && cardManager.destroyEnd) {
			turns = -2;
			rule = 0;

			resetForNewSet = false;
			destroyCardAtEndMatch = false;
		} else if (resetForNewSet && !destroyCardAtEndMatch && !cardManager.animationsEnd) {
			destroyCardAtEndMatch = true;
			StartCoroutine (cardManager.DestroyCards (1f, 1f));
		} else {
			// Create New Cards
			if (scoreTeamA == gameControl.scoreTeamA && scoreTeamB == gameControl.scoreTeamB) {
				// Reset
				if (Mathf.Abs (gameControl.turns - turns) > 1) {
					Debug.Log ("Create New Cards");

					rule = gameControl.rule;
					turns = gameControl.turns;
					governor = gameControl.governor;
					cardManager.animationGovernoverEnd = false;
					stepSetGovernor = 0;
					StartAnimation ();
				}
			}

			// Set Rule Animation
			if (rule == 0 && gameControl.rule != 0 && stepSetGovernor == 1) {
				Debug.Log ("Set Rule Animation");
				StartCoroutine (cardManager.animationForSetRule (governor, 5, 8));
				rule = gameControl.rule;
				SetRuleIcon ();
			}
				
			// Agar Daste Jadid Bud
			if (scoreTeamA != gameControl.scoreTeamA || scoreTeamB != gameControl.scoreTeamB) {
				// Reset Kadane Score
				if (numberCardInCenter == 0 && cardManager.animationsEnd) {
					Debug.Log ("Reset Kadane Score");
					scoreTeamA = gameControl.scoreTeamA;
					scoreTeamB = gameControl.scoreTeamB;
					matchWinA = gameControl.matchWinA;
					matchWinB = gameControl.matchWinB;

					resetForNewSet = true;
					cardManager.animationsEnd = false;
					cardManager.destroyEnd = false;
					turns = gameControl.turns;

					SetScores ();
				} else if (numberCardInCenter != 0) {
					Debug.Log ("Agar Daste Jadid Bud");
					cardManager.ShowLastMovement ();
				}
			}

			// Nobate Harekat Harif
			if (turn != myTurn && !disableGetMovement && !requestManager.GetStatus ("Get Movement")) {
				Debug.Log ("Nobate Harekat Harif");
				requestManager.EnableRequest ("Get Movement");
				requestManager.DisableRequest ("Alive");
			}

			// Show Aniamtion Card
			if (turns != -1 && turns != gameControl.turns && gameControl.turns == turns + 1) {
				Debug.Log ("Show Aniamtion Card");
				cardManager.ShowLastMovement ();
				turns = gameControl.turns;
			}

			// Show Card For Selection
			if (!showCardForSelection && myTurn == turn && stepSetGovernor == 3) {
				Debug.Log ("Show Card For Selection");
				cardManager.CardSelection ();
				showCardForSelection = true;
				Handheld.Vibrate ();
			}

			// Center Is Fulled 
			if (numberCardInCenter == 4 && (matchWinA != gameControl.matchWinA || matchWinB != gameControl.matchWinB)) {
				Debug.Log ("Center Is Fulled ");
				matchWinA = gameControl.matchWinA;
				matchWinB = gameControl.matchWinB;
				StartCoroutine (cardManager.animationForCenter ());
				SetScores ();
				numberCardInCenter = 0;
			}
		}
	}

	// Show Winner
	public IEnumerator ShowWinner(){
		yield return new WaitForSeconds (1f);
		winnerCanvas.SetActive (true);
		showWinnerPage = true;

		// Set Options
		winnerCanvas.transform.Find("WinnerPage").Find("Top").Find("UserA").Find("TextParent").Find ("Username").GetComponent<Text> ().text = gameControl.userA;
		winnerCanvas.transform.Find("WinnerPage").Find("Top").Find("UserC").Find("TextParent").Find ("Username").GetComponent<Text> ().text = gameControl.userC;
		winnerCanvas.transform.Find("WinnerPage").Find("Bottom").Find("UserB").Find("TextParent").Find ("Username").GetComponent<Text> ().text = gameControl.userB;
		winnerCanvas.transform.Find("WinnerPage").Find("Bottom").Find("UserD").Find("TextParent").Find ("Username").GetComponent<Text> ().text = gameControl.userD;
		winnerCanvas.transform.Find("WinnerPage").Find("Top").Find("UserA").Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceA - 1];
		winnerCanvas.transform.Find("WinnerPage").Find("Top").Find("UserC").Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceC - 1];
		winnerCanvas.transform.Find("WinnerPage").Find("Bottom").Find("UserB").Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceB - 1];
		winnerCanvas.transform.Find("WinnerPage").Find("Bottom").Find("UserD").Find ("ImageParent").Find ("Image").Find ("Face").GetComponent<Image> ().sprite = animationManager.faceImages [gameControl.faceD - 1];

		if (gameControl.winner == 1) {
			winnerCanvas.transform.Find ("WinnerPage").Find ("Top").Find ("UserA").Find ("ImageParent").Find ("Image").Find ("Medal").gameObject.SetActive (true);
			winnerCanvas.transform.Find ("WinnerPage").Find ("Top").Find ("UserC").Find ("ImageParent").Find ("Image").Find ("Medal").gameObject.SetActive (true);
		} else if (gameControl.winner == 2) {
			winnerCanvas.transform.Find ("WinnerPage").Find ("Bottom").Find ("UserB").Find ("ImageParent").Find ("Image").Find ("Medal").gameObject.SetActive (true);
			winnerCanvas.transform.Find ("WinnerPage").Find ("Bottom").Find ("UserD").Find ("ImageParent").Find ("Image").Find ("Medal").gameObject.SetActive (true);
		}

		// Play Shound
		soundsManager.EnableSound("Win");
		gameControl.translateLanguage = true;
	}

	// Exit Popup
	public void ExitPopup(){}

	// Add Friend Popup
	public void AddFriendPopup(){}

	// Open Exit Popup
	public void OpenExitPopup(){
		popUpManager.ShowPopUp ("Exit");
	}

	// Open Add Friend Popup
	public void OpenAddFriendPopup(Button btn){
		friend = btn.transform.parent.Find ("Username").GetComponent<Text> ().text;
		popUpManager.ShowPopUp ("AddFriend");
	}

	// Exit
	public void Exit(){
		if (tv)
			ExitToMain ();
		else {
			popUpManager.ClosePopUp ();
			requestManager.EnableRequest ("Exit Game");
		}
	}

	// Exit to Main
	public void ExitToMain(){
		requestManager.DisableRequest ("Send Data");
		requestManager.DisableRequest ("Get Movement");
		requestManager.DisableRequest ("Alive");

		gameControl.code = "";
		gameControl.gameTypeA = "";
		gameControl.gameTypeB = "";
		gameControl.gameTypeC = "";
		gameControl.gameTypeD = "";
		gameControl.status = 0;
		gameControl.userA = "";
		gameControl.userB = "";
		gameControl.userC = "";
		gameControl.userD = "";
		gameControl.LoadScene ("Main");
	}

	// Add Friend Yes Button
	public void AddFriendYesButton(){
		requestManager.EnableRequest ("Add Friend");
		popUpManager.ClosePopUp ();
	}

	// Set Rule
	public void SetRule()
	{
		loading = true;

		// Disable Play Timeout
		DisablePlayTimeOut ();

	    WWWForm data = new WWWForm();
	    data.AddField("code", gameControl.code);
	    data.AddField("type", ruleNumber);
	    requestManager.SendData("setrule", data, CheckSetRule, "Set Rule");
	}

	// Check Set Rule
	public void CheckSetRule()
	{
		Dictionary<string, string> result = requestManager.result;

		if (CheckPlayersExsits (result) && gameControl.rule != 0) {
			// Disable
			loading = false;
			showCardForSelection = false;
			requestManager.DisableRequest ("Set Rule");

			// Update Information
			UpdateInformation ();
			UpdateInformationMessage ();
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result ["error"]);
			requestManager.DisableRequest ("Set Rule");

			if (result ["error"] == "Rule is Setted.") {
				requestManager.EnableRequest ("Get Movement");
				requestManager.DisableRequest ("Alive");
			}
		}
	}

	// Send Data
	public void SendData()
	{
		loading = true;

		// Disable Play Timeout
		DisablePlayTimeOut ();

		WWWForm data = new WWWForm();
		data.AddField("code", gameControl.code);
		data.AddField("type", cardType);
		data.AddField("number", cardNumber);
		requestManager.SendData("senddata", data, CheckSendData, "Send Data");
	}

	// Check Send Data
	public void CheckSendData()
	{
		Dictionary<string, string> result = requestManager.result;

		if (CheckPlayersExsits (result) && gameControl.turns != turns) {
			// Disable
			loading = false;
			showCardForSelection = false;
			requestManager.DisableRequest ("Send Data");

			// Update Information
			UpdateInformation ();
			UpdateInformationMessage ();
			turns = gameControl.turns;
		} else if (result.ContainsKey ("error")) {
			if (result ["error"] == "Turns was not accepted.") {
				requestManager.DisableRequest ("Send Data");
				requestManager.EnableRequest ("Get Movement");
				requestManager.DisableRequest ("Alive");
			}
			Debug.Log (result ["error"]);
		}
	}

	// Get Movement
	public void GetMovement()
	{
		loading = true;

		WWWForm data = new WWWForm ();
		data.AddField ("code", gameControl.code);
		data.AddField ("message", message);
		requestManager.SendData ("getmovement", data, CheckGetMovement, "Get Movement");
	}

	// Check Get Movement
	public void CheckGetMovement()
	{
		Dictionary<string, string> result = requestManager.result;

		if (CheckPlayersExsits (result) && (gameControl.rule != rule || gameControl.turns != turns)) {
			// Disable
			loading = false;

			requestManager.DisableRequest ("Get Movement");
			requestManager.EnableRequest ("Alive");

			// Disable Play Timeout
			if (turns != gameControl.turns)
				DisablePlayTimeOut ();

			if (turn == myTurn && turn != gameControl.turn)
				cardManager.DisableAllSelection ();

			// Update Information
			UpdateInformation ();
			UpdateInformationMessage ();
		} else if (result.ContainsKey ("error"))
			Debug.Log (result ["error"]);
	}

	// Alive
	public void Alive(){
		WWWForm data = new WWWForm();
		data.AddField("code" , gameControl.code);
		data.AddField("message" , message);
		requestManager.SendData ("alive", data , CheckAlive , "Alive");
	}

	// Check Alive
	public void CheckAlive(){
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True" && result.ContainsKey ("messageA") && result.ContainsKey ("messageB") && result.ContainsKey ("messageC") && result.ContainsKey ("messageD")) {
			if (int.Parse (result ["messageA"]) != messageA)
				gameControl.messageA = int.Parse (result ["messageA"]);
			if (int.Parse (result ["messageB"]) != messageB)
				gameControl.messageB = int.Parse (result ["messageB"]);
			if (int.Parse (result ["messageC"]) != messageC)
				gameControl.messageC = int.Parse (result ["messageC"]);
			if (int.Parse (result ["messageD"]) != messageD)
				gameControl.messageD = int.Parse (result ["messageD"]);

			// Update Information Message
			UpdateInformationMessage ();

			if ((result.ContainsKey ("turns") && int.Parse (result ["turns"]) != turns) || (result.ContainsKey ("rule") && int.Parse (result ["rule"]) != rule)) {
				requestManager.DisableRequest ("Alive");
				requestManager.EnableRequest ("Get Movement");
			}
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result ["error"]);
		}
	}

	// Add Friend
	public void AddFriend(){
		WWWForm data = new WWWForm();
		data.AddField ("friend", friend);
		requestManager.SendData ("addfriend", data, CheckAddFriend, "Add Friend");
	}

	// Check Add Friend
	public void CheckAddFriend() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True" && result.ContainsKey ("statusFriend") && result.ContainsKey ("usernameFriend") && result.ContainsKey ("levelFriend")) {
			requestManager.DisableRequest ("Add Friend");
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result ["error"]);
			requestManager.DisableRequest ("Add Friend");
		}
	}

	// Exit Game
	public void ExitGame(){
		WWWForm data = new WWWForm();
		data.AddField("code" , gameControl.code);
		requestManager.SendData ("exitgame", data, CheckExitGame, "Exit Game");
	}

	// Check Exit Game
	public void CheckExitGame() {
		Dictionary<string,string> result = requestManager.result;

		if (result ["statusRequest"] == "True") {
			requestManager.DisableRequest ("Exit Game");
			ExitToMain ();
		} else if (result.ContainsKey ("error")) {
			Debug.Log (result ["error"]);
			requestManager.DisableRequest ("Exit Game");
		}
	}

	// Read JSON Cards Animation
	public List<Card> ReadJSONCardsAnimation(string input)
	{
		var json = (Dictionary<string, object>)Json.Deserialize (input);
		List<Card> list = new List<Card> ();

		if (json.Count > 0)
			foreach (object jsonItem in json.Keys) {
				var key = jsonItem.ToString ();
				Dictionary<string, object> arr = null;
				if (json [key] != null)
					arr = (Dictionary<string, object>)json [key];

				// Add To List
				Card card = new Card ();
				card.number = int.Parse (arr ["number"].ToString ());
				card.type = (Cards)int.Parse (arr ["type"].ToString ());
				list.Add (card);
			}

		return list;
	}

	// Read JSON Table Game
	public void ReadJSONTableGame(string input)
	{
		var json = (Dictionary<string, object>)Json.Deserialize (input);
		tableGame = new List<TableGameData> ();

		if (json.Count > 0)
			foreach (object jsonItem in json.Keys) {
				var key = jsonItem.ToString ();
				Dictionary<string, object> arr = null;
	    
				if (json [key] != null)
					arr = (Dictionary<string, object>)json [key];

				TableGameData itemTableGame = new TableGameData ();

				if (arr.Count > 0)
					foreach (var arrItem in arr.Keys) {
						var keyItem = arrItem.ToString ();

						if (keyItem == "moves") {
							Dictionary<string, object> movesArr = null;
							List<MoveData> moves = new List<MoveData> ();

							if (arr [arrItem] != null)
								movesArr = (Dictionary<string, object>)arr [arrItem];

							if (movesArr.Count > 0)
								foreach (var moveItem in movesArr.Keys) {
									var keyMove = moveItem.ToString ();

									Dictionary<string, object> movesArrItems = null;
									MoveData move = new MoveData ();

									if (movesArr [keyMove] != null)
										movesArrItems = (Dictionary<string, object>)movesArr [keyMove];
	                        
									move.type = (Cards)int.Parse (movesArrItems ["type"].ToString ());
									move.number = int.Parse (movesArrItems ["number"].ToString ());
									move.turn = int.Parse (movesArrItems ["turn"].ToString ());
									moves.Add (move);
								}

							itemTableGame.moves = moves;
						} else if (keyItem == "first")
							itemTableGame.first = int.Parse (arr [keyItem].ToString ());
						else if (keyItem == "win")
							itemTableGame.win = int.Parse (arr [keyItem].ToString ());
					}
	            
				tableGame.Add (itemTableGame);
			}
	}

	// Read JSON Last Movement
	public void ReadJSONLastMovement(string input)
	{
	    var json = (Dictionary<string, object>)Json.Deserialize(input);
	    lastMovement = new TableGameData();

	    if (json.Count > 0)
	        foreach (object jsonItem in json.Keys)
	        {
	            var key = jsonItem.ToString();

	            if (key == "moves")
	            {
	                Dictionary<string, object> arr = null;
	                List<MoveData> moves = new List<MoveData>();

	                if (json[key] != null)
	                    arr = (Dictionary<string, object>)json[key];

	                if (arr.Count > 0)
	                    foreach (var moveItem in arr.Keys)
	                    {
	                        var keyMove = moveItem.ToString();

	                        Dictionary<string, object> movesArrItems = null;
	                        MoveData move = new MoveData();

	                        if (arr[keyMove] != null)
	                            movesArrItems = (Dictionary<string, object>)arr[keyMove];

							move.type = (Cards)int.Parse(movesArrItems["type"].ToString());
	                        move.number = int.Parse(movesArrItems["number"].ToString());
							move.turn = int.Parse(movesArrItems["turn"].ToString());
	                        moves.Add(move);
	                    }

	                lastMovement.moves = moves;
	            }
	            else if (key == "first")
	                lastMovement.first = int.Parse(json[key].ToString());
	            else if (key == "win")
	                lastMovement.win = int.Parse(json[key].ToString());
	        }
	}

	// Check Players Exsits
	private bool CheckPlayersExsits(Dictionary<string, string> result)
	{
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
			result.ContainsKey ("calculate") && result.ContainsKey ("winnerGoldCoin") && result.ContainsKey ("loserGoldCoin"))
		{
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

	    return false;
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TapsellSDK;

public class GameControl : MonoBehaviour {
	public static GameControl gameControl;
	public WindowManager windowManager;

	// Language
	[HideInInspector]
	public bool translateLanguageAsNew = false;
	[HideInInspector]
	public bool translateLanguage = false;
	[HideInInspector]
	public Dictionary<int,string> ID = new Dictionary<int,string> ();
	[HideInInspector]
	public Dictionary<int,string> IDDefault = new Dictionary<int,string> ();
	[HideInInspector]
	public Dictionary<int,string> IDIcon = new Dictionary<int,string> ();

	// Other
	[HideInInspector]
	public bool networkAccess = false;
	[HideInInspector]
	public bool networkRetry = false;
	public GameObject network;
	public GameObject loadingObject;

	// loading
	public GameObject loading;
	public Slider slider;
	private AsyncOperation async = null;

	// Information
	[HideInInspector]
	public string session;
	[HideInInspector]
	public string game;
	public string url;
	public Languages language;
	[Range(0.0f , 1.0f)]
	public float music;
	[Range(0.0f , 1.0f)]
	public float sfx;

	// Application
	public string market;
	public string version;

	// Information User
	[HideInInspector]
	public string alias;
	public string username;
	public string reagent;
	public string mobile;
	public string password;
	[HideInInspector]
	public bool signUp;
	[HideInInspector]
	public bool changeUsername;
	[HideInInspector]
	public bool changeProfile;
	[HideInInspector]
	[Range(1 , 100)]
	public int level;
	[HideInInspector]
	[Range(0.0f , 1.0f)]
	public float levelDegree;
	[HideInInspector]
	[Range(0 , 100000000)]
	public int goldCoin;
	[HideInInspector]
	[Range(0 , 100000000)]
	public int coin;
	[HideInInspector]
	[Range(1 , 82)]
	public int face;
	[HideInInspector]
	[Range(0 , 100000000)]
	public int games;
	[HideInInspector]
	[Range(0 , 100000000)]
	public int win;
	[HideInInspector]
	[Range(0 , 100000000)]
	public int lose;
	[HideInInspector]
	public string friends;
	[HideInInspector]
	public string messages;
	[HideInInspector]
	[Range(0 , 3)]
	public int oxygen;
	[HideInInspector]
	[Range(0 , 6)]
	public int oxygenStatus;
	[HideInInspector]
	[Range(0 , 7776000)]
	public int oxygenTime;

	// Game Information
	[HideInInspector]
	public string code;
	[HideInInspector]
	public string gameTypeA;
	[HideInInspector]
	public string gameTypeB;
	[HideInInspector]
	public string gameTypeC;
	[HideInInspector]
	public string gameTypeD;
	[HideInInspector]
	public int status;
	[HideInInspector]
	public string userA;
	[HideInInspector]
	public string userB;
	[HideInInspector]
	public string userC;
	[HideInInspector]
	public string userD;
	[HideInInspector]
	public int matchWinA;
	[HideInInspector]
	public int matchWinB;
	[HideInInspector]
	public int turns;
	[HideInInspector]
	[Range(1 , 4)]
	public int turn;
	[HideInInspector]
	[Range(1 , 4)]
	public int lastTurn;
	[HideInInspector]
	public int firstTurn;
	[HideInInspector]
	public string movements;
	[HideInInspector]
	public string lastMovement;
	[HideInInspector]
	public string cards;
	[HideInInspector]
	public string cardsAnimation;
	[HideInInspector]
	public string tableGame;
	[HideInInspector]
	[Range(0 , 2)]
	public int winner;
	[HideInInspector]
	[Range(1, 7)]
	public int rounds;
	[HideInInspector]
	[Range(1 , 10)]
	public int scoreTeamA;
	[HideInInspector]
	[Range(1 , 10)]
	public int scoreTeamB;
	[HideInInspector]
	[Range(1, 4)]
	public int governor;
	[HideInInspector]
	[Range(1, 4)]
	public int rule;
	[HideInInspector]
	public string aliasA;
	[HideInInspector]
	public string aliasB;
	[HideInInspector]
	public string aliasC;
	[HideInInspector]
	public string aliasD;
	[HideInInspector]
	[Range(1 , 100)]
	public int levelA;
	[HideInInspector]
	[Range(1, 100)]
	public int levelB;
	[HideInInspector]
	[Range(1, 100)]
	public int levelC;
	[HideInInspector]
	[Range(1, 100)]
	public int levelD;
	[HideInInspector]
	[Range(0.0f , 1.0f)]
	public float levelDegreeA;
	[HideInInspector]
	[Range(0.0f, 1.0f)]
	public float levelDegreeB;
	[HideInInspector]
	[Range(0.0f, 1.0f)]
	public float levelDegreeC;
	[HideInInspector]
	[Range(0.0f, 1.0f)]
	public float levelDegreeD;
	[HideInInspector]
	[Range(1 , 82)]
	public int faceA;
	[HideInInspector]
	[Range(1 , 82)]
	public int faceB;
	[HideInInspector]
	[Range(1 , 82)]
	public int faceC;
	[HideInInspector]
	[Range(1 , 82)]
	public int faceD;
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
	[HideInInspector]
	public bool calculate;
	[HideInInspector]
	public int winnerCoin;
	[HideInInspector]
	public int loserCoin;
	[HideInInspector]
	public int winnerGoldCoin;
	[HideInInspector]
	public int loserGoldCoin;

	// Other Game Information
	[HideInInspector]
	public bool showLoginMessage = false;
	[HideInInspector]
	public string previosUsername = "";
	[HideInInspector]
	public string previosPassword = "";

	// Set Dont Destory Game Control Information
	void Awake(){
		// Screen Never Sleep
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		// Load Data
		Load ();

		// Create Dont Destroy On Load
		if (gameControl == null) {
			DontDestroyOnLoad (gameObject);
			gameControl = this;
		} else if (gameControl != this) {
			Destroy (gameObject);
		}

		if (Application.internetReachability != NetworkReachability.NotReachable)
			networkAccess = true;
	}

	// Save Data to playerData.dat
	public void Save(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerData.dat");
		PlayerData data = new PlayerData ();

		// Save Information
		data.session = session;
		data.game = game;
		data.url = url;
		data.language = language;
		data.music = music;
		data.sfx = sfx;
		data.market = market;
		data.version = version;
		data.alias = alias;
		data.username = username;
		data.reagent = reagent;
		data.mobile = mobile;
		data.password = password;
		data.signUp = signUp;
		data.changeUsername = changeUsername;
		data.changeProfile = changeProfile;
		data.level = level;
		data.levelDegree = levelDegree;
		data.goldCoin = goldCoin;
		data.coin = coin;
		data.face = face;
		data.games = games;
		data.win = win;
		data.lose = lose;
		data.friends = friends;
		data.messages = messages;
		data.oxygen = oxygen;
		data.oxygenStatus = oxygenStatus;
		data.oxygenTime = oxygenTime;

		// Serialize
		bf.Serialize(file , data);
		// Close
		file.Close ();
	}

	// Load Data from playerData.dat
	public void Load(){
		if (File.Exists (Application.persistentDataPath + "/playerData.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerData.dat", FileMode.Open);
			// Deserialize
			PlayerData data = (PlayerData)bf.Deserialize (file);
			// Close
			file.Close ();

			// Load Information
			session = data.session;
			game = data.game;
			url = data.url;
			language = data.language;
			music = data.music;
			sfx = data.sfx;
			market = data.market;
			version = data.version;
			alias = data.alias;
			username = data.username;
			reagent = data.reagent;
			mobile = data.mobile;
			password = data.password;
			signUp = data.signUp;
			changeUsername = data.changeUsername;
			changeProfile = data.changeProfile;
			level = data.level;
			levelDegree = data.levelDegree;
			goldCoin = data.goldCoin;
			coin = data.coin;
			face = data.face;
			games = data.games;
			win = data.win;
			lose = data.lose;
			friends = data.friends;
			messages = data.messages;
			oxygen = data.oxygen;
			oxygenStatus = data.oxygenStatus;
			oxygenTime = data.oxygenTime;
		}
	}

	// Reset Data to playerData.dat
	public void Reset(){
		// Save Information
		session = "";
		music = 1f;
		sfx = 1f;
		alias = "";
		username = "_NEW";
		reagent = "";
		password = "_NEW";
		mobile = "";
		signUp = false;
		changeUsername = false;
		changeProfile = false;
		level = 1;
		levelDegree = 0;
		goldCoin = 0;
		coin = 0;
		face = 1;
	
		// Save File
		Save ();
	}

	// Init Data to playerData.dat
	public void Init(){
		// Save Information
		session = "";
		music = 0.5f;
		sfx = 0.5f;
		alias = "";
		username = "digarsoo";
		reagent = "";
		password = "=2g9gC<e)(y['QAc";
		mobile = "";
		signUp = true;
		changeUsername = true;
		changeProfile = true;
		level = 1;
		levelDegree = 0;
		goldCoin = 0;
		coin = 0;
		face = 1;

		// Save File
		Save ();
	}

	// Reset Game Information
	public void resetGameInformation(){
		code = "";
		gameTypeA = "";
		gameTypeB = "";
		gameTypeC = "";
		gameTypeD = "";
		status = 0;
		userA = "";
		userB = "";
		userC = "";
		userD = "";
		turns = 0;
		turn = 0;
		lastTurn = 0;
		movements = "";
		lastMovement = "";
		cards = "";
		winner = 0;
		rounds = 0;
		scoreTeamA = 0;
		scoreTeamB = 0;
		governor = 0;
		rule = 0;
		aliasA = "";
		aliasB = "";
		aliasC = "";
		aliasD = "";
		levelA = 0;
		levelB = 0;
		levelC = 0;
		levelD = 0;
		levelDegreeA = 0f;
		levelDegreeB = 0f;
		levelDegreeC = 0f;
		levelDegreeD = 0f;
		faceA = 1;
		faceB = 1;
		faceC = 1;
		faceD = 1;
		messageA = 0;
		messageB = 0;
		messageC = 0;
		messageD = 0;
		calculate = false;
		winnerCoin = 0;
		loserCoin = 0;
		winnerGoldCoin = 0;
		loserGoldCoin = 0;
	}

	// Other Actions
	void Update(){
		// Back Button
		if(Input.GetKeyDown(KeyCode.Escape)){
			int windowIndex = windowManager.GetActiveWindowIndex ();
			string windowName = windowManager.windows [windowIndex].name;

			if (SceneManager.GetActiveScene ().buildIndex == 0) {
				if (windowName != "Home" && windowName != "NewGame") {
					windowManager.Open (0);
				} else if (windowName == "Home")
					Exit ();
			}
		}

		// No Network
		if(Application.internetReachability == NetworkReachability.NotReachable && networkAccess)
			DeactiveNetwork ();

		// Loading Aniamtion
		if (networkRetry) {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 1f);
			loadingObject.transform.Rotate (Vector3.forward * -90 * (3f * Time.deltaTime));
		} else {
			loadingObject.GetComponent<Text> ().color = new Color (1f, 1f, 1f, 0f);
		}
	}

	// Deactive Network
	public void DeactiveNetwork(){
		network.SetActive (true);
		networkAccess = false;
		networkRetry = false;
		translateLanguage = true;

		network.transform.Find ("NetworkWindow").Find ("Buttons").Find ("Retry").GetComponent<Button> ().interactable = true;
	}

	// Active Network
	public void ActiveNetwork(){
		network.SetActive (false);
		networkAccess = true;
		networkRetry = false;
	}

	// Retry
	public void Retry(){
		networkRetry = true;
		network.transform.Find ("NetworkWindow").Find ("Buttons").Find ("Retry").GetComponent<Button> ().interactable = false;
	}

	// Exit
	public void Exit(){
		Application.Quit ();
	}

	// LoadScene
	public void LoadScene(string sceneName){
		StartCoroutine (LoadingScene(sceneName));
	}

	// Loading Staging
	public IEnumerator LoadingScene(string sceneName){
		loading.SetActive (true);
		async = SceneManager.LoadSceneAsync (sceneName);
		async.allowSceneActivation = false;

		// for Simulate
		//yield return new WaitForSeconds (3);

		while (async.isDone == false) {
			slider.value = async.progress;

			if (async.progress == 0.9f) {
				slider.value = 1f;
				async.allowSceneActivation = true;
				loading.SetActive (false);
			}

			translateLanguage = true;
			yield return null;
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using MiniJSON;

public class RequestManager : MonoBehaviour {
	[HideInInspector]
	public List<RequestData> requestList = new List<RequestData>();
	public static GameControl gameControl;

	public Dictionary<string,string> result;
	public delegate void Caller();

	void Awake() {
		if (GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();
	}

	void Update() {
		foreach (var item in requestList) {
			bool retry = gameControl.networkAccess || (gameControl.networkRetry && (item.name == "Check Login" || item.name == "Get Movement"));
	
			if (retry) {
				if (item.status) {
					if (!item.pause)
						item.timerInterval -= Time.deltaTime;
				}

				if (item.status && item.timerInterval < 0f) {
					item.function.Invoke ();
					item.lastExecute = Time.time;

					if (gameControl.networkAccess)
						item.timerInterval = item.timer;
					else
						item.timerInterval = 1;
				}
			}
		}
	}

	public void EnableRequest(string name){
		foreach (var item in requestList) {
			if (item.name == name) {
				item.status = true;
				item.function.Invoke ();
			}
		}
	}

	public void DisableRequest(string name){
		foreach (var item in requestList) {
			if (item.name == name) {
				item.status = false;
			}
		}
	}

	public void PauseRequest(string name){
		foreach (var item in requestList) {
			if (item.name == name) {
				item.pause = true;
			}
		}
	}

	public void PlayRequest(string name){
		foreach (var item in requestList) {
			if (item.name == name) {
				item.pause = false;
			}
		}
	}

	public bool GetStatus(string name){
		foreach (var item in requestList) {
			if (item.name == name) {
				return item.status;
			}
		}

		return false;
	}

	public Coroutine SendData(string url, WWWForm data, Caller function , string name = ""){
		string finalUrl = gameControl.url + url + "?t=" + GetUTCtime ();
		//data.AddField("session" , gameControl.session);
		data.AddField("game" , gameControl.game);
		data.AddField("username" , gameControl.username);
		data.AddField("password" , gameControl.password);
		return StartCoroutine (Send (finalUrl, data , function , name));
	}

	private string GetUTCtime(){
		System.Int32 timeStamp = (System.Int32)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
		return timeStamp.ToString();
	}

	IEnumerator Send(string url, WWWForm data, Caller function , string name = ""){
		UnityWebRequest www = UnityWebRequest.Post (url , data);

		if (name != "")
			PauseRequest (name);

		yield return www.Send ();

		if (www.isError) {
			if (name != "")
				PlayRequest (name);

			if (www.error == "Cannot resolve destination host")
				gameControl.DeactiveNetwork ();

			Debug.Log (www.error);
			yield break;
		} else if (www.isDone && www.responseCode == 200) {
			if (name != "")
				PlayRequest (name);

			string wwwResult = www.downloadHandler.text;
			// Debug.Log ("OUTPUt----->" + wwwResult);

			if (wwwResult == "") {
				Debug.Log ("Empty " + wwwResult);
				PlayRequest (name);
				yield return false;
			}
	
			var json = (Dictionary<string , object>)Json.Deserialize (wwwResult);
			result = new Dictionary<string,string> ();

			if (json.Count > 0) {
				if (gameControl.networkRetry)
					gameControl.ActiveNetwork ();

				foreach (object jsonItem in json.Keys) {
					var key = jsonItem.ToString ();
					var value = "";

					if (json [key] != null)
						value = json [key].ToString ();

					if (key != "session") {
						result.Add (key, value);
					} else {
						gameControl.session = value;
						gameControl.Save ();
					}
				}
			}

			if(name == "" || GetStatus(name) && gameControl.networkAccess)
				function ();
			yield return true;
		} else if (www.isDone) {
			Debug.Log ("Done");
			if (name != "")
				PlayRequest (name);
		}
	}
}
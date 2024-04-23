using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TapsellSDK;

public class AdvertisementManager : MonoBehaviour {
	// Tapsell
	private string tapsellCode = "gqtbfjaddlphooimckpobmjsbqtnqmrsrateshhcndearsetkpgqiasfggsplhjogngqmh";
	[HideInInspector]
	public string oxygenZoneID = "59a404034684651c44b97a9e";

	private string ID = "";
	private bool cache = false;
	private bool send = false;
	private static string advertisementID = null;
	private static bool available = false;

	void Start() {
		// Tapsell
		Tapsell.initialize (tapsellCode);
		Tapsell.setDebugMode (true);

		advertisementID = null;
		available = false;
	}

	void OnGUI(){
		if (send) {
			if (advertisementID == null)
				SendRequest ();
			else if(available)
				Debug.Log (advertisementID);
		}
	}

	// Show Oxygen Advertisement
	public void ShowOxygenAdvertisement(){
		ID = oxygenZoneID;
		cache = false;
		send = true;
	}

	// Send Request
	public void SendRequest(){
		Debug.Log ("Send");
		Tapsell.requestAd(ID , cache , 
			(TapsellResult result) => {
				// onAdAvailable
				Debug.Log("Action: onAdAvailable");
				available = true;
				advertisementID = result.adId; // store this to show the ad later
			},

			(string zoneId) => {
				// onNoAdAvailable
				Debug.Log("No Ad Available");
			},

			(TapsellError error) => {
				// onError
				Debug.Log(error.error);
			},

			(string zoneId) => {
				// onNoNetwork
				Debug.Log("No Network");
			},

			(TapsellResult result) => {
				// onExpiring
				Debug.Log("Expiring");
				// this ad is expired, you must download a new ad for this zone
				advertisementID = null;
				SendRequest();
			}
		);
	}
}
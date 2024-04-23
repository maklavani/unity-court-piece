using System;

[Serializable]
public class PlayerData {
	// Information
	public string session = "";
	public string game = "Hokm";
	public string url;
	public Languages language;
	public float music;
	public float sfx;

	// Application
	public string market;
	public string version;

	// Information User
	public string alias;
	public string username;
	public string reagent;
	public string mobile;
	public string password;
	public bool signUp = false;
	public bool changeUsername = false;
	public bool changeProfile = false;
	public int level = 1;
	public float levelDegree = 0.0f;
	public int goldCoin = 0;
	public int coin = 0;
	public int face = 1;
	public int games = 0;
	public int win = 0;
	public int lose = 0;
	public string friends;
	public string messages;
	public int oxygen = 3;
	public int oxygenStatus = 0;
	public int oxygenTime = 0;
}
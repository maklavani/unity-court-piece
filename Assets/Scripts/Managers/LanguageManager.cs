using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using MiniJSON;

public class LanguageManager : MonoBehaviour {
	public static LanguageManager languageManager = null;

	[HideInInspector]
	public Font[] fonts;
	[HideInInspector]
	public TextAsset[] files;
	public Font iconFont;
	private Font font;

	private GameControl gameControl = null;
	private Languages language;
	private Dictionary<string,string> _TEXTS = new Dictionary<string,string> ();

	void Awake(){
		// Find Game Control
		if (gameControl == null && GameObject.Find ("GameControl") != null)
			gameControl = GameObject.Find ("GameControl").GetComponent<GameControl> ();

		// Create Dont Destroy On Load
		if (languageManager == null) {
			DontDestroyOnLoad (gameObject);
			languageManager = this;
		} else if (gameControl != this) {
			Destroy (gameObject);
		}

		SetLanguage ();
	}

	void FixedUpdate(){
		if (language != gameControl.language) {
			SetLanguage ();
		} else if (gameControl.translateLanguage) {
			gameControl.translateLanguage = false;
			SetLanguage ();
		}
	}

	private void SetLanguage(){
		language = gameControl.language;

		// Check Font Exist
		if (fonts.Length >= (int)language && (int)language > 0 && fonts [(int)language - 1] != null) {
			font = fonts [(int)language - 1];
		} else {
			font = null;
		}

		// Translate Text
		TranslateText ();
		// Translate Icon
		TranslateIcon ();
	}

	private void TranslateText(){
		ReadFile ();
		var objs = GameObject.FindGameObjectsWithTag("Text");

		if (objs != null) {
			foreach (GameObject obj in objs) {
				var text = obj.GetComponent<Text> ();
				string defaultText = "";

				if (gameControl.ID.ContainsKey (obj.GetInstanceID ()) == false) {
					gameControl.ID.Add (obj.GetInstanceID (), text.text);
					gameControl.IDDefault.Add (obj.GetInstanceID (), text.text);
					defaultText = text.text;
				} else if (gameControl.ID [obj.GetInstanceID ()] != text.text) {
					defaultText = text.text;
				}

				if (gameControl.translateLanguageAsNew && gameControl.IDDefault.ContainsKey (obj.GetInstanceID ()) == true)
					defaultText = gameControl.IDDefault [obj.GetInstanceID ()];

				if (defaultText != "") {
					text.text = GetText (defaultText);

					// Set Font
					if (font != null)
						text.font = font;

					// Translate Persian
					if (Languages.Persian == language)
						text.text = PersianText.Convert (text.text);

					gameControl.ID [obj.GetInstanceID ()] = text.text;
				}
			}
		}
	}

	private void ReadFile(){
		if ((int)language > 0 && files.Length >= (int)language && files[(int)language - 1] != null) {
			var jsonString = files[(int)language - 1].text;
			var data = (Dictionary<string , object>)Json.Deserialize (jsonString);
			_TEXTS = new Dictionary<string,string> ();
			foreach (object dataItem in data.Keys) {
				var key = dataItem.ToString();
				var value = data[key].ToString();
				_TEXTS.Add (key , value);
			}
		}
	}

	private string GetText(string txt){
		if (_TEXTS.ContainsKey (txt))
			return _TEXTS [txt];
		return txt;
	}
		
	private void TranslateIcon(){
		var objs = GameObject.FindGameObjectsWithTag("Icon");

		if (objs != null) {
			foreach (GameObject obj in objs) {
				var text = obj.GetComponent<Text> ();
				string defaultText = "";

				if (gameControl.IDIcon.ContainsKey (obj.GetInstanceID ()) == false) {
					gameControl.IDIcon.Add (obj.GetInstanceID (), text.text);
					defaultText = text.text;
				} else if (gameControl.IDIcon [obj.GetInstanceID ()] != text.text)
					defaultText = text.text;

				if (defaultText != null && defaultText != "") {
					// Set Font
					if (iconFont != null)
						text.font = iconFont;

					char icon = '\ue900';
					int iconInt;

					if(int.TryParse (defaultText , out iconInt)){
						icon += (char)(iconInt);
						text.text = icon.ToString ();
						gameControl.ID [obj.GetInstanceID ()] = text.text;
					}
				}
			}
		}
	}

	// Get Text Converted
	public string GetTextConverted(string txt , bool persian = false){
		string output = "";
		if (_TEXTS.ContainsKey (txt))
			output = _TEXTS [txt];
		else
			output = txt;

		if (persian && Languages.Persian == language)
			output = PersianText.Convert (output);
		
		return output;
	}

	// Get Icon Converted
	public static string GetIconConverted(string txt){
		string output = "";
		char icon = '\ue900';
		int iconInt;

		if(int.TryParse (txt , out iconInt)){
			icon += (char)(iconInt);
			output = icon.ToString ();
		}

		return output;
	}
}
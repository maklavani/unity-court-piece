using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class PersianText {
	private static Dictionary<string, string[]> utf8Characters =  new Dictionary<string, string[]>() {
		{"آ" , new string[]{"ﺂ" , "ﺂ" , "آ"}} , {"ا" , new string[]{"ﺎ" , "ﺎ" , "ا"}} , {"ب" , new string[]{"ﺐ" , "ﺒ" , "ﺑ"}} , {"پ" , new string[]{"ﭗ" , "ﭙ" , "ﭘ"}} , 
		{"ت" , new string[]{"ﺖ" , "ﺘ" , "ﺗ"}} , {"ث" , new string[]{"ﺚ" , "ﺜ" , "ﺛ"}} , {"ج" , new string[]{"ﺞ" , "ﺠ" , "ﺟ"}} , {"چ" , new string[]{"ﭻ" , "ﭽ" , "ﭼ"}} , 
		{"ح" , new string[]{"ﺢ" , "ﺤ" , "ﺣ"}} , {"خ" , new string[]{"ﺦ" , "ﺨ" , "ﺧ"}} , {"د" , new string[]{"ﺪ" , "ﺪ" , "ﺩ"}} , {"ذ" , new string[]{"ﺬ" , "ﺬ" , "ﺫ"}} , 
		{"ر" , new string[]{"ﺮ" , "ﺮ" , "ﺭ"}} , {"ز" , new string[]{"ﺰ" , "ﺰ" , "ﺯ"}} , {"ژ" , new string[]{"ﮋ" , "ﮋ" , "ﮊ"}} , {"س" , new string[]{"ﺲ" , "ﺴ" , "ﺳ"}} , 
		{"ش" , new string[]{"ﺶ" , "ﺸ" , "ﺷ"}} , {"ص" , new string[]{"ﺺ" , "ﺼ" , "ﺻ"}} , {"ض" , new string[]{"ﺾ" , "ﻀ" , "ﺿ"}} , {"ط" , new string[]{"ﻂ" , "ﻄ" , "ﻃ"}} , 
		{"ظ" , new string[]{"ﻆ" , "ﻈ" , "ﻇ"}} , {"ع" , new string[]{"ﻊ" , "ﻌ" , "ﻋ"}} , {"غ" , new string[]{"ﻎ" , "ﻐ" , "ﻏ"}} , {"ف" , new string[]{"ﻒ" , "ﻔ" , "ﻓ"}} , 
		{"ق" , new string[]{"ﻖ" , "ﻘ" , "ﻗ"}} , {"ک" , new string[]{"ﻚ" , "ﻜ" , "ﻛ"}} , {"گ" , new string[]{"ﮓ" , "ﮕ" , "ﮔ"}} , {"ل" , new string[]{"ﻞ" , "ﻠ" , "ﻟ"}} , 
		{"م" , new string[]{"ﻢ" , "ﻤ" , "ﻣ"}} , {"ن" , new string[]{"ﻦ" , "ﻨ" , "ﻧ"}} , {"و" , new string[]{"ﻮ" , "ﻮ" , "ﻭ"}} , {"ی" , new string[]{"ﯽ" , "ﯿ" , "ﯾ"}} , 
		{"ك" , new string[]{"ﻚ" , "ﻜ" , "ﻛ"}} , {"ي" , new string[]{"ﻲ" , "ﻴ" , "ﻳ"}} , {"أ" , new string[]{"ﺄ" , "ﺄ" , "ﺃ"}} , {"ؤ" , new string[]{"ﺆ" , "ﺆ" , "ﺅ"}} , 
		{"إ" , new string[]{"ﺈ" , "ﺈ" , "ﺇ"}} , {"ئ" , new string[]{"ﺊ" , "ﺌ" , "ﺋ"}} , {"ة" , new string[]{"ﺔ" , "ﺘ" , "ﺗ"}}};

	private static Dictionary<string, string[]> tahoma = new Dictionary<string, string[]>() {{"ه" , new string[]{"ﻪ" , "ﮭ" , "ﮬ"}}};
	private static Dictionary<string, string[]> normal = new Dictionary<string, string[]>() {{"ه" , new string[]{"ﻪ" , "ﻬ" , "ﻫ"}}};
	private static string[] numbers = new string[]{"٠" , "١" , "٢" , "٣" , "٤" , "٥" , "٦" , "٧" , "٨" , "٩" , "۴" , "۵" , "۶"};
	private static string[] nextIgnoreList = new string[]{"آ" , "ا" , "د" , "ذ" , "ر" , "ز" , "ژ" , "و" , "أ" , "إ" , "ؤ"};
	private static string[] alamatList = new string[]{"ٌ" , "ٍ" , "ً" , "ُ" , "ِ" , "َ" , "ّ" , "ٓ" , "ٰ" , "ٔ" , "ﹶ" , "ﹺ" , "ﹸ" , "ﹼ" , "ﹾ" , "ﹴ" , "ﹰ" , "ﱞ" , "ﱟ" , "ﱠ" , "ﱡ" , "ﱢ" , "ﱣ"};
	private static string[] openClose = new string[]{">" , ")" , "}" , "]" , "<" , "(" , "{" , "["};
	private static string[] openCloseReverse = new string[]{"<" , "(" , "{" , "[" , ">" , ")" , "}" , "]"};

	public static string Convert(string txt , string method = "normal" , bool convertNumbers = true , string language = "persian"){
		string output = "";

		// Method	
		if (method == "tahoma") {
			foreach (var item in tahoma) {
				utf8Characters [item.Key] = item.Value;
			}
		} else {
			foreach (var item in normal) {
				utf8Characters[item.Key] = item.Value;
			}
		}

		// Allah
		txt = txt.Replace("الله" , "اللّه");

		// Init
		int len = txt.Length;
		string prevCharacter = null;
		string nextCharacter = null;

		// Revers Number
		txt = ReverseNumberInString(txt);

		for (var i = len - 1; i >= 0; i--) {
			prevCharacter = null;

			if(i > 0 && (utf8Characters.ContainsKey(txt[i - 1].ToString()) || InArray(alamatList , txt[i - 1].ToString())))
				prevCharacter = txt[i - 1].ToString();

			if(InArray(nextIgnoreList , prevCharacter))
				prevCharacter = null;

			if(InArray(openClose , txt[i].ToString()))
			{
				output = openCloseReverse [GetIndexInArray (openClose, txt [i].ToString ())];
				nextCharacter = null;
			}
			else
			{
				if(utf8Characters.ContainsKey(txt[i].ToString()))
				{
					if(nextCharacter == null && prevCharacter == null){
						output += txt[i].ToString();
						nextCharacter = txt[i].ToString();
					} else {
						int ind = 1;

						if (prevCharacter == null) {
							ind = 2;
						} else if (nextCharacter == null) {
							ind = 0;
						}

						output += utf8Characters[txt[i].ToString()][ind];
						nextCharacter = utf8Characters[txt[i].ToString()][ind];
					}
				} else if(convertNumbers && InArray(new string[]{"0" , "1" , "2" , "3" , "4" , "5" , "6" , "7" , "8" , "9"} , txt[i].ToString())) {
					int ind = 0;
					int.TryParse (txt [i].ToString (), out ind);

					if(language == "arabic")
						output += numbers[ind];
					else
					{
						if(InArray(new string[]{"4" , "5" , "6"} , txt[i].ToString()))
							output += numbers[ind + 6];
						else
							output += numbers[ind];
					}

					nextCharacter = null;
				} else {
					output += txt[i].ToString();
					nextCharacter = txt[i].ToString();

					if(!InArray(alamatList , txt[i].ToString()))
						nextCharacter = null;
				}
			}
		}

		return output;
	}

	private static bool InArray(string[] stringArray , string stringToCheck){
		foreach (string str in stringArray) {
			if (str.Equals (stringToCheck)) {
				return true;
			}
		}

		return false;
	}

	private static int GetIndexInArray(string[] stringArray , string stringToCheck){
		int i = 0;
		foreach (string str in stringArray) {
			if (str.Equals (stringToCheck)) {
				return i;
			}
			i++;
		}

		return -1;
	}

	private static string ReverseNumberInString(string stringToCheck){
		Regex r = new Regex(@"[0-9\.\,]+" , RegexOptions.RightToLeft);
		MatchCollection matches = r.Matches(stringToCheck);

		if (matches.Count > 0)
			foreach (Match match in matches) {
				string find = match.Groups [0].Value;
				stringToCheck = stringToCheck.Replace (find , Reverse(find));
			}

		return stringToCheck;
	}

	private static string Reverse(string stringToCheck)
	{
		char[] charArray = stringToCheck.ToCharArray();
		System.Array.Reverse(charArray);
		return new string(charArray);
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(GameControl))]
public class GameControlEditor : Editor {
	public override void OnInspectorGUI (){
		DrawDefaultInspector ();

		if(GUILayout.Button("Save Data to playerData.dat")){
			((GameControl)target).Save ();
			Debug.Log ("Saved Data!");
		}

		if(GUILayout.Button("Load Data from playerData.dat")){
			((GameControl)target).Load ();
			Debug.Log ("Loaded Data!");
		}

		if(GUILayout.Button("Reset Data to playerData.dat")){
			((GameControl)target).Reset ();
			Debug.Log ("Reseted Data!");
		}

		if(GUILayout.Button("Init Data to playerData.dat")){
			((GameControl)target).Init ();
			Debug.Log ("Init Data!");
		}

		if(GUILayout.Button("Location")){
			Debug.Log (Application.persistentDataPath + "/playerData.dat");
		}
	}
}
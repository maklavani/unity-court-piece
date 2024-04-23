using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(RequestManager))]
public class RequestManagerEditor : Editor {
	private ReorderableList list;

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		list.DoLayoutList ();
		serializedObject.ApplyModifiedProperties ();
	}

	private void OnEnable(){
		list = new ReorderableList (serializedObject, serializedObject.FindProperty ("requestList"), true, true, true, true);
		//list.elementHeight = EditorGUIUtility.singleLineHeight * 2f;
		list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect , "Request List"); };

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			var style = new GUIStyle();
			string str = "";

			if(element.FindPropertyRelative("pause").boolValue == true){
				style.normal.textColor = Color.yellow;
				str = "Paused";
			} else if(element.FindPropertyRelative("status").boolValue == false){
				style.normal.textColor = Color.red;
				str = "Disabled";
			} else if(element.FindPropertyRelative("timerInterval").floatValue + 2f >= element.FindPropertyRelative("timer").floatValue && element.FindPropertyRelative("lastExecute").floatValue != 0f){
				style.normal.textColor = Color.cyan;
				str = (element.FindPropertyRelative("timerInterval").floatValue).ToString("F6") + " | Run";
			} else if(element.FindPropertyRelative("timerInterval").floatValue == 0f){
				style.normal.textColor = Color.green;
				str = "Enabled";
			} else {
				style.normal.textColor = Color.white;
				str = (element.FindPropertyRelative("timerInterval").floatValue).ToString("F6");
			}

			EditorGUI.LabelField(new Rect(rect.x , rect.y, 120 - 2, EditorGUIUtility.singleLineHeight) , str , style);

			EditorGUI.PropertyField(new Rect(rect.x + 120, rect.y, 20, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("status"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + 140, rect.y, rect.width - 190 - 2 , EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("name"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + rect.width - 50 , rect.y, 50 , EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("timer"), GUIContent.none);

			SerializedProperty sp = element.FindPropertyRelative("function");
			EditorGUILayout.LabelField(element.FindPropertyRelative("name").stringValue);
			EditorGUILayout.PropertyField(sp);
		};

		list.onAddCallback = (ReorderableList items) => {
			var index = items.serializedProperty.arraySize;
			items.serializedProperty.arraySize++;
			items.index = index;
			var element = items.serializedProperty.GetArrayElementAtIndex(index);

			element.FindPropertyRelative("name").stringValue = "";
			element.FindPropertyRelative("status").boolValue = false;
			element.FindPropertyRelative("timer").floatValue = 1000f;
			element.FindPropertyRelative("timerInterval").floatValue = 0f;
			element.FindPropertyRelative("lastExecute").floatValue = 0f;
			element.FindPropertyRelative("pause").boolValue = false;
		};


		list.onCanRemoveCallback = (ReorderableList l) => { return l.count > 0; };


		list.onRemoveCallback = (ReorderableList l) => {
			if (EditorUtility.DisplayDialog("Warning!" , "Are you sure you want to delete the Request?" , "Yes" , "No")){
				ReorderableList.defaultBehaviours.DoRemoveButton(l);
			}
		};
	}
}
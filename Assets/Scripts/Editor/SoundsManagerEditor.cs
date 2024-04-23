using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(SoundsManager))]
public class SoundsManagerEditor : Editor {
	private ReorderableList list;

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		list.DoLayoutList ();
		serializedObject.ApplyModifiedProperties ();
	}

	private void OnEnable(){
		list = new ReorderableList (serializedObject, serializedObject.FindProperty ("sounds"), true, true, true, true);
		//list.elementHeight = EditorGUIUtility.singleLineHeight * 2f;
		list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect , "Sounds"); };

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			var style = new GUIStyle();
			string str = "";


			EditorGUI.PropertyField(new Rect(rect.x , rect.y, 80 - 4 , EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("name"), GUIContent.none);

			str = "Status";

			if(element.FindPropertyRelative("status").boolValue == true)
				style.normal.textColor = Color.green;
			else
				style.normal.textColor = Color.red;

			EditorGUI.LabelField(new Rect(rect.x + 80 , rect.y, 50 - 2 , EditorGUIUtility.singleLineHeight), str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 130 , rect.y - 2 , 20 - 2 , EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("status"), GUIContent.none);

			str = "Repeat";

			if(element.FindPropertyRelative("loop").boolValue == true)
				style.normal.textColor = Color.green;
			else
				style.normal.textColor = Color.red;

			EditorGUI.LabelField(new Rect(rect.x + 150 , rect.y, 50 - 2 , EditorGUIUtility.singleLineHeight), str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 200 , rect.y - 2 , 20 - 2 , EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("loop"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + 220 , rect.y, rect.width - 220 , EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("clip"), GUIContent.none);
		};

		list.onAddCallback = (ReorderableList items) => {
			var index = items.serializedProperty.arraySize;
			items.serializedProperty.arraySize++;
			items.index = index;
			var element = items.serializedProperty.GetArrayElementAtIndex(index);

			element.FindPropertyRelative("name").stringValue = "";
			element.FindPropertyRelative("status").boolValue = false;
			element.FindPropertyRelative("loop").boolValue = false;
		};


		list.onCanRemoveCallback = (ReorderableList l) => { return l.count > 0; };


		list.onRemoveCallback = (ReorderableList l) => {
			if (EditorUtility.DisplayDialog("Warning!" , "Are you sure you want to delete the Audio?" , "Yes" , "No")){
				ReorderableList.defaultBehaviours.DoRemoveButton(l);
			}
		};
	}
}
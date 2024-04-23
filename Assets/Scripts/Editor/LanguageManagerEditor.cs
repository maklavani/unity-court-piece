using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(LanguageManager))]
public class LanguageManagerEditor : Editor {
	private ReorderableList fontsList;
	private ReorderableList filesList;

	public override void OnInspectorGUI (){
		DrawDefaultInspector ();
		fontsList.DoLayoutList ();
		filesList.DoLayoutList ();
		serializedObject.ApplyModifiedProperties ();
	}

	private void OnEnable(){
		fontsList = new ReorderableList (serializedObject, serializedObject.FindProperty ("fonts"), true, true, true, true);
		filesList = new ReorderableList (serializedObject, serializedObject.FindProperty ("files"), true, true, true, true);
		fontsList.drawHeaderCallback  = (Rect rect) => { EditorGUI.LabelField(rect , "Fonts"); };
		filesList.drawHeaderCallback  = (Rect rect) => { EditorGUI.LabelField(rect , "Files"); };

		fontsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = fontsList.serializedProperty.GetArrayElementAtIndex(index);
			EditorGUI.PropertyField(new Rect(rect.x , rect.y , Screen.width - 75 , EditorGUIUtility.singleLineHeight) , element , new GUIContent(((Languages)(index + 1)).ToString()));
		};

		filesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = filesList.serializedProperty.GetArrayElementAtIndex(index);
			EditorGUI.PropertyField(new Rect(rect.x , rect.y , Screen.width - 75 , EditorGUIUtility.singleLineHeight) , element , new GUIContent(((Languages)(index + 1)).ToString()));
		};
	}
}
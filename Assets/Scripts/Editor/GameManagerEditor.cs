using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {
	private ReorderableList list;
	private ReorderableList listB;

	public override void OnInspectorGUI (){
		DrawDefaultInspector ();
		list.DoLayoutList ();
		listB.DoLayoutList ();
		serializedObject.ApplyModifiedProperties ();
	}

	private void OnEnable(){
		list = new ReorderableList (serializedObject, serializedObject.FindProperty ("users"), true, true, true, true);
		listB = new ReorderableList (serializedObject, serializedObject.FindProperty ("timer"), true, true, true, true);
		list.drawHeaderCallback  = (Rect rect) => { EditorGUI.LabelField(rect , "Users"); };
		listB.drawHeaderCallback  = (Rect rect) => { EditorGUI.LabelField(rect , "Timer"); };

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			EditorGUI.PropertyField(new Rect(rect.x , rect.y , Screen.width - 75 , EditorGUIUtility.singleLineHeight) , element , GUIContent.none);
		};

		listB.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = listB.serializedProperty.GetArrayElementAtIndex(index);
			EditorGUI.PropertyField(new Rect(rect.x , rect.y , Screen.width - 75 , EditorGUIUtility.singleLineHeight) , element , GUIContent.none);
		};
	}
}
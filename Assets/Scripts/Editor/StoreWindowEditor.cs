using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(StoreWindow))]
public class StoreWindowEditor : Editor {
	private ReorderableList list;

	public override void OnInspectorGUI (){
		DrawDefaultInspector ();
		list.DoLayoutList ();
		serializedObject.ApplyModifiedProperties ();
	}

	private void OnEnable(){
		list = new ReorderableList (serializedObject, serializedObject.FindProperty ("messagesObject"), true, true, true, true);
		list.drawHeaderCallback  = (Rect rect) => { EditorGUI.LabelField(rect , "Messages Object"); };

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			EditorGUI.PropertyField(new Rect(rect.x , rect.y , Screen.width - 75 , EditorGUIUtility.singleLineHeight) , element , GUIContent.none);
		};
	}
}
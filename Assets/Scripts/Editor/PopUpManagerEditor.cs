using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(PopUpManager))]
public class PopUpManagerEditor : Editor {
	private ReorderableList list;

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		list.DoLayoutList ();
		serializedObject.ApplyModifiedProperties ();
	}

	private void OnEnable(){
		list = new ReorderableList (serializedObject, serializedObject.FindProperty ("windows"), true, true, true, true);
		//list.elementHeight = EditorGUIUtility.singleLineHeight * 2f;
		list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect , "Windows"); };

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);

			EditorGUI.PropertyField(new Rect(rect.x , rect.y, 100 - 2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("name"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + 100, rect.y, rect.width - 100 , EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("go"), GUIContent.none);

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
		};


		list.onCanRemoveCallback = (ReorderableList l) => { return l.count > 0; };


		list.onRemoveCallback = (ReorderableList l) => {
			if (EditorUtility.DisplayDialog("Warning!" , "Are you sure you want to delete the Request?" , "Yes" , "No")){
				ReorderableList.defaultBehaviours.DoRemoveButton(l);
			}
		};
	}
}
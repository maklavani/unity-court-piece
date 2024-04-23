using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Text;

[CustomEditor(typeof(WindowManager))]
public class WindowManagerEditor : Editor {
	private ReorderableList list;

	public override void OnInspectorGUI (){
		DrawDefaultInspector ();
		list.DoLayoutList ();
		serializedObject.ApplyModifiedProperties ();

		if (GUILayout.Button ("Generate Window Enums")) {
			var windows = ((WindowManager)target).windows;
			var total = windows.Length;

			var sb = new StringBuilder();
			sb.Append ("public enum Windows {\n");
			sb.Append ("\tNone = 0,\n");

			for (var i = 0; i < total; i++) {
				sb.Append ("\t");
				sb.Append (windows [i].name.Replace(" ", ""));
				sb.Append (" = " + (i + 1));
				if (i < total - 1)
					sb.Append (",\n");
			}

			sb.Append ("\n};");

			var path = EditorUtility.SaveFilePanel ("Save The Window Enums" , "" , "WindowEnums.cs" , "cs");

			using (FileStream fs = new FileStream (path, FileMode.Create)) {
				using (StreamWriter writer = new StreamWriter (fs)) {
					writer.Write (sb.ToString ());
				};
			};

			AssetDatabase.Refresh ();
		}
	}

	private void OnEnable(){
		list = new ReorderableList (serializedObject, serializedObject.FindProperty ("windows"), true, true, true, true);

		list.drawHeaderCallback  = (Rect rect) => {
			EditorGUI.LabelField(rect , "Windows");
		};

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			EditorGUI.PropertyField(new Rect(rect.x , rect.y , Screen.width - 75 , EditorGUIUtility.singleLineHeight) , element , GUIContent.none);
		};
	}
}
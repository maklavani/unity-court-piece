using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(CardManager))]
public class CardManagerEditor : Editor {

    private ReorderableList list;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("spriteList"), true, true, true, true);
        //list.elementHeight = EditorGUIUtility.singleLineHeight * 2f;
        list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Sprite List"); };
        
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);

            // element.FindPropertyRelative("number").intValue = (index + 1) - ((index + 1) > 13 ? (int)((index) / 13) * 13 : 0);

            /*
              if (index < 13)
                  element.FindPropertyRelative("type").enumValueIndex = 0;
              else if (index < 26)
                  element.FindPropertyRelative("type").enumValueIndex = 1;
              else if (index < 39)
                  element.FindPropertyRelative("type").enumValueIndex = 2;
              else if (index < 52)
                  element.FindPropertyRelative("type").enumValueIndex = 3;
            */

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 30 - 2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("number"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 30, rect.y, 60 - 2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("type"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + 90, rect.y, rect.width - 90, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("image"), GUIContent.none);
        };

        list.onCanRemoveCallback = (ReorderableList l) => { return l.count > 0; };

        list.onRemoveCallback = (ReorderableList l) => {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the Card?", "Yes", "No"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
            }
        };
    }
}
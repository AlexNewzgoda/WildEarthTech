using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InputSettingsActionChanger))]
public class InputSettingsActionChanger_Inspector : Editor
{
    string[] ActionsList;
    int selected = -1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        InputSettingsActionChanger actionChanger = (InputSettingsActionChanger)target;

        serializedObject.Update();

        SerializedProperty ActionName = serializedObject.FindProperty("ActionName");

        System.Reflection.FieldInfo[] fields = typeof(InputSettings).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        if (ActionsList == null || ActionsList.Length != fields.Length)
            ActionsList = new string[fields.Length];

        if (fields != null)
        {
            for(int q = 0; q < fields.Length; q++)
            {
                if (fields[q].FieldType == typeof(KeyCode))
                {
                    string name = fields[q].Name.Replace("key_", "");

                    ActionsList[q] = name;
                }
            }
        }

        selected = -1;
        selected = EditorGUILayout.Popup("Select Action", selected, ActionsList);

        if (selected != -1)
        {
            ActionName.stringValue = ActionsList[selected];
        }

        serializedObject.ApplyModifiedProperties();
    }
}

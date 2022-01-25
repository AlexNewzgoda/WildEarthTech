using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Entity))]
public class Entity_Inspector : Editor
{
    private bool IsPrefab()
    {
        return EntityHelper.IsPrefab(((Entity)target).gameObject);
    }

    private string _newID = "";

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Entity prop = target as Entity;

        SerializedProperty ID = serializedObject.FindProperty("ID");

        SerializedProperty prefabReference = serializedObject.FindProperty("PrefabReference");
        EditorGUILayout.PropertyField(prefabReference);

        GUILayout.Space(10);
        
        //ID view----------------------------------------
        GUILayout.BeginVertical("HelpBox");

        GUILayout.BeginHorizontal();
        if (!IsPrefab() && GUILayout.Button("Init Scene ID"))
        {
            ID.stringValue = EntityHelper.GetNewID();
            serializedObject.ApplyModifiedProperties();
        }
        if (GUILayout.Button("Reset ID"))
        {
            ID.stringValue = null;
            serializedObject.ApplyModifiedProperties();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");

        GUI.skin.label.richText = true;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        string content = "ID: " + ((prop.ID == null) ? "<color=red>Unassigned</color>" : "<color=#8888ff>" + prop.ID.ToString() + "</color>");    
        GUILayout.Label(content);

        if(GUILayout.Button("Copy", GUILayout.Width(50)))
        {
            EditorGUIUtility.systemCopyBuffer = prop.ID;
        }

        GUILayout.EndHorizontal();

        _newID = EditorGUILayout.TextField(_newID);

        if (GUILayout.Button("Set ID"))
        {
            ID.stringValue = _newID;
            serializedObject.ApplyModifiedProperties();
        }

        GUI.skin.label.fontStyle = FontStyle.Normal;
        //ID view----------------------------------------

        GUILayout.EndVertical();

        GUILayout.Space(10);

        SerializedProperty SerializableComponents = serializedObject.FindProperty("SerializableComponents");

        EditorGUILayout.PropertyField(SerializableComponents);

        /*SerializableComponents.arraySize = EditorGUILayout.IntField(SerializableComponents.arraySize);

        for(int q = 0; q < SerializableComponents.arraySize; q++)
        {
            SerializedProperty scObjProperty = SerializableComponents.GetArrayElementAtIndex(q);
            EditorGUILayout.ObjectField(scObjProperty);

            if(scObjProperty.objectReferenceValue is ISaveSerialized || scObjProperty.objectReferenceValue is Rigidbody)
            {

            }
            else
            {
                scObjProperty.objectReferenceValue = null;
            }
        }*/

        serializedObject.ApplyModifiedProperties();
    }
}

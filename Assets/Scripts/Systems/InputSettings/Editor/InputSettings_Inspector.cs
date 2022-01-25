using UnityEditor;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(InputSettings))]
public class InputSettings_Inspector : Editor
{
    private string _keyName = "";
    private KeyCode _keyValue = KeyCode.None;

    private string _valueName = "";
    private float _valueValue = 0;

    public override void OnInspectorGUI()
    {
        InputSettings i = (InputSettings)target;

        GUILayout.Label("Buttons");
        EditorGUILayout.BeginVertical("box");
        foreach (FieldInfo p in i.GetType().GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (p.FieldType == typeof(KeyCode))
            {
                EditorGUILayout.BeginHorizontal("box", GUILayout.Width(250));

                EditorGUILayout.LabelField(p.Name.Replace("key_", ""), GUILayout.Width(150));

                if (GUILayout.Button(p.GetValue(i).ToString(), "HelpBox", GUILayout.Width(100)))
                {
                    _keyName = p.Name.Replace("key_", "");
                }

                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    DeleteKeyString(p.Name);
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();

        GUILayout.Label("Values");
        EditorGUILayout.BeginVertical("box");
        foreach (FieldInfo p in i.GetType().GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (p.FieldType == typeof(float))
            {
                EditorGUILayout.BeginHorizontal("box", GUILayout.Width(250));

                EditorGUILayout.LabelField(p.Name.Replace("val_", ""), GUILayout.Width(150));

                if (GUILayout.Button(p.GetValue(i).ToString(), "HelpBox", GUILayout.Width(100)))
                {
                    _valueName = p.Name.Replace("val_", "");
                }

                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    DeleteValueString(p.Name);
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();

        GUILayout.BeginVertical("Keys Editor", "Window");
        _keyName = EditorGUILayout.TextField("Key Name: ", _keyName);
        _keyName = _keyName.Replace(" ", "");
        _keyValue = (KeyCode)EditorGUILayout.EnumPopup("Default Key: ", _keyValue);
        if (GUILayout.Button("Add Key"))
        {
            GenerateKeyStrings(_keyName,_keyValue);
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical("Values Editor", "Window");
        _valueName = EditorGUILayout.TextField("Value Name: ", _valueName);
        _valueName = _valueName.Replace(" ", "");
        _valueValue = EditorGUILayout.FloatField("Default Value: ", _valueValue);
        if (GUILayout.Button("Add Value"))
        {
            GenerateValueStrings(_valueName, _valueValue);
        }
        GUILayout.EndVertical();
    }

    public void GenerateValueStrings (string valueName, float defaultValue)
    {
        string filePath;
        if (!GetCSFilePath(out filePath))
            return;

        string[] data = File.ReadAllLines(filePath);
        List<string> fileContent = new List<string>(data.Length);
        foreach (string s in data)
        {
            fileContent.Add(s);
        }

        int findedStringIndx = FindStringIndx($"public static float val_{valueName}", fileContent);

        string declareValueString = $"    public static float val_{valueName} = {defaultValue};";
        string declareLoadValueString = $"        val_{valueName} = GetValue(\"{valueName}\", {defaultValue}, true);";

        if (findedStringIndx == -1)
        {
            int endIndex = fileContent.FindIndex((x) => x.Contains("//ValuesEnd"));

            if (endIndex == -1)
                return;

            fileContent.Insert(endIndex, declareValueString);

            endIndex = fileContent.FindIndex((x) => x.Contains("//ValuesEndLoad"));

            if (endIndex == -1)
                return;

            fileContent.Insert(endIndex, declareLoadValueString);
        }
        else
        {
            fileContent[findedStringIndx] = declareValueString;

            findedStringIndx = FindStringIndx($"val_{valueName} = GetKeyValue(", fileContent);

            if (findedStringIndx != -1)
                fileContent[findedStringIndx] = declareLoadValueString;
        }

        File.WriteAllLines(filePath, fileContent);

        AssetDatabase.Refresh();
    }

    public void DeleteExistValue(string valueName, List<string> fileContent)
    {
        string varLine = $"public static float {valueName}";

        fileContent.RemoveAll((x) => x.Contains(varLine));

        string varDefaultKeyLine = $"{valueName} = GetValue(";

        fileContent.RemoveAll((x) => x.Contains(varDefaultKeyLine));
    }

    public void DeleteValueString(string valueName)
    {
        string filePath;
        if (!GetCSFilePath(out filePath))
            return;

        string[] data = File.ReadAllLines(filePath);
        List<string> fileContent = new List<string>(data.Length);
        foreach (string s in data)
        {
            fileContent.Add(s);
        }

        DeleteExistValue(valueName, fileContent);

        File.WriteAllLines(filePath, fileContent);

        AssetDatabase.Refresh();
    }

    public void GenerateKeyStrings (string keyName, KeyCode defaultKey)
    {
        string filePath;
        if (!GetCSFilePath(out filePath))
            return;

        string[] data = File.ReadAllLines(filePath);
        List<string> fileContent = new List<string>(data.Length);
        foreach (string s in data)
        {
            fileContent.Add(s);
        }

        int findedStringIndx = FindStringIndx($"public static KeyCode key_{keyName}", fileContent);

        string declareKeyString = $"    public static KeyCode key_{keyName} = KeyCode.{defaultKey};";
        string declareLoadKeyString = $"        key_{keyName} = GetKeyValue(\"{keyName}\", KeyCode.{defaultKey}, true);";

        if (findedStringIndx == -1)
        {
            int endIndex = fileContent.FindIndex((x) => x.Contains("//KeysEnd"));

            if (endIndex == -1)
                return;

            fileContent.Insert(endIndex, declareKeyString);

            endIndex = fileContent.FindIndex((x) => x.Contains("//KeysEndLoad"));

            if (endIndex == -1)
                return;

            fileContent.Insert(endIndex, declareLoadKeyString);
        }
        else
        {
            fileContent[findedStringIndx] = declareKeyString;

            findedStringIndx = FindStringIndx($"key_{keyName} = GetKeyValue(", fileContent);

            if(findedStringIndx != -1)
                fileContent[findedStringIndx] = declareLoadKeyString;
        }

        File.WriteAllLines(filePath, fileContent);

        AssetDatabase.Refresh();
    }

    public int FindStringIndx (string str, List<string> fileContent)
    {
        return fileContent.FindIndex((x) => x.Contains(str));
    }

    public void DeleteExistKey (string keyName, ref List<string> fileContent)
    {
        string varLine = $"public static KeyCode {keyName}";

        fileContent.RemoveAll((x) => x.Contains(varLine));

        string varDefaultKeyLine = $"{keyName} = GetKeyValue(";

        fileContent.RemoveAll((x) => x.Contains(varDefaultKeyLine));
    }

    public void DeleteKeyString (string keyName)
    {
        string filePath;
        if (!GetCSFilePath(out filePath))
            return;

        string[] data = File.ReadAllLines(filePath);
        List<string> fileContent = new List<string>(data.Length);
        foreach(string s in data)
        {
            fileContent.Add(s);
        }

        DeleteExistKey(keyName, ref fileContent);

        File.WriteAllLines(filePath, fileContent);

        AssetDatabase.Refresh();
    }

    public bool GetCSFilePath (out string filePath)
    {
        filePath = Application.dataPath.Replace("Assets", "");
        bool findedFile = false;
        foreach (string script in AssetDatabase.FindAssets("InputSettings t:script"))
        {
            string file = AssetDatabase.GUIDToAssetPath(script);
            if (file.Contains("InputSettings.cs"))
            {
                filePath += file;
                findedFile = true;
                break;
            }
        }
        return findedFile;
    }
}
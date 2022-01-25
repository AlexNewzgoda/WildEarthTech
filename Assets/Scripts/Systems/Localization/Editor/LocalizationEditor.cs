using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizationEditor : EditorWindow
{
    [MenuItem("Tools/Localization Editor")]
    public static void ShowEditorWindow ()
    {
        LocalizationEditor localizationEditor = new LocalizationEditor();
        localizationEditor.Show();
        localizationEditor.minSize = new Vector2(800, 850);
        localizationEditor.maxSize = new Vector2(800, 850);
        localizationEditor.Init();
    }

    public string[] LangKeysList = null;

    public string CurrentFile;
    public string CurrentFileName;
    public string CurrentFileData;
    public LocalizationManager.KeyValStack CurrentFileStructedData;

    [System.Serializable]
    public class LocalizationFile
    {
        public string FilePath;
        public string LangKey;
        public LocalizationManager.KeyValStack Data;
    }
    public List<LocalizationFile> LocalizationFiles;

    private string LocalizationFolderPath = ".\\Localization";

    public void Init ()
    {
        if(!Directory.Exists(LocalizationFolderPath))
        {
            EditorUtility.DisplayDialog("Error", "Localization folder not found", "ok");
            this.Close();
        }
    }

    private Vector2 scroll = Vector2.zero;
    private Vector2 scroll2 = Vector2.zero;

    private string _findKeyStr = "";
    private string _findValueStr = "";
    private int _findedIndx = -1;

    private void OnGUI()
    {
        GUILayout.BeginVertical("HelpBox");

        if (GUILayout.Button("Add new Lang"))
        {
            string langKeysFile = LocalizationFolderPath + "\\language_keys.txt";
            if(File.Exists(langKeysFile))
            {
                LangKeysList = File.ReadAllLines(langKeysFile);
            }
        }

        if(LangKeysList != null)
        {
            GUI.color = Color.red;
            if(GUILayout.Button("X", GUILayout.Width(20)))
            {
                LangKeysList = null;
                return;
            }
            GUI.color = Color.white;

            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(256));

            foreach (string key in LangKeysList)
            {
                if(GUILayout.Button(key))
                {
                    string filePath = LocalizationFolderPath + "\\" + key + ".lang";
                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath).Close();
                        ClickOpen();
                        LangKeysList = null;
                        return;
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        GUILayout.EndVertical();

        GUILayout.Space(20);

        GUILayout.BeginVertical("File", "Window", GUILayout.Height(50));

        if (GUILayout.Button("OpenFile"))
        {
            ClickOpen();
        }

        if (string.IsNullOrEmpty(CurrentFile))
        {
            GUILayout.EndVertical();
            return;
        }

        GUILayout.Label($"Current file: {CurrentFileName}");

        GUILayout.EndVertical();

        GUILayout.Space(20);

        if (CurrentFileStructedData != null && CurrentFileStructedData.keyVals != null)
        {
            int langStringSize = LocalizationFiles.Count;
            float elementSize = 50;

            float langValueSize = 60;
            float elementSizeScroll = langValueSize * langStringSize + 12;

            //Find-------------------------------------------------------------------------------------
            GUILayout.BeginVertical("Find items", "Window", GUILayout.Height(100));

            //Key Find
            GUILayout.BeginHorizontal("Box");
            _findKeyStr = GUILayout.TextField(_findKeyStr);
            if (GUILayout.Button("Key Find", GUILayout.Width(256)))
            {
                _findedIndx = CurrentFileStructedData.keyVals.FindIndex((x) => x.Key == _findKeyStr);
            }
            GUILayout.EndHorizontal();

            //Value Find
            GUILayout.BeginHorizontal("Box");
            _findValueStr = GUILayout.TextField(_findValueStr);
            if (GUILayout.Button("Value Find", GUILayout.Width(256)))
            {
                foreach(LocalizationFile lf in LocalizationFiles)
                {
                    _findedIndx = lf.Data.keyVals.FindIndex((x) => x.Value.Contains(_findValueStr));

                    if (_findedIndx != -1)
                        break;
                }
            }
            GUILayout.EndHorizontal();

            if (_findedIndx != -1)
                GUILayout.Label("Finded in ItemID: " + _findedIndx, "HelpBox");


            GUILayout.EndVertical();
            //--------------------------------------------------------------------------------------------
            
            GUILayout.Space(20);

            GUILayout.BeginVertical("Items", "Window");

            if (GUILayout.Button("Save"))
            {
                SaveCurrentFile();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Count: " + CurrentFileStructedData.keyVals.Count, GUILayout.Width(100));
            if (GUILayout.Button("Add"))
            {
                CurrentFileStructedData.keyVals.Add(new LocalizationManager.KeyVal() { Key = "null", Value = "null" });
                for (int l = 0; l < LocalizationFiles.Count; l++)
                {
                    LocalizationFiles[l].Data.keyVals.Add(new LocalizationManager.KeyVal() { Key = "null", Value = "null" });
                }
            }
            GUILayout.EndHorizontal();

            scroll2 = GUILayout.BeginScrollView(scroll2, GUILayout.Height(512));

            for (int q = 0; q < CurrentFileStructedData.keyVals.Count; q++)
            {
                GUILayout.BeginHorizontal("HelpBox");

                EditorGUI.BeginChangeCheck();

                GUILayout.Label("ID: " + q, GUILayout.Width(60));

                string key = GUILayout.TextField(CurrentFileStructedData.keyVals[q].Key, GUILayout.Width(150));

                if (EditorGUI.EndChangeCheck())
                {
                    CurrentFileStructedData.keyVals[q] = new LocalizationManager.KeyVal() { Key = key, Value = "null" };
                    for (int l = 0; l < LocalizationFiles.Count; l++)
                    {
                        LocalizationManager.KeyVal v = LocalizationFiles[l].Data.keyVals[q];
                        v.Key = key;
                        LocalizationFiles[l].Data.keyVals[q] = v;
                    }
                }

                GUILayout.Space(10);

                GUILayout.BeginVertical(GUILayout.Height(elementSize));
                for (int l = 0; l < LocalizationFiles.Count; l++)
                {
                    GUILayout.BeginHorizontal("HelpBox");

                    GUILayout.Label(LocalizationFiles[l].LangKey, GUILayout.Width(80));

                    EditorGUI.BeginChangeCheck();

                    string value = EditorGUILayout.TextArea(LocalizationFiles[l].Data.keyVals[q].Value, GUILayout.Height(langValueSize));

                    if (EditorGUI.EndChangeCheck())
                    {
                        LocalizationManager.KeyVal v = LocalizationFiles[l].Data.keyVals[q];
                        v.Value = value;
                        LocalizationFiles[l].Data.keyVals[q] = v;
                    }

                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUI.color = Color.red;
                if(GUILayout.Button("X", GUILayout.Width(20)))
                {
                    CurrentFileStructedData.keyVals.RemoveAt(q);
                    for (int l = 0; l < LocalizationFiles.Count; l++)
                    {
                        LocalizationFiles[l].Data.keyVals.RemoveAt(q);
                    }
                    return;
                }
                GUI.color = Color.white;

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();

        Undo.RecordObject(this, "lm");
    }

    private void ClickOpen ()
    {
        string selectedFile = LocalizationFolderPath + "\\keyList.txt";

        if (File.Exists(selectedFile))
        {
            OpenFile(selectedFile);
        }
        else
        {
            File.Create(selectedFile).Close();
            OpenFile(selectedFile);
        }
    }

    private void OpenFile (string filePath)
    {
        CurrentFile = filePath;
        CurrentFileName = Path.GetFileName(filePath);
        CurrentFileData = File.ReadAllText(filePath);

        LocalizationManager.KeyValStack data = null;
        try
        {
            data = JsonUtility.FromJson<LocalizationManager.KeyValStack>(CurrentFileData);
        }
        catch
        {

        }

        if(data == null)
        {
            if(string.IsNullOrEmpty(CurrentFileData))
            {
                CurrentFileStructedData = new LocalizationManager.KeyValStack();
                CurrentFileStructedData.keyVals = new List<LocalizationManager.KeyVal>();
                LoadLocalizations();
            }
            else
            {
                if (EditorUtility.DisplayDialog("File is broken", "Если продолжить работать с ним, его контент затрётся", "ок", "не ок"))
                {
                    CurrentFileStructedData = new LocalizationManager.KeyValStack();
                    CurrentFileStructedData.keyVals = new List<LocalizationManager.KeyVal>();
                    LoadLocalizations();
                }
                else
                {
                    this.Close();
                }
            }
        }
        else
        {
            CurrentFileStructedData = data;
            if (CurrentFileStructedData.keyVals == null)
                CurrentFileStructedData.keyVals = new List<LocalizationManager.KeyVal>();

            LoadLocalizations();
        }
    }

    private void LoadLocalizations ()
    {
        string[] files = Directory.GetFiles(LocalizationFolderPath, "*.lang");

        LocalizationFiles = new List<LocalizationFile>();

        for (int f = 0; f < files.Length; f++)
        {
            string json = File.ReadAllText(files[f]);

            LocalizationManager.KeyValStack jdata = JsonUtility.FromJson<LocalizationManager.KeyValStack>(json);
            if(jdata == null)
            {
                jdata = new LocalizationManager.KeyValStack();
                jdata.keyVals = new List<LocalizationManager.KeyVal>();
            }
            else
            {
                if (jdata.keyVals == null)
                    jdata.keyVals = new List<LocalizationManager.KeyVal>();
            }

            LocalizationManager.KeyValStack data = new LocalizationManager.KeyValStack();
            data.keyVals = new List<LocalizationManager.KeyVal>();

            for (int q = 0; q < CurrentFileStructedData.keyVals.Count; q++)
            {
                int valueIndx = jdata.keyVals.FindIndex((x) => x.Key == CurrentFileStructedData.keyVals[q].Key);

                string value = "null";

                if (valueIndx != -1)
                    value = jdata.keyVals[valueIndx].Value;

                data.keyVals.Add(new LocalizationManager.KeyVal()
                {
                    Key = CurrentFileStructedData.keyVals[q].Key,
                    Value = value
                });
            }

            LocalizationFiles.Add(new LocalizationFile()
            {
                FilePath = files[f],
                LangKey = Path.GetFileNameWithoutExtension(files[f]),
                Data = data
            });
        }
    }

    private void SaveCurrentFile ()
    {
        if(!string.IsNullOrEmpty(CurrentFile) && CurrentFileStructedData != null && CurrentFileStructedData.keyVals != null)
        {
            string data = JsonUtility.ToJson(CurrentFileStructedData);
            File.WriteAllText(CurrentFile, data);

            foreach(LocalizationFile lf in LocalizationFiles)
            {
                data = JsonUtility.ToJson(lf.Data, true);
                File.WriteAllText(lf.FilePath, data);
            }
        }
    }
}

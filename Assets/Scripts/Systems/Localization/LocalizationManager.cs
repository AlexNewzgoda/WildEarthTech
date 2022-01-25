using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

/*public class SteamLanguage
{
    public const string arabic = "arabic";
    public const string bulgarian = "bulgarian";
    public const string schinese = "schinese";
    public const string tchinese = "tchinese";
    public const string czech = "czech";
    public const string danish = "danish";
    public const string dutch = "dutch";
    public const string english = "english";
    public const string finnish = "finnish";
    public const string french = "french";
    public const string german = "german";
    public const string greek = "greek";
    public const string hungarian = "hungarian";
    public const string italian = "italian";
    public const string japanese = "japanese";
    public const string koreana = "koreana";
    public const string norwegian = "norwegian";
    public const string polish = "polish";
    public const string portuguese = "portuguese";
    public const string brazilian = "brazilian";
    public const string romanian = "romanian";
    public const string russian = "russian";
    public const string spanish = "spanish";
    public const string latam = "latam";
    public const string swedish = "swedish";
    public const string thai = "thai";
    public const string turkish = "turkish";
    public const string ukrainian = "ukrainian";
    public const string vietnamese = "vietnamese";
}*/

public class LocalizationManager
{
    public enum Language
    {
        arabic,
        bulgarian,
        schinese,
        tchinese,
        czech,
        danish,
        dutch,
        english,
        finnish,
        french,
        german,
        greek,
        hungarian,
        italian,
        japanese,
        koreana,
        norwegian,
        polish,
        portuguese,
        brazilian,
        romanian,
        russian,
        spanish,
        latam,
        swedish,
        thai,
        turkish,
        ukrainian,
        vietnamese,
    }

    [System.Serializable]
    public struct KeyVal
    {
        public string Key;
        public string Value;
    }

    [System.Serializable]
    public class KeyValStack
    {
        public List<KeyVal> keyVals;
    }

    public delegate void LangUpdateEvent();
    public static LangUpdateEvent OnLangUpdate;

    public static Dictionary<string, string> CurrentDictonary;

    private string _localizationFolderPath;
    private string _langKey;

    public string CurrentLangKey { get => _langKey; }

    public const string DefaultLangFile = "english.lang";

    public static LocalizationManager Instance;

    public LocalizationManager (string localizationFolderPath, string langKey)
    {
        Instance = this;

        _localizationFolderPath = localizationFolderPath;
        _langKey = langKey;

        Init();
    }

    public void ChangeLang (string langKey)
    {
        _langKey = langKey;

        Init();
    }

    public string[] GetLangKeysArray ()
    {
        string[] keys = Directory.GetFiles(_localizationFolderPath, "*.lang");
        for(int q = 0; q < keys.Length; q++)
        {
            keys[q] = Path.GetFileNameWithoutExtension(keys[q]);
        }
        return keys;
    }

    private void Init ()
    {
        if(Directory.Exists(_localizationFolderPath))
        {
            string[] files = Directory.GetFiles(_localizationFolderPath, "*.lang");

            string findedFile = null;
            for(int f = 0; f < files.Length; f++)
            {
                string fname = Path.GetFileNameWithoutExtension(files[f]);
                if (fname == _langKey)
                {
                    findedFile = files[f];
                    break;
                }
            }

            if(string.IsNullOrEmpty(findedFile))
            {
                findedFile = Path.Combine(_localizationFolderPath, DefaultLangFile);
            }

            if(!string.IsNullOrEmpty(findedFile) && File.Exists(findedFile))
            {
                string data = File.ReadAllText(findedFile);

                KeyVal[] keyVals = null;
                KeyValStack kvs = JsonUtility.FromJson<KeyValStack>(data);

                if (kvs != null)
                    keyVals = kvs.keyVals.ToArray();

                if (keyVals != null)
                {
                    CurrentDictonary = new Dictionary<string, string>();

                    for (int q = 0; q < keyVals.Length; q++)
                    {
                        if (!CurrentDictonary.ContainsKey(keyVals[q].Key))
                        {
                            CurrentDictonary.Add(keyVals[q].Key, keyVals[q].Value);
                        }
                    }

                    OnLangUpdate?.Invoke();
                }
                else
                {
                    InitError($"File broke: {_langKey}.lang");
                }
            }
            else
            {
                InitError($"File not found: {_langKey}.lang");
            }
        }
        else
        {
            InitError("Folder not found");
        }
    }

    private void InitError (string error)
    {
        if(Application.isEditor)
            Debug.LogError($"localization init error: {error}");

        //krkr.MessageBox.Show("Fatal error", "Code: lle", true, () => Application.Quit(), () => Application.Quit());
    }
}

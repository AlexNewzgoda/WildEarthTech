using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class InputSettings : MonoBehaviour 
{
    //KeysStart
    public static KeyCode key_Jump = KeyCode.Space;
    public static KeyCode key_Run = KeyCode.LeftShift;
    public static KeyCode key_Use = KeyCode.E;
    public static KeyCode key_MoveForward = KeyCode.W;
    public static KeyCode key_MoveBack = KeyCode.S;
    public static KeyCode key_MoveLeft = KeyCode.A;
    public static KeyCode key_Inventory = KeyCode.I;
    public static KeyCode key_Slot1 = KeyCode.Alpha1;
    public static KeyCode key_Slot2 = KeyCode.Alpha2;
    public static KeyCode key_Slot3 = KeyCode.Alpha3;
    public static KeyCode key_Slot4 = KeyCode.Alpha4;
    public static KeyCode key_Slot5 = KeyCode.Alpha5;
    public static KeyCode key_Slot6 = KeyCode.Alpha6;
    public static KeyCode key_Slot7 = KeyCode.Alpha7;
    public static KeyCode key_Slot8 = KeyCode.Alpha8;
    public static KeyCode key_Slot9 = KeyCode.Alpha9;
    //KeysEnd

    //ValuesStart
    public static float val_Sensitivity = 1;
    //ValuesEnd

    public static InputSettings Instance;
    private string _settingsFilePath;
    private static JObject _loadedData;

    //Runtime Edit
    public static event Action<bool> EditEvent;
    private bool _isEditing = false;
    private KeyCode _scannedKeyCode = KeyCode.None;
    private string _currentAction = "";
    private bool LShift = false;
    private bool RShift = false;
    private InputSettingsActionChanger.ChangeResult _currentTargetAction;
    private UnityEngine.EventSystems.EventSystem _eventSystemTemp;
    public KeyCode EscapeButton = KeyCode.Escape;

    public void Init (string dataFilePath)
    {
        Instance = this;

        _settingsFilePath = dataFilePath;

        if (!File.Exists(_settingsFilePath))
        {
            _loadedData = new JObject();
            SaveFile();
        }
        else
        {
            try
            {
                _loadedData = JObject.Parse(File.ReadAllText(_settingsFilePath, System.Text.Encoding.UTF8));

                if (_loadedData == null)
                    throw new System.Exception();
            }
            catch
            {
                _loadedData = new JObject();
                SaveFile();
            }
        }
        
        LoadKeyMap();
    }

    public void ResetToDefault ()
    {
        _loadedData = new JObject();
        LoadKeyMap();
    }

    public void SaveFile ()
    {
        string jData = JsonConvert.SerializeObject(_loadedData, Formatting.Indented);

        byte[] data = System.Text.Encoding.UTF8.GetBytes(jData);

        using (FileStream fs = File.Create(_settingsFilePath))
        {
            fs.Write(data, 0, data.Length);
        }
    }

    public T GetValue<T> (string name, T defaultValue, bool addIsNotExixs = false)
    {
        JToken token;
        if (_loadedData.TryGetValue(name, out token))
        {
            return token.Value<T>();
        }
        else
        {
            if (addIsNotExixs)
            {
                _loadedData.Add(name, JToken.FromObject(defaultValue));
            }
        }

        return defaultValue;
    }

    public void SetValue<T>(string name, T value, bool addIsNotExixs = false)
    {
        JToken token;
        if (_loadedData.TryGetValue(name, out token))
        {
            token.Replace(JToken.FromObject(value));
        }
        else
        {
            if (addIsNotExixs)
            {
                _loadedData.Add(name, JToken.FromObject(value));
            }
        }
    }

    public KeyCode GetKeyValue (string keyName, KeyCode defaultKey, bool addIsNotExixs = false)
    {
        return (KeyCode)GetValue(keyName, (int)defaultKey, addIsNotExixs);
    }

    public void SetKeyValue (string keyName, KeyCode keyCode, bool addIsNotExixs = false)
    {
        SetValue(keyName, (int)keyCode, addIsNotExixs);
    }

    public void LoadKeyMap ()
    {
        //KeysStartLoad
        key_Jump = GetKeyValue("Jump", KeyCode.Space, true);
        key_Run = GetKeyValue("Run", KeyCode.LeftShift, true);
        key_Use = GetKeyValue("Use", KeyCode.E, true);
        key_MoveForward = GetKeyValue("MoveForward", KeyCode.W, true);
        key_MoveBack = GetKeyValue("MoveBack", KeyCode.S, true);
        key_MoveLeft = GetKeyValue("MoveLeft", KeyCode.A, true);
        key_Inventory = GetKeyValue("Inventory", KeyCode.I, true);
        key_Slot1 = GetKeyValue("Slot1", KeyCode.Alpha1, true);
        key_Slot2 = GetKeyValue("Slot2", KeyCode.Alpha2, true);
        key_Slot3 = GetKeyValue("Slot3", KeyCode.Alpha3, true);
        key_Slot4 = GetKeyValue("Slot4", KeyCode.Alpha4, true);
        key_Slot5 = GetKeyValue("Slot5", KeyCode.Alpha5, true);
        key_Slot6 = GetKeyValue("Slot6", KeyCode.Alpha6, true);
        key_Slot7 = GetKeyValue("Slot7", KeyCode.Alpha7, true);
        key_Slot8 = GetKeyValue("Slot8", KeyCode.Alpha8, true);
        key_Slot9 = GetKeyValue("Slot9", KeyCode.Alpha9, true);
        //KeysEndLoad

        //ValuesStartLoad
        val_Sensitivity = GetValue("Sensitivity", 1, true);
        //ValuesEndLoad
    }

    private void Update()
    {
        if(_isEditing)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                LShift = true;
            if (Input.GetKeyDown(KeyCode.RightShift))
                RShift = true;

            if (UnityEngine.EventSystems.EventSystem.current != null)
                _eventSystemTemp = UnityEngine.EventSystems.EventSystem.current;

            if (_eventSystemTemp != null)
                _eventSystemTemp.enabled = false;
        }
        else
        {
            if (_eventSystemTemp != null)
            {
                _eventSystemTemp.enabled = true;
                _eventSystemTemp = null;
            }
        }
    }

    private void OnGUI()
    {
        if (_isEditing)
        {
            if (Event.current.isKey || LShift || RShift)
            {
                _scannedKeyCode = Event.current.keyCode;

                if (LShift)
                    _scannedKeyCode = KeyCode.LeftShift;

                if (RShift)
                    _scannedKeyCode = KeyCode.RightShift;
            }
            else
            {
                _scannedKeyCode = KeyCode.None;
            }

            if (Event.current.isMouse)
            {
                int mouseButton = Event.current.button + 323;
                _scannedKeyCode = (KeyCode)mouseButton;
            }

            if (_scannedKeyCode == EscapeButton)
            {
                CancelKeyEdit();
                return;
            }

            if (_scannedKeyCode != KeyCode.None && _currentTargetAction != null && !string.IsNullOrEmpty(_currentAction))
            {
                SetKeyValue(_currentAction, _scannedKeyCode);

                _currentTargetAction.Set(_scannedKeyCode);

                _scannedKeyCode = KeyCode.None;
                _currentAction = null;

                _isEditing = false;
                EditEvent?.Invoke(false);

                LoadKeyMap();
            }

            LShift = false;
            RShift = false;
        }
    }

    private void OnApplicationQuit()
    {
        SaveFile();
    }

    public void StartKeyEdit (ref InputSettingsActionChanger.ChangeResult result, string action)
    {
        if (_isEditing == true)
            return;

        _isEditing = true;

        _currentTargetAction = result;
        _currentTargetAction.Set(KeyCode.None);

        _currentAction = action;

        EditEvent?.Invoke(true);
    }

    public void CancelKeyEdit ()
    {
        _isEditing = false;

        if (_currentTargetAction != null)
            _currentTargetAction.Set(GetKeyValue(_currentAction, 0));

        _currentAction = null;
        _currentTargetAction = null;

        EditEvent?.Invoke(false);
    }

}

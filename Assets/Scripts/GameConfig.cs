using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    public InputSettings inputSettings;
    private LocalizationManager _localizationManager;
    private void Awake()
    {
        _localizationManager = new LocalizationManager(Application.dataPath + "/../Localization", "russian");
        inputSettings.Init(Application.dataPath + "/../UserData/Input.txt");
    

    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {

           GameSerializer.Instance.Save(Application.dataPath + "/Save.txt");
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            GameSerializer.Instance.Load(Application.dataPath + "/Save.txt");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string TextKey = "null";
    public Text TextView;

    private void Start()
    {
        UpdateText();
        LocalizationManager.OnLangUpdate += UpdateText;
    }

    public static string GetText(string TextKey)
    {
        string text;

        if(!LocalizationManager.CurrentDictonary.TryGetValue(TextKey, out text))
            text = "????";

        return text;
    }

    public void UpdateText()
    {
        if (TextView != null)
        {
            TextView.text = GetText(TextKey);
        }
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLangUpdate -= UpdateText;
    }
}

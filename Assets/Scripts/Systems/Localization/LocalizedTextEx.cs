using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedTextEx : MonoBehaviour
{
    public string Text;
    public string[] Keys;
    private string[] Texts;

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
            text = "?";

        return text;
    }

    public void UpdateText()
    {
        if (TextView != null)
        {
            Texts = new string[Keys.Length];

            for (int q = 0; q < Keys.Length; q++)
            {
                Texts[q] = GetText(Keys[q]);
            }

            TextView.text = string.Format(Text, Texts);
        }
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLangUpdate -= UpdateText;
    }
}

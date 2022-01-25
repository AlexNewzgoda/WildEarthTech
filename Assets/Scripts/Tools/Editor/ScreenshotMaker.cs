using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ScreenshotMaker
{
    [MenuItem("Tools/Screenshot %F12")]
    public static void Shot ()
    {
        string time = System.DateTime.Now.ToString("dd_MM_yy_HH_mm_ss");

        string dirPath = Application.dataPath + "/../Screenshots";

        if(!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        Debug.Log(dirPath);

        ScreenCapture.CaptureScreenshot($"{dirPath}/screenshot_{time}.png", 1);
    }

    [MenuItem("Tools/Screenshot Save Dialog %F11")]
    public static void ShotSaveDialog()
    {
        string time = System.DateTime.Now.ToString("dd_MM_yy_HH_mm_ss");
        string fileName = $"screenshot_{time}";

        string filePath = EditorUtility.SaveFilePanel("save", Application.dataPath, fileName, "png");

        if (string.IsNullOrEmpty(filePath))
            return;

        ScreenCapture.CaptureScreenshot(filePath, 1);
    }
}

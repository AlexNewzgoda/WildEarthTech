using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;


public class PreBuildLocalizationCopy : IPreprocessBuildWithReport
{
    private static string LocalizationFolderPath { get { return Application.dataPath + "/../Localization"; } }

    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        try
        {
            string buildRootDir = Path.GetDirectoryName(report.summary.outputPath);
            string localizationFolderPath = Path.Combine(buildRootDir, "Localization");
            string localizationFolderPath_Project = LocalizationFolderPath;

            if (Directory.Exists(localizationFolderPath))
                Directory.Delete(localizationFolderPath, true);

            Directory.CreateDirectory(localizationFolderPath);
        
            foreach(string file in Directory.GetFiles(localizationFolderPath_Project, "*.lang"))
            {
                string fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(localizationFolderPath, fileName), true);
            }
        }
        catch
        {
            Debug.LogError("Localization copy error");
        }
        finally
        {
            Debug.Log("Localization copied");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "AudioClipL", menuName = "Localized Audio Clip")]
public class AudioClipL : ScriptableObject
{
    [System.Serializable]
    public struct LocalizedClip
    {
        public LocalizationManager.Language LangKey;
        public AudioClip Audio;
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/Localized Audio Clip From File", true, 7)]
    public static bool CreateValidate ()
    {
        if (UnityEditor.Selection.objects != null)
        {
            foreach (Object obj in UnityEditor.Selection.objects)
            {
                if (obj is AudioClip)
                {
                    return true;
                }
            }
        }
        return false;
    }

    [UnityEditor.MenuItem("Assets/Create/Localized Audio Clip From File", false, 7)]
    public static void Create ()
    {
        Object selectedClip = null;

        if (UnityEditor.Selection.objects != null)
        {
            foreach (Object obj in UnityEditor.Selection.objects)
            {
                if (obj is AudioClip)
                {
                    selectedClip = obj;
                    break;
                }
            }
        }

        if (selectedClip != null)
        {
            string path = System.IO.Path.GetDirectoryName( UnityEditor.AssetDatabase.GetAssetPath(selectedClip));
            path = path + "\\" + System.IO.Path.GetFileNameWithoutExtension(selectedClip.name) + ".asset";
            
            AudioClipL asset = CreateInstance<AudioClipL>();

            UnityEditor.AssetDatabase.CreateAsset(asset, path);
            UnityEditor.AssetDatabase.SaveAssets();

            UnityEditor.Selection.activeObject = asset;
        }
    }
#endif

    public LocalizedClip[] LocalizedClips;

    public AudioClip GetAudioClip ()
    {
        AudioClip localizedClip = LocalizedClips.Where((x) => x.LangKey.ToString() == LocalizationManager.Instance.CurrentLangKey).FirstOrDefault().Audio;
        if(localizedClip == null)
        {
            localizedClip = LocalizedClips.Where((x) => x.LangKey.ToString() == "english").FirstOrDefault().Audio;
        }

        return localizedClip;
    }

    public AudioClip GetAudioClip(string langKey)
    {
        AudioClip localizedClip = LocalizedClips.Where((x) => x.LangKey.ToString() == langKey).FirstOrDefault().Audio;
        if (localizedClip == null)
        {
            localizedClip = LocalizedClips.Where((x) => x.LangKey.ToString() == "english").FirstOrDefault().Audio;
        }

        return localizedClip;
    }

    public static implicit operator AudioClip (AudioClipL value)
    {
        return value.GetAudioClip();
    }
}

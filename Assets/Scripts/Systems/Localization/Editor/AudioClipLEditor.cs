using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioClipL))]
public class AudioClipLEditor : Editor
{
    public static void PlayClip(AudioClip clip, int startSample, bool loop)
    {
        if (clip == null)
            return;

        Assembly audioImporter = typeof(AudioImporter).Assembly;
        System.Type audioUtil = audioImporter.GetType("UnityEditor.AudioUtil");

        audioUtil.InvokeMember("PlayClip", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[3] { clip, startSample, loop });
    }

    public static void StopClip(AudioClip clip)
    {
        if (clip == null)
            return;

        Assembly audioImporter = typeof(AudioImporter).Assembly;
        System.Type audioUtil = audioImporter.GetType("UnityEditor.AudioUtil");

        audioUtil.InvokeMember("StopClip", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[1] { clip });
    }

    public static void StopAllClips()
    {
        Assembly audioImporter = typeof(AudioImporter).Assembly;
        System.Type audioUtil = audioImporter.GetType("UnityEditor.AudioUtil");

        audioUtil.InvokeMember("StopAllClips", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
    }

    public static bool IsClipPlaying(AudioClip clip)
    {
        Assembly audioImporter = typeof(AudioImporter).Assembly;
        System.Type audioUtil = audioImporter.GetType("UnityEditor.AudioUtil");

        return (bool)audioUtil.InvokeMember("IsClipPlaying", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[1] { clip });
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty localizedClips = serializedObject.FindProperty("LocalizedClips");

        localizedClips.arraySize = EditorGUILayout.IntField(new GUIContent("Size"), localizedClips.arraySize);
        if (GUILayout.Button("Add"))
        {
            localizedClips.arraySize++;
            serializedObject.ApplyModifiedProperties();
            return;
        }

        for (int q = 0; q < localizedClips.arraySize; q++)
        {
            EditorGUILayout.BeginVertical("HelpBox");

            EditorGUILayout.PropertyField(localizedClips.GetArrayElementAtIndex(q).FindPropertyRelative("LangKey"));
            EditorGUILayout.PropertyField(localizedClips.GetArrayElementAtIndex(q).FindPropertyRelative("Audio"));

            EditorGUILayout.BeginHorizontal("Box");

            if (GUILayout.Button("Play"))
            {
                SerializedProperty currentItem = localizedClips.GetArrayElementAtIndex(q);
                AudioClip currentItemAudio = (AudioClip)currentItem.FindPropertyRelative("Audio").objectReferenceValue;

                if (IsClipPlaying(currentItemAudio))
                {
                    StopClip(currentItemAudio);
                }

                PlayClip(currentItemAudio, 0, false);
            }

            if (GUILayout.Button("Stop"))
            {
                StopAllClips();
            }

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Up", GUILayout.Width(60)))
            {
                if (q > 0)
                {
                    SerializedProperty currentItem = localizedClips.GetArrayElementAtIndex(q);
                    int currentItemLangKey = currentItem.FindPropertyRelative("LangKey").enumValueIndex;
                    Object currentItemAudio = currentItem.FindPropertyRelative("Audio").objectReferenceValue;

                    SerializedProperty preItem = localizedClips.GetArrayElementAtIndex(q - 1);
                    int nextItemLangKey = preItem.FindPropertyRelative("LangKey").enumValueIndex;
                    Object nextItemAudio = preItem.FindPropertyRelative("Audio").objectReferenceValue;

                    localizedClips.GetArrayElementAtIndex(q - 1).FindPropertyRelative("LangKey").enumValueIndex = currentItemLangKey;
                    localizedClips.GetArrayElementAtIndex(q - 1).FindPropertyRelative("Audio").objectReferenceValue = currentItemAudio;

                    localizedClips.GetArrayElementAtIndex(q).FindPropertyRelative("LangKey").enumValueIndex = nextItemLangKey;
                    localizedClips.GetArrayElementAtIndex(q).FindPropertyRelative("Audio").objectReferenceValue = nextItemAudio;

                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }

            if (GUILayout.Button("Down", GUILayout.Width(60)))
            {
                if (q + 1 < localizedClips.arraySize)
                {
                    SerializedProperty currentItem = localizedClips.GetArrayElementAtIndex(q);
                    int currentItemLangKey = currentItem.FindPropertyRelative("LangKey").enumValueIndex;
                    Object currentItemAudio = currentItem.FindPropertyRelative("Audio").objectReferenceValue;

                    SerializedProperty nextItem = localizedClips.GetArrayElementAtIndex(q + 1);
                    int nextItemLangKey = nextItem.FindPropertyRelative("LangKey").enumValueIndex;
                    Object nextItemAudio = nextItem.FindPropertyRelative("Audio").objectReferenceValue;

                    localizedClips.GetArrayElementAtIndex(q + 1).FindPropertyRelative("LangKey").enumValueIndex = currentItemLangKey;
                    localizedClips.GetArrayElementAtIndex(q + 1).FindPropertyRelative("Audio").objectReferenceValue = currentItemAudio;

                    localizedClips.GetArrayElementAtIndex(q).FindPropertyRelative("LangKey").enumValueIndex = nextItemLangKey;
                    localizedClips.GetArrayElementAtIndex(q).FindPropertyRelative("Audio").objectReferenceValue = nextItemAudio;

                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }

            GUI.color = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(60)))
            {
                localizedClips.DeleteArrayElementAtIndex(q);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(20);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

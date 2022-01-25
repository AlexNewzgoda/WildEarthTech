using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class EntityHelper : Editor
{
    public static List<Entity> Entities;

    static EntityHelper ()
    {
        Entities = new List<Entity>();
        Entities.Clear();
        Entities.AddRange(FindObjectsOfType<Entity>());

        EditorApplication.hierarchyChanged += EditorApplication_hierarchyChanged;
    }

    private static void EditorApplication_hierarchyChanged()
    {
        bool needUpdatePropsList = false;

        if (!EditorApplication.isPlaying)
        {
            if (Selection.objects != null)
            {
                foreach (GameObject go in Selection.gameObjects)
                {
                    if (!IsPrefab(go))
                    {
                        Entity prop;
                        if (go.TryGetComponent(out prop))
                        {
                            if(!Entities.Contains(prop))
                            {
                                prop.ID = null;
                                InitSceneID(prop);

                                needUpdatePropsList = true;
                            }
                        }
                    }
                }
            }
        }

        if (needUpdatePropsList)
        {
            Debug.Log("Update Props");
            Entities.Clear();
            Entities.AddRange(FindObjectsOfType<Entity>());
        }

        ResetIDInPrefabs();
    }

    public static void InitSceneID (Entity prop)
    {
        SerializedObject so = new SerializedObject(prop);
        SerializedProperty ID = so.FindProperty("ID");

        if (string.IsNullOrEmpty(ID.stringValue))
        {
            ID.stringValue = GetNewID();
            so.ApplyModifiedProperties();
        }
    }

    public static string GetNewID()
    {
        return System.Guid.NewGuid().ToString();
    }

    public static bool IsPrefab (GameObject gameObject)
    {
        return UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) != null || EditorUtility.IsPersistent(gameObject);
    }

    [MenuItem("Tools/Reset ID in prefabs")]
    public static void ResetIDInPrefabs ()
    {
        bool changed = false;
        Entity[] allNV = (Entity[])Resources.FindObjectsOfTypeAll(typeof(Entity));
        foreach(Entity prop in allNV)
        {
            if(IsPrefab(prop.gameObject))
            {
                if (!string.IsNullOrEmpty(prop.ID))
                {
                    prop.ID = null;
                    EditorUtility.SetDirty(prop.gameObject);
                    Debug.Log(prop.name + " fixed");
                    changed = true;
                }
            }       
        }
        if(changed)
            AssetDatabase.SaveAssets();
    }

    public static void CheckPropsInScene ()
    {
        Entity[] allProps = FindObjectsOfType<Entity>();
        foreach (Entity props in allProps)
        {
            InitSceneID(props);
        }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class GameSerializer : MonoBehaviour
{
    [System.Serializable]
    public struct GameData
    {
        public List<string> LoadedScenes;
        public List<SceneData> Scenes;
    }

    [System.Serializable]
    public struct SceneData
    {
        public string Scene;
        public bool IsFirstLoaded;
        public string[] Props;
    }

    public static GameSerializer Instance;
    private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
    private static List<Entity> _loadedEntities = new List<Entity>();

    public string SaveDirectoryLocalPath = "../UserData/";
    public string SaveDirectoryPath => Path.Combine(Application.dataPath, SaveDirectoryLocalPath); 
    private GameData _gameData = default;
    
    public static string GetDataFromObject<T> (T obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented, _serializerSettings);
    }

    public static T GetObjectFromData<T>(string data)
    {
        return JsonConvert.DeserializeObject<T>(data, _serializerSettings);
    }

    public static bool SceneIsFirstLoaded (string sceneName)
    {
        if (Instance == null)
            return true;

        if(Instance._gameData.Scenes != null)
        {
            int sceneIndex = Instance._gameData.Scenes.FindIndex((s) => s.Scene.Equals(sceneName));

            if(sceneIndex > -1)
            {
                return Instance._gameData.Scenes[sceneIndex].IsFirstLoaded;
            }
        }

        return false;
    }

    public static void OnAddEntity (Entity entity)
    {
        _loadedEntities.Add(entity);
    }

    public static void OnRemoveEntity (Entity entity)
    {
        _loadedEntities.Remove(entity);
    }

    public static Entity GetEntityByID (string id)
    {
        return _loadedEntities.Find((e) => e.ID.Equals(id));
    }

    //-----------------------------------------------------------

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (!Directory.Exists(SaveDirectoryPath))
        {
            Directory.CreateDirectory(SaveDirectoryPath);
        }

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Save(string saveFilePath)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        UpdateSceneDataFromScene(currentScene);

        _gameData.LoadedScenes = new List<string> { currentScene.name };

        string jData = JsonConvert.SerializeObject(_gameData, Formatting.Indented, _serializerSettings);

        File.WriteAllText(saveFilePath, jData);
    }

    public void Load(string saveFilePath)
    {
        string jData = File.ReadAllText(saveFilePath);

        _gameData = JsonConvert.DeserializeObject<GameData>(jData);

        if(_gameData.LoadedScenes != null)
        {
            if (_gameData.LoadedScenes.Count > 0 && !string.IsNullOrEmpty(_gameData.LoadedScenes[0]))
            {
                SceneManager.LoadScene(_gameData.LoadedScenes[0]);
            }
        }
    }

    //---------------------------------------------------------------------------

    private void OnActiveSceneChanged(Scene preScene, Scene nextScene)
    {
        //ApplyStoredDataToScene(nextScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyStoredDataToScene(scene);
    }

    public void UpdateSceneDataFromScene (Scene scene)
    {
        SceneData sceneData = new SceneData();
        Entity[] entytiesInScene = FindObjectsOfType<Entity>(true).Where((x) => x.gameObject.scene == scene).ToArray();

        sceneData.Scene = scene.name;

        if (entytiesInScene != null)
        {
            sceneData.Props = new string[entytiesInScene.Length];

            for (int q = 0; q < entytiesInScene.Length; q++)
            {
                sceneData.Props[q] = entytiesInScene[q].GetData();
            }
        }

        if (_gameData.Scenes == null)
            _gameData.Scenes = new List<SceneData>();

        int findedSceneDataIndx = _gameData.Scenes.FindIndex((x) => x.Scene == sceneData.Scene);

        if (findedSceneDataIndx != -1)
        {
            _gameData.Scenes[findedSceneDataIndx] = sceneData;
        }
        else
        {
            _gameData.Scenes.Add(sceneData);
        }
    }

    private void ApplyStoredDataToScene (Scene loadedScene)
    {
        if (_gameData.Scenes != null)
        {
            int sceneIndx = _gameData.Scenes.FindIndex((x) => x.Scene == loadedScene.name);

            if (sceneIndx != -1)
            {
                SceneData sceneData = _gameData.Scenes[sceneIndx];

                sceneData.IsFirstLoaded = false;
                _gameData.Scenes[sceneIndx] = sceneData;

                Entity.PropData[] propsData = null;
                if (sceneData.Props != null)
                {
                    propsData = new Entity.PropData[sceneData.Props.Length];

                    for (int q = 0; q < sceneData.Props.Length; q++)
                    {
                        propsData[q] = JsonConvert.DeserializeObject<Entity.PropData>(sceneData.Props[q]);
                    }
                }

                List<Entity> preloadedEntitiesInScene = new List<Entity>();
                preloadedEntitiesInScene.AddRange(FindObjectsOfType<Entity>(true).Where((x) => x.gameObject.scene == loadedScene).ToArray());

                if (propsData != null)
                {
                    for (int q = 0; q < propsData.Length; q++)
                    {
                        Entity.PropData propData = propsData[q];

                        if (!string.IsNullOrEmpty(propData.PrefabGUID))
                        {
                            Entity findedEntity = preloadedEntitiesInScene.Where((x) => x.ID == propData.ID).FirstOrDefault();

                            if (findedEntity)
                            {
                                findedEntity.SetData(propData);
                                preloadedEntitiesInScene.Remove(findedEntity);
                            }
                            else
                            {
                                AssetReference reference = new AssetReference(propData.PrefabGUID);
                                GameObject entityObject = reference.InstantiateAsync().WaitForCompletion();

                                Entity entity;
                                if (entityObject.TryGetComponent(out entity))
                                {
                                    entity.SetData(propData);
                                }
                            }
                        }
                        else
                        {
                            Entity findedEntity = preloadedEntitiesInScene.Where((x) => x.ID == propData.ID).FirstOrDefault();

                            if (findedEntity != null)
                            {
                                findedEntity.SetData(propData);
                                preloadedEntitiesInScene.Remove(findedEntity);
                            }
                        }
                    }
                }

                for (int q = 0; q < preloadedEntitiesInScene.Count; q++)
                {
                    Destroy(preloadedEntitiesInScene[q].gameObject);
                }
            }
        }
    }
}

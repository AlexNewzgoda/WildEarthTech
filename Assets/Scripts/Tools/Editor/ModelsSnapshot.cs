using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ModelsSnapshot : EditorWindow
{
    private Camera _snapShotCamera;
    private RenderTexture _renderTexture;
    private GameObject _targetObject;
    private Light _snapshotLight;
    private ReflectionProbe _reflectionProbe;

    public GameObject SelectedObject;

    public Color BackgroundColor = Color.white;
    public Vector3 _posOffset = Vector3.forward * 20;
    public Vector3 _rotOffset = Vector3.zero;

    public Vector2Int CamResolution = new Vector2Int(256, 256);

    private Scene SnapShotScene;
    private string PrevScenePath;

    [MenuItem("Tools/ModelsSnapshot")]
    public static void ShowWindow ()
    {
        ModelsSnapshot window = (ModelsSnapshot)EditorWindow.GetWindow(typeof(ModelsSnapshot));
        window.Show();
        window.Init();
        window.minSize = new Vector2(512, 600);
    }

    public void Init ()
    {
        PrevScenePath = EditorSceneManager.GetActiveScene().path;
        SnapShotScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SnapShotScene.name = "SnapshotScene";
        EditorSceneManager.SetActiveScene(SnapShotScene);
        
        _snapShotCamera = new GameObject("SSCamera").AddComponent<Camera>();
        _snapShotCamera.backgroundColor = BackgroundColor;
        _snapShotCamera.clearFlags = CameraClearFlags.SolidColor;
        _renderTexture = new RenderTexture(256, 256, 0);
        _snapShotCamera.targetTexture = _renderTexture;
        _snapShotCamera.Render();

        _snapshotLight = new GameObject("Light").AddComponent<Light>();
        _snapshotLight.type = LightType.Directional;
        _snapshotLight.intensity = 1;
        _snapshotLight.color = Color.white;
        _snapshotLight.shadows = LightShadows.Soft;

        _reflectionProbe = new GameObject("RefProbe").AddComponent<ReflectionProbe>();
        _reflectionProbe.gameObject.SetActive(false);
        _reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
        _reflectionProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;

        OnSelectionChange();
        OpenObject();

        SceneView.beforeSceneGui += SceneView_beforeSceneGui;
    }

    private void SceneView_beforeSceneGui(SceneView obj)
    {
        if (Event.current.type == EventType.Repaint)
        {
            if (_targetObject != null)
            {
                Handles.color = Handles.zAxisColor;
                Handles.ArrowHandleCap(0, _targetObject.transform.position, _targetObject.transform.rotation, 1f, EventType.Repaint);

                Handles.color = Handles.xAxisColor;
                Handles.ArrowHandleCap(1, _targetObject.transform.position, _targetObject.transform.rotation * Quaternion.Euler(0, -90, 0), 1f, EventType.Repaint);

                Handles.color = Handles.yAxisColor;
                Handles.ArrowHandleCap(2, _targetObject.transform.position, _targetObject.transform.rotation * Quaternion.Euler(-90, 0, 0), 1f, EventType.Repaint);
            }
        }
    }

    private void OnSelectionChange()
    {
        if(Selection.gameObjects != null && Selection.gameObjects.Length > 0)
        {
            SelectedObject = Selection.gameObjects[0];
            Repaint();
        }
    }

    private void OpenObject ()
    {
        if (SelectedObject == null)
            return;

        InitSelectedObject();
        Repaint();
    }

    private void InitSelectedObject ()
    {
        if(_targetObject != null)
        {
            DestroyImmediate(_targetObject);
        }

        _targetObject = Instantiate(SelectedObject, _snapShotCamera.transform, true);
        _targetObject.name = _targetObject.name.Replace("(Clone)", "");
        _targetObject.transform.localPosition = Vector3.zero;
        _targetObject.transform.localRotation = Quaternion.identity;
        
        _posOffset = new Vector3(_renderTexture.width * 0.5f, _renderTexture.height * 0.5f, _targetObject.GetComponentInChildren<Renderer>().bounds.max.magnitude * 2f);
    }

    private void OnGUI ()
    {
        SelectedObject = (GameObject)EditorGUILayout.ObjectField("Selected Object", SelectedObject, typeof(GameObject), false);

        if (SelectedObject != null)
        {
            if (GUILayout.Button("Open"))
            {
                OpenObject();
            }
        }

        if (_targetObject != null)
        {
            GUILayout.BeginVertical("HelpBox");

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Current Object", _targetObject, typeof(GameObject), false);
            GUI.enabled = true;

            EditorGUI.BeginChangeCheck();
            CamResolution = EditorGUILayout.Vector2IntField("Cam Resolution", CamResolution);
            GUILayout.Space(12);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = EditorGUILayout.ColorField(new GUIContent("Ambient"), RenderSettings.ambientLight, true, true, true);
            
            if(_reflectionProbe.gameObject.activeSelf)
            {
                if (GUILayout.Button("Off RefProbe"))
                {
                    _reflectionProbe.gameObject.SetActive(false);
                    Repaint();
                }
                if (GUILayout.Button("Update RefProbe"))
                {
                    _reflectionProbe.RenderProbe();
                    Repaint();
                }
            }
            else
            {
                if (GUILayout.Button("On RefProbe"))
                {
                    _reflectionProbe.gameObject.SetActive(true);
                    _reflectionProbe.RenderProbe();
                    Repaint();
                }
            }
            
            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                if (CamResolution.x < 1)
                    CamResolution.x = 1;

                if (CamResolution.y < 1)
                    CamResolution.y = 1;

                _renderTexture = new RenderTexture(CamResolution.x, CamResolution.y, 0);
                _snapShotCamera.targetTexture = _renderTexture;
                _snapShotCamera.Render();
            }

            GUILayout.BeginVertical("Box");

            _posOffset = EditorGUILayout.Vector3Field("Model Position Offset", _posOffset);
            _rotOffset = EditorGUILayout.Vector3Field("Model Rotation Offset", _rotOffset);
            UpdateTransform();

            GUILayout.Label("Move: Mouse Wheel" + System.Environment.NewLine + "Rotate: Y: Mouse1 | X: Mouse1 + Shift | Z: Mouse1 + Ctrl", "HelpBox");
            GUILayout.Box(_renderTexture);

            BackgroundColor = EditorGUILayout.ColorField(BackgroundColor);
            _snapShotCamera.backgroundColor = BackgroundColor;
            
            GUILayout.EndVertical();

            if (Event.current.button == 2)
            {
                Vector3 delta = Event.current.delta;
                _posOffset += new Vector3(delta.x, -delta.y, 0) * 0.5f;

                UpdateTransform();
                Repaint();
            }

            if (Event.current.button == 1)
            {
                Vector3 delta = Event.current.delta;

                if (Event.current.control)
                    _rotOffset += new Vector3(0, 0, delta.y) * 0.05f;
                else if (Event.current.shift)
                    _rotOffset += new Vector3(delta.y, 0, 0) * 0.05f;
                else
                    _rotOffset += new Vector3(0, -delta.x, 0) * 0.05f;

                UpdateTransform();
                Repaint();
            }

            if (Event.current.isScrollWheel)
            {
                Vector3 delta = Event.current.delta;
                _posOffset += new Vector3(0, 0, delta.y) * 0.05f;
                UpdateTransform();
                Repaint();
            }

            if (GUILayout.Button("Snap"))
            {
                CreateSnapshot();
            }
        }
    }

    private void UpdateTransform ()
    {
        _targetObject.transform.localPosition = _snapShotCamera.ScreenPointToRay(_posOffset).GetPoint(_posOffset.z);
        _targetObject.transform.localRotation = Quaternion.Euler(_rotOffset);
        _reflectionProbe.transform.position = _targetObject.transform.position;
    }

    private void CreateSnapshot()
    {
        _snapShotCamera.Render();
        RenderTexture.active = _renderTexture;
        Texture2D mapImg = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.ARGB32, false);
        mapImg.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
        mapImg.Apply();
        RenderTexture.active = null;

        byte[] imgData = mapImg.EncodeToPNG();

        string selectedPath = EditorUtility.SaveFilePanelInProject("Select save path", _targetObject.name + "_icon", "png", "meow");
        if (!string.IsNullOrEmpty(selectedPath))
        {
            if (File.Exists(selectedPath))
                File.Delete(selectedPath);

            using (FileStream fs = File.Create(selectedPath))
            {
                fs.Write(imgData, 0, imgData.Length);
            }
        }
    }

    private void OnDestroy()
    {
        if (_snapShotCamera != null)
            DestroyImmediate(_snapShotCamera.gameObject);

        if (_targetObject != null)
            DestroyImmediate(_targetObject);

        if (_snapshotLight != null)
            DestroyImmediate(_snapshotLight.gameObject);

        if (SnapShotScene != null)
            EditorSceneManager.UnloadSceneAsync(SnapShotScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        if (_reflectionProbe != null)
            DestroyImmediate(_reflectionProbe.gameObject);

        SceneView.beforeSceneGui -= SceneView_beforeSceneGui;

        EditorSceneManager.OpenScene(PrevScenePath, OpenSceneMode.Single);
    }
}

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class TerrainToDeformableGround : EditorWindow
{
    private SaveFormat _meshFormat = SaveFormat.Quads;
    private int _chunksCount = 1;

    private TerrainData _currentTerrainData;
    private Terrain _currentTerrain;
    private Vector3 _terrainPos;

    private int tCount;
    private int counter;
    private int totalCount;
    private int progressUpdateInterval = 10000;

    public Material ChunkMaterial;
    private string _materialName = "DeformableGround";

    private float _down = 0.001f;

    [MenuItem("Terrain/To DeformableGround")]
    private static void Open ()
    {
        EditorWindow window = GetWindow<TerrainToDeformableGround>();
        window.minSize = new Vector2(300, 150);
        window.maxSize = new Vector2(300, 150);
        window.Show();

    }

    private void OnEnable()
    {
        OnSelectionChanged();
        Selection.selectionChanged += OnSelectionChanged;

        string[] assets = AssetDatabase.FindAssets(_materialName);

        if (assets != null && assets.Length > 0)
        {
            foreach(string assetGUID in assets)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                if(assetPath.Contains(_materialName + ".mat"))
                {
                    ChunkMaterial = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                    break;
                }
            }
        }
    }

    private void OnDestroy()
    {
        Selection.selectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged ()
    {
        if (Selection.activeObject == null || !(Selection.activeObject is GameObject))
            return;

        Terrain selectedTerrain = (Selection.activeObject as GameObject).GetComponent<Terrain>();

        if (selectedTerrain != null)
        {
            if (_currentTerrain != selectedTerrain)
            {
                _currentTerrain = selectedTerrain;

                if (_currentTerrain)
                {
                    _currentTerrainData = _currentTerrain.terrainData;
                    _terrainPos = _currentTerrain.transform.position;
                }
            }
        }
        else
        {
            _currentTerrain = null;
            _currentTerrainData = null;
        }

        Repaint();
    }

    void OnGUI()
    {
        if (!_currentTerrain)
        {
            GUILayout.Label("Terrain not selected", "HelpBox");
            return;
        }

        GUILayout.Label("Selected: " + _currentTerrain.name, "HelpBox");
        GUILayout.Space(10);

        _meshFormat = (SaveFormat)EditorGUILayout.EnumPopup("Mesh Format", _meshFormat);

        if (_chunksCount > 1)
            _chunksCount = _chunksCount - _chunksCount % 2;

        _chunksCount = EditorGUILayout.IntSlider(_chunksCount, 1, 20);

        ChunkMaterial = (Material)EditorGUILayout.ObjectField("Chunk Material", ChunkMaterial, typeof(Material), false, null);

        _down = EditorGUILayout.FloatField("_down", _down);

        GUILayout.Space(10);

        if (GUILayout.Button("Create"))
        {
            int counter = 10;
            while(_currentTerrain.transform.childCount > 0)
            {
                counter--;
                if(_currentTerrain.transform.childCount > 0)
                    DestroyImmediate(_currentTerrain.transform.GetChild(0).gameObject);
            }

            Create();
        }
    }

    private void Create ()
    {
        string selectedFolderPath = EditorUtility.SaveFolderPanel("Export chunks to folder", "", Application.dataPath);

        string folderPath = Path.Combine(selectedFolderPath, _currentTerrain.name + "_Chunks");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        int heightMapWidht = _currentTerrainData.heightmapResolution;
        int heightMapHeight = _currentTerrainData.heightmapResolution;
        
        int chunkWidht = heightMapWidht / _chunksCount;
        int chunkHeight = heightMapHeight / _chunksCount;

        Vector3 meshScale = _currentTerrainData.size;

        meshScale = new Vector3(meshScale.x / (heightMapWidht - 1) * 1, meshScale.y, meshScale.z / (heightMapHeight - 1) * 1);
        Vector2 uvScale = new Vector2(1.0f / (chunkWidht + 2), 1.0f / (chunkHeight + 2));

        Vector3[] tVertices;
        Vector2[] tUV;
        int[] tPolys;

        for (int chY = 0; chY < _chunksCount; chY++)
        {
            for (int chX = 0; chX < _chunksCount; chX++)
            {
                int startX = chX * chunkWidht;
                int startY = chY * chunkHeight;

                int dataWidth = chX == _chunksCount - 1 ? heightMapWidht - startX : chunkWidht + 1;
                int dataHeight = chY == _chunksCount - 1 ? heightMapHeight - startY : chunkHeight + 1;
                
                float[,] tDataRaw = _currentTerrainData.GetHeights(startX, startY, dataWidth, dataHeight);

                dataWidth += 2;
                dataHeight += 2;

                float[,] tData = new float[dataWidth, dataHeight];

                float down = -0.0001f;

                for (int y = 0; y < dataHeight; y++)
                {
                    for (int x = 0; x < dataWidth; x++)
                    {
                        if (x == 0 || y == 0 || x == dataWidth - 1 || y == dataHeight - 1)
                        {
                            tData[x, y] = _down;// tDataRaw[Mathf.Clamp(x, 1, dataWidth - 3), Mathf.Clamp(y, 1, dataHeight - 3)] - down;
                        }
                        else
                        {
                            tData[x, y] = tDataRaw[x - 1, y - 1];
                        }
                    }
                }

                tVertices = new Vector3[dataWidth  * dataHeight];
                tUV = new Vector2[dataWidth * dataHeight];

                if (_meshFormat == SaveFormat.Triangles)
                    tPolys = new int[(dataWidth - 1) * (dataHeight - 1) * 6];
                else
                    tPolys = new int[(dataWidth - 1) * (dataHeight - 1) * 4];

                // Build vertices and UVs
                for (int y = 0; y < dataHeight; y++)
                {
                    for (int x = 0; x < dataWidth; x++)
                    {
                        float q = 0;

                        try
                        {
                            q = tData[x,y];
                        }
                        catch
                        {
                            Debug.LogError($"x:{x} y:{y} {tData.Length}");
                        }

                        Vector3 vPos = new Vector3(-y, q, x);

                        //if (y == 0)
                            //vPos.x = -y - 2;
                        
                        //if (x == 0)
                            //vPos.z = x + 2;

                        tVertices[y * dataWidth + x] = new Vector3(vPos.x * meshScale.x, vPos.y * meshScale.y, vPos.z * meshScale.z) + _terrainPos - new Vector3(-1, 0, -1);
                        tUV[y * dataWidth + x] = new Vector2(x * uvScale.x, y * uvScale.y);
                    }
                }

                int index = 0;
                if (_meshFormat == SaveFormat.Triangles)
                {
                    // Build triangle indices: 3 indices into vertex array for each triangle
                    for (int y = 0; y < dataHeight - 1; y++)
                    {
                        for (int x = 0; x < dataWidth - 1; x++)
                        {
                            // For each grid cell output two triangles
                            tPolys[index++] = (y * dataWidth) + x;
                            tPolys[index++] = ((y + 1) * dataWidth) + x;
                            tPolys[index++] = (y * dataWidth) + x + 1;

                            tPolys[index++] = ((y + 1) * dataWidth) + x;
                            tPolys[index++] = ((y + 1) * dataWidth) + x + 1;
                            tPolys[index++] = (y * dataWidth) + x + 1;
                        }
                    }
                }
                else
                {
                    // Build quad indices: 4 indices into vertex array for each quad
                    for (int y = 0; y < dataHeight - 1; y++)
                    {
                        for (int x = 0; x < dataWidth - 1; x++)
                        {
                            // For each grid cell output one quad
                            tPolys[index++] = (y * dataWidth) + x;
                            tPolys[index++] = ((y + 1) * dataWidth) + x;
                            tPolys[index++] = ((y + 1) * dataWidth) + x + 1;
                            tPolys[index++] = (y * dataWidth) + x + 1;
                        }
                    }
                }

                string chunkModelPath = Path.Combine(folderPath, "chunk_" + chX + "_" + chY + ".obj");
                ExportToObj(chunkModelPath, tVertices, tUV, tPolys);

                /*Vector3 chunkPos = new Vector3(chX * (_currentTerrainData.size.x / _chunksCount), -_terrainPos.y + 1, chY * (_currentTerrainData.size.z / _chunksCount));

                GameObject chunkObj = new GameObject("DGChunk " + chX + "-" + chY);

                chunkObj.transform.SetParent(_currentTerrain.transform);
                chunkObj.transform.localPosition = chunkPos;

                int assetsWordIndx = chunkModelPath.IndexOf("Assets");
                Debug.Log(chunkModelPath);
                string localAssetPath = chunkModelPath.Remove(0, assetsWordIndx);

                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(localAssetPath);

                Mesh chunkMesh = go.GetComponentInChildren<MeshFilter>().sharedMesh;

                chunkObj.AddComponent<MeshFilter>().mesh = chunkMesh;
                chunkObj.AddComponent<MeshRenderer>().sharedMaterial = ChunkMaterial;

                chunkObj.AddComponent<MeshCollider>();

                chunkObj.AddComponent<DeformableGround>();

                chunkObj.layer = 6;*/
            }
        }

        AssetDatabase.Refresh();

        /*_currentTerrain = null;
        EditorUtility.DisplayProgressBar("Saving file to disc.", "This might take a while...", 1f);
        EditorWindow.GetWindow<ExportTerrain>().Close();
        EditorUtility.ClearProgressBar();*/
    }

    private void UpdateProgress()
    {
        if (counter++ == progressUpdateInterval)
        {
            counter = 0;
            EditorUtility.DisplayProgressBar("Saving...", "", Mathf.InverseLerp(0, totalCount, ++tCount));
        }
    }

    private void ExportToObj (string filePath, Vector3[] tVertices, Vector2[] tUV, int[] tPolys)
    {
        using (StreamWriter sw = File.CreateText(filePath))// new StreamWriter(filePath))
        {
            try
            {
                // Write vertices
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                counter = tCount = 0;
                totalCount = (tVertices.Length * 2 + (_meshFormat == SaveFormat.Triangles ? tPolys.Length / 3 : tPolys.Length / 4)) / progressUpdateInterval;
                for (int i = 0; i < tVertices.Length; i++)
                {
                    UpdateProgress();
                    StringBuilder sb = new StringBuilder("v ", 20);
                    // StringBuilder stuff is done this way because it's faster than using the "{0} {1} {2}"etc. format
                    // Which is important when you're exporting huge terrains.
                    sb.Append(tVertices[i].x.ToString()).Append(" ").
                       Append(tVertices[i].y.ToString()).Append(" ").
                       Append(tVertices[i].z.ToString());
                    sw.WriteLine(sb);
                }
                // Write UVs
                for (int i = 0; i < tUV.Length; i++)
                {
                    UpdateProgress();
                    StringBuilder sb = new StringBuilder("vt ", 22);
                    sb.Append(tUV[i].x.ToString()).Append(" ").
                       Append(tUV[i].y.ToString());
                    sw.WriteLine(sb);
                }
                if (_meshFormat == SaveFormat.Triangles)
                {
                    // Write triangles
                    for (int i = 0; i < tPolys.Length; i += 3)
                    {
                        UpdateProgress();
                        StringBuilder sb = new StringBuilder("f ", 43);
                        sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                           Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                           Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1);
                        sw.WriteLine(sb);
                    }
                }
                else
                {
                    // Write quads
                    for (int i = 0; i < tPolys.Length; i += 4)
                    {
                        UpdateProgress();
                        StringBuilder sb = new StringBuilder("f ", 57);
                        sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                           Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                           Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1).Append(" ").
                           Append(tPolys[i + 3] + 1).Append("/").Append(tPolys[i + 3] + 1);
                        sw.WriteLine(sb);
                    }
                }
            }
            catch (Exception err)
            {
                Debug.Log("Error saving file: " + err.Message);
            }
        }
    }
}

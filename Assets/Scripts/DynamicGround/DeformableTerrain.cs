using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DeformableTerrain))]
public class DeformableTerrain_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Preview DeformMap"))
        {
            ((DeformableTerrain)target).PreviewDeformMap();
        }
    }
}
#endif

public class DeformableTerrain : DeformableGround
{

#if UNITY_EDITOR
    private void OnValidate()
    {
        PreviewDeformMap();
    }

    public void PreviewDeformMap()
    {
        Terrain _renderer = this.GetComponent<Terrain>();
        Texture2D initMap = _renderer.terrainData.alphamapTextures[this.UsedSplatMapIndex];

        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        _renderer.GetSplatMaterialPropertyBlock(propertyBlock);

        propertyBlock.SetFloat(DeformableTerrain.DEFORM_HEIGHT_NAME, this.MaxHeight);
        propertyBlock.SetTexture(DeformableTerrain.DEFORM_MAP_NAME, initMap);

        _renderer.SetSplatMaterialPropertyBlock(propertyBlock);
    }
#endif

    protected new Terrain _renderer;

    [Header("Terrain")]
    public int UsedSplatMapIndex = 0;

    protected override void Awake()
    {
        _renderer = this.GetComponent<Terrain>();

        PixelsInMeter = MeterToPixels(1);

        _objectSizeX = GetObjectSize() / this.transform.localScale.x;

        UpdateMatrix();

        _resultHeightBuffer = new ComputeBuffer(2, sizeof(float));

        _heightMap = new RenderTexture(HeightMapSize.x, HeightMapSize.y, HeightMapSize.z, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        _heightMap.enableRandomWrite = true;
        _heightMap.Create();

        _heightMapBuffer = new RenderTexture(HeightMapSize.x, HeightMapSize.y, HeightMapSize.z, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        _heightMapBuffer.enableRandomWrite = true;
        _heightMapBuffer.Create();

        if (InitHeightMap == null)
        {
            Texture2D initMap = _renderer.terrainData.alphamapTextures[UsedSplatMapIndex];
            Graphics.Blit(initMap, _heightMap);
        }
        else
        {
            _startHeightMap = InitHeightMap;
            Graphics.Blit(_startHeightMap, _heightMap);
        }

        Graphics.Blit(_heightMap, _heightMapBuffer);

        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        _renderer.GetSplatMaterialPropertyBlock(propertyBlock);

        propertyBlock.SetFloat("_DeformHeight", MaxHeight);
        propertyBlock.SetTexture(DEFORM_MAP_NAME, _heightMap);

        _renderer.SetSplatMaterialPropertyBlock(propertyBlock);
    }

    private void OnDestroy()
    {
        _heightMap.Release();
        _resultHeightBuffer.Release();
    }

    public override float MeterToPixels(float meters)
    {
        float sizeX = _renderer.terrainData.size.x;

        float pixelsInUnit = HeightMapSize.x / sizeX;

        return meters * pixelsInUnit;
    }

    public override float GetObjectSize()
    {
        return _renderer.terrainData.bounds.size.x;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (_heightMap != null
            && UnityEditor.Selection.objects != null
            && UnityEditor.Selection.objects.Length == 1
            && UnityEditor.Selection.objects[0] == this.gameObject)

        {
            GUILayout.Box(_heightMap, GUILayout.Width(512), GUILayout.Height(512));
        }
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableTerrain2 : MonoBehaviour
{
    public Terrain Renderer;
    public Texture2D[] HeightMap;
    private int _mapIndex = 0;

    private void ApplyMap (int index)
    {
        Graphics.Blit(HeightMap[index], Renderer.terrainData.heightmapTexture);
        Renderer.terrainData.DirtyHeightmapRegion(new RectInt(0, 0, 512, 512), TerrainHeightmapSyncControl.HeightAndLod);
    }

    public void ApplyMap (RenderTexture source)
    {
        Graphics.Blit(source, Renderer.terrainData.heightmapTexture);
        Renderer.terrainData.DirtyHeightmapRegion(new RectInt(0, 0, 512, 512), TerrainHeightmapSyncControl.HeightAndLod);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            _mapIndex++;

            if (_mapIndex >= HeightMap.Length)
                _mapIndex = 0;

            ApplyMap(_mapIndex);
        }
    }
}

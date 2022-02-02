using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableTerrain : MonoBehaviour
{
    public enum DeformProfileType
    {
        Straight,
        Pow,
        Profile
    }

    private Terrain _renderer;
    private TerrainCollider _collider;
    private TerrainData _terrainData;

    private void Awake()
    {
        _renderer = this.GetComponent<Terrain>();
        _collider = _renderer.GetComponent<TerrainCollider>();

        _terrainData = Instantiate(_renderer.terrainData);
        _renderer.terrainData = _terrainData;
        _collider.terrainData = _terrainData;
    }

    public float HeightToTerrainHeight(float height)
    {
        return height / _terrainData.size.y;
    }

    public Vector2 WorldToUVPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = this.transform.InverseTransformPoint(worldPosition);

        Vector2 uv;
        uv.x = localPosition.x / _terrainData.size.x;
        uv.y = localPosition.z / _terrainData.size.y;

        return uv;
    }

    public void ApplyDeform(Vector3 uvPosition, float radius, float height, int paintInLayer = -1)
    {
        ApplyDeform(uvPosition, radius, height, paintInLayer, DeformProfileType.Straight);
    }

    public void ApplyDeformPow(Vector3 uvPosition, float radius, float height, int paintInLayer = -1)
    {
        ApplyDeform(uvPosition, radius, height, paintInLayer, DeformProfileType.Pow);
    }

    public void ApplyDeformProfile(Vector3 uvPosition, float radius, float height, AnimationCurve profileCurve, int paintInLayer = -1)
    {
        ApplyDeform(uvPosition, radius, height, paintInLayer, DeformProfileType.Profile, profileCurve);
    }

    public void ApplyDeform(Vector2 uvPosition, float radius, float height, int paintInLayer, DeformProfileType profileType, AnimationCurve profileCurve = null)
    {
        float tHeight = HeightToTerrainHeight(height);
        int tRadius = Mathf.RoundToInt(_terrainData.heightmapResolution / _terrainData.size.x * radius);
        int tDiametr = tRadius * 2;
        
        int hitX = Mathf.RoundToInt(uvPosition.x * _terrainData.heightmapResolution);
        int hitY = Mathf.RoundToInt(uvPosition.y * _terrainData.heightmapResolution);

        int startX = hitX - tRadius;
        int startY = hitY - tRadius;
        
        float[,] heights = _terrainData.GetHeights(startX, startY, tDiametr, tDiametr);

        for (int y = 0; y < tDiametr; y++)
        {
            for (int x = 0; x < tDiametr; x++)
            {
                float distance = Vector2.Distance(new Vector2(y, x), new Vector2(tRadius, tRadius));
                if (distance <= tRadius)
                {
                    float power = 1;

                    if(profileType == DeformProfileType.Straight)
                    {
                        power = 1;
                    }
                    else if(profileType == DeformProfileType.Pow)
                    {
                        power = 1 - Mathf.Pow(distance / radius, 2);
                    }
                    else if(profileType == DeformProfileType.Profile)
                    {
                        power = 1 - profileCurve.Evaluate(distance / tRadius);
                    }
                    
                    heights[x, y] += tHeight * power;
                }
            }
        }

        _terrainData.SetHeights(startX, startY, heights);

        //-------------------------------------------------------------

        if (paintInLayer > -1 && paintInLayer < _terrainData.alphamapLayers)
        {
            tRadius = Mathf.RoundToInt(_terrainData.alphamapResolution / _terrainData.size.x * radius);
            tDiametr = tRadius * 2;

            hitX = Mathf.RoundToInt(uvPosition.x * _terrainData.alphamapResolution);
            hitY = Mathf.RoundToInt(uvPosition.y * _terrainData.alphamapResolution);

            startX = hitX - tRadius;
            startY = hitY - tRadius;

            float[,,] alphaMaps = _terrainData.GetAlphamaps(0, 0, _terrainData.alphamapResolution, _terrainData.alphamapResolution);

            for (int y = 0; y < tDiametr; y++)
            {
                for (int x = 0; x < tDiametr; x++)
                {
                    for (int l = 0; l < _terrainData.alphamapLayers; l++)
                    {
                        Vector2Int amCoords = new Vector2Int(startY + y, startX + x);
                        if (Vector2.Distance(new Vector2(y, x), new Vector2(tRadius, tRadius)) <= tRadius)
                        {
                            if (l == paintInLayer)
                                alphaMaps[amCoords.x, amCoords.y, l] = 1;
                            else
                                alphaMaps[amCoords.x, amCoords.y, l] = 0;
                        }
                    }
                }
            }

            _terrainData.SetAlphamaps(0, 0, alphaMaps);
        }

        //-----------------------------------------------------------

        tRadius = Mathf.RoundToInt(_terrainData.detailResolution / _terrainData.size.x * radius);
        tDiametr = tRadius * 2;

        hitX = Mathf.RoundToInt(uvPosition.x * _terrainData.detailResolution);
        hitY = Mathf.RoundToInt(uvPosition.y * _terrainData.detailResolution);

        startX = hitX - tRadius;
        startY = hitY - tRadius;

        for (int l = 0; l < _terrainData.detailPrototypes.Length; l++)
        {
            int[,] details = _terrainData.GetDetailLayer(startX, startY, tDiametr, tDiametr, 0);
            
            for (int y = 0; y < tDiametr; y++)
            {
                for (int x = 0; x < tDiametr; x++)
                {
                    if (Vector2.Distance(new Vector2(y, x), new Vector2(tRadius, tRadius)) <= tRadius)
                        details[x, y] = 0;
                }
            }

            _terrainData.SetDetailLayer(startX, startY, l, details);
        }
    }
}

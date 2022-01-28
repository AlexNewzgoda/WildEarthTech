using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushTest : MonoBehaviour
{

    public ComputeShader CShader;

    public Texture2D BrushTexture;
    public float BrushScale = 1;
    public float Angle = 0;
    public Vector2 Pos;
    public Vector2 Offset;
    public RenderTexture CanvasTexture;

    public void ApplyBrush ()
    {
        Vector2Int changeSize = new Vector2Int(Mathf.CeilToInt(BrushTexture.width * BrushScale), Mathf.CeilToInt(BrushTexture.height * BrushScale));

        int processingSize = changeSize.x + changeSize.y % 8;

        if (processingSize < 8)
            return;

        CShader.SetTexture(0, "Result", CanvasTexture);
        CShader.SetTexture(0, "Brush", BrushTexture);
        CShader.SetFloat("BrushScale", BrushScale);
        CShader.SetInt("BrushSize", BrushTexture.width);
        CShader.SetFloat("Angle", Angle * Mathf.Deg2Rad);
        CShader.SetVector("Pos", Pos);
        CShader.SetVector("Offset", Offset);

        CShader.Dispatch(0, processingSize / 8, processingSize / 8, 1);
    }

    public void ClearCanvas ()
    {
        CShader.SetTexture(1, "Result", CanvasTexture);
        CShader.Dispatch(1, CanvasTexture.width / 8, CanvasTexture.height / 8, 1);
    }


    private void Start()
    {
        CanvasTexture = new RenderTexture(512, 512, 1, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        CanvasTexture.enableRandomWrite = true;
        CanvasTexture.Create();
    }

    private void OnDestroy()
    {
        if(CanvasTexture != null)
        {
            CanvasTexture.Release();
        }
    }

    private void Update()
    {
        ClearCanvas();
        ApplyBrush();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyBrush();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearCanvas();
        }
    }

    private void OnGUI()
    {
        if (CanvasTexture != null)
        {
            GUILayout.Box(CanvasTexture, GUILayout.Width(CanvasTexture.width), GUILayout.Height(CanvasTexture.height));
        }

        if(BrushTexture != null)
        {
            GUILayout.Box(BrushTexture, GUILayout.Width(BrushTexture.width), GUILayout.Height(BrushTexture.height));
        }
    }
}

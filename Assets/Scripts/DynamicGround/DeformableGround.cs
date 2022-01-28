using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DeformableGround))]
public class DeformableGround_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Preview DeformMap"))
        {
            DeformableGround _target = (DeformableGround)target;
            Renderer _renderer = _target.GetComponent<Renderer>();

            RenderTexture previewHeightMap = new RenderTexture(_target.HeightMapSize.x, _target.HeightMapSize.y, _target.HeightMapSize.z);

            Graphics.Blit(_target.InitHeightMap, previewHeightMap);

            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetFloat("_DeformHeight", _target.MaxHeight);
            propertyBlock.SetTexture(DeformableGround.DEFORM_MAP_NAME, previewHeightMap);

            _renderer.SetPropertyBlock(propertyBlock, 0);
        }
    }
}
#endif

public class DeformableGround : MonoBehaviour
{
    protected static Texture2D _initSimpleHeigtMap;
    public const string DEFORM_MAP_NAME = "_DeformMap";
    public const string DEFORM_HEIGHT_NAME = "_DeformHeight";

    [SerializeField]
    protected RenderTexture _heightMap;
    [SerializeField]
    protected RenderTexture _heightMapBuffer;
    public float MaxHeight = 1;

    public Vector3Int HeightMapSize = new Vector3Int(512, 512, 1);

    protected Matrix4x4 _worldToLocalMatrix;
    protected float _objectSizeX;
    public float PixelsInMeter { get; protected set; }

    protected Renderer _renderer;
    protected MeshFilter _meshFilter;

    protected ComputeBuffer _resultHeightBuffer;
    protected NativeArray<float> _clearHeightData;

    [Header("Init Settings")]
    public Texture2D InitHeightMap;
    protected Texture2D _startHeightMap;

    [Header("Auto Self Recover")]
    public bool IsRecoverable = false;
    public float RecoverSpeed = 1f;
    [Range(0.005f, 1)]
    public float RecoverStep = 0.005f;
    protected float _recoverValue = 0;
    protected float _recoverTimer = 0;

    public bool EnableDebug = false;

    protected virtual void Awake ()
    {
        _renderer = this.GetComponent<Renderer>();
        _meshFilter = this.GetComponent<MeshFilter>();

        Bounds newBounds = _meshFilter.mesh.bounds;
        newBounds.size += Vector3.up * 1.5f;
        newBounds.center += Vector3.up * 0.5f;
        _meshFilter.mesh.bounds = newBounds;

        PixelsInMeter = MeterToPixels(1);
        _objectSizeX = GetObjectSize() / this.transform.localScale.x;
        UpdateMatrix();

        //Init height buffer---------------------------------------
        _clearHeightData = new NativeArray<float>(new float[2] { 0f, 0f }, Allocator.Persistent);
        _resultHeightBuffer = new ComputeBuffer(2, sizeof(float));

        //Create HeightMap---------------------------------------
        _heightMap = new RenderTexture(HeightMapSize.x, HeightMapSize.y, HeightMapSize.z, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        _heightMap.enableRandomWrite = true;
        _heightMap.Create();

        _heightMapBuffer = new RenderTexture(_heightMap);
        _heightMapBuffer.enableRandomWrite = true;
        _heightMapBuffer.Create();

        if (InitHeightMap == null)
        {
            if (_initSimpleHeigtMap == null)
            {
                _initSimpleHeigtMap = new Texture2D(1, 1, TextureFormat.ARGB32, 0, false);
                _initSimpleHeigtMap.SetPixel(0, 0, new Color(1, 0, 0, 1));
                _initSimpleHeigtMap.Apply();
            }

            _startHeightMap = _initSimpleHeigtMap;
            Graphics.Blit(_initSimpleHeigtMap, _heightMap);
        }
        else
        {
            _startHeightMap = InitHeightMap;
            Graphics.Blit(_startHeightMap, _heightMap);
        }

        Graphics.Blit(_heightMap, _heightMapBuffer);

        //Set HeightMap---------------------------------------
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat("_DeformHeight", MaxHeight);
        propertyBlock.SetTexture(DEFORM_MAP_NAME, _heightMap);

        _renderer.SetPropertyBlock(propertyBlock, 0);
    }

    private void FixedUpdate()
    {
        if (IsRecoverable)
        {
            if (_recoverValue < 1.1f)
            {
                if (_recoverTimer > 1f)
                {
                    _recoverTimer -= 1f;

                    Recover(RecoverStep);

                    _recoverValue += RecoverStep;
                }

                _recoverTimer += Time.deltaTime * RecoverSpeed;
            }
        }
    }

    private void OnDestroy()
    {
        _heightMap.Release();
        Destroy(_heightMap);

        _resultHeightBuffer.Release();
        _clearHeightData.Dispose();
    }

    private void CopyTexture (RenderTexture source, RenderTexture target, Vector2 offset, Vector2 size)
    {
        GPUDeformerEntry.CopyTexture(source, target, offset, size);
    }

    #region Actions
    private void GetHeightInternal (Vector2 uv, float radius = 0.01f)
    {
        int sizeInPixelsHalf = Mathf.CeilToInt(PixelsInMeter * radius);
        int sizeInPixels = sizeInPixelsHalf * 2;
        Vector4 getPos = new Vector4(uv.x * HeightMapSize.x, uv.y * HeightMapSize.y, 0, 0);
        int processingSize = Mathf.Max(sizeInPixels, 8);

        int kernelHandle = GPUDeformerEntry.Instance.GetHeightValue_Kernel;

        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, GPUDeformerEntry.sp_Result, _heightMap);

        GPUDeformerEntry.Instance.DeformCShader.SetVector(GPUDeformerEntry.sp_Pos, getPos);
        GPUDeformerEntry.Instance.DeformCShader.SetVector(GPUDeformerEntry.sp_DeformOffset, new Vector4(getPos.x - sizeInPixelsHalf, getPos.y - sizeInPixelsHalf, 0, 0));
        GPUDeformerEntry.Instance.DeformCShader.SetFloat(GPUDeformerEntry.sp_DeformRadius, sizeInPixelsHalf);

        _resultHeightBuffer.SetData(_clearHeightData);

        GPUDeformerEntry.Instance.DeformCShader.SetBuffer(kernelHandle, GPUDeformerEntry.sp_ResultBuffer, _resultHeightBuffer);

        GPUDeformerEntry.Instance.DeformCShader.Dispatch(kernelHandle, processingSize / 8, processingSize / 8, 1);
    }

    public void GetHeightAsync (Vector2 uv, System.Action<float> result, float radius = 0.01f)
    {
        GetHeightInternal(uv, radius);
        AsyncGPUReadbackRequest readbackRequest = AsyncGPUReadback.Request(_resultHeightBuffer, (x) => GetHeightCallback(x, result));
    }

    public float GetHeight (Vector2 uv, float radius = 0.01f)
    {
        GetHeightInternal(uv, radius);
        AsyncGPUReadbackRequest readbackRequest = AsyncGPUReadback.Request(_resultHeightBuffer);
        readbackRequest.WaitForCompletion();

        NativeArray<float> data = readbackRequest.GetData<float>();

        float count = data[1];

        return count > 0 ? data[0] / count : 0;
    }

    private void GetHeightCallback(AsyncGPUReadbackRequest request, System.Action<float> height)
    {
        NativeArray<float> data = request.GetData<float>();

        float count = data[1];

        height?.Invoke(count > 0 ? data[0] / count : 0);

        data.Dispose();
    }

    public void ApplyDeformSphere (Vector2 uv, float radius = 1, float power = 1, float depth = 1)
    {
        int sizeInPixelsHalf = Mathf.CeilToInt(PixelsInMeter * radius);
        int sizeInPixels = sizeInPixelsHalf * 2;
        Vector4 drawPos = new Vector4(uv.x * HeightMapSize.x, uv.y * HeightMapSize.y, 0, 0);
        int processingSize = Mathf.Max(sizeInPixels + sizeInPixels % 8, 8);

        int kernelHandle = GPUDeformerEntry.Instance.Deform_Kernel;

        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, GPUDeformerEntry.sp_Result, _heightMap);
        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, GPUDeformerEntry.sp_ReferenceTex, _heightMapBuffer);

        GPUDeformerEntry.Instance.DeformCShader.SetVector(GPUDeformerEntry.sp_Pos, drawPos);
        GPUDeformerEntry.Instance.DeformCShader.SetFloat(GPUDeformerEntry.sp_DeformDepth, depth);
        GPUDeformerEntry.Instance.DeformCShader.SetFloat(GPUDeformerEntry.sp_DeformPower, power);
        GPUDeformerEntry.Instance.DeformCShader.SetFloat(GPUDeformerEntry.sp_DeformRadius, sizeInPixelsHalf);
        GPUDeformerEntry.Instance.DeformCShader.SetVector(GPUDeformerEntry.sp_DeformOffset, new Vector4(drawPos.x - sizeInPixelsHalf, drawPos.y - sizeInPixelsHalf, 0, 0));

        GPUDeformerEntry.Instance.DeformCShader.Dispatch(kernelHandle, processingSize / 8, processingSize / 8, 1);

        CopyTexture(_heightMap, _heightMapBuffer, drawPos - new Vector4(sizeInPixelsHalf, sizeInPixelsHalf, 0, 0), new Vector2(processingSize, processingSize));

       _recoverValue = -0.1f;
    }

    public void ApplyBrushDeform(Vector2 uv, float angle, float scale, Texture brushTexture)
    {
        if (brushTexture == null)
            throw new System.Exception("brushTexture is NULL");

        Vector4 drawPos = new Vector4(uv.x * HeightMapSize.x, uv.y * HeightMapSize.y, 0, 0);

        Vector2Int brushSize = new Vector2Int(
                Mathf.RoundToInt(brushTexture.width * scale),
                Mathf.RoundToInt(brushTexture.height * scale)
                );

        Vector2 scaledHalfSize = brushSize / 2;

        int kernelHandle = GPUDeformerEntry.Instance.BrushDeform_Kernel;

        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, GPUDeformerEntry.sp_Result, _heightMap);
        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, GPUDeformerEntry.sp_ReferenceTex, _heightMapBuffer);
        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, GPUDeformerEntry.sp_BrushTex, brushTexture);

        GPUDeformerEntry.Instance.DeformCShader.SetVector(GPUDeformerEntry.sp_Pos, drawPos);

        GPUDeformerEntry.Instance.DeformCShader.SetFloat(GPUDeformerEntry.sp_Angle, angle * Mathf.Deg2Rad);
        GPUDeformerEntry.Instance.DeformCShader.SetFloat(GPUDeformerEntry.sp_BrushScale, scale);
        GPUDeformerEntry.Instance.DeformCShader.SetInt(GPUDeformerEntry.sp_BrushHalfSize, (int)(brushTexture.width * 0.5f));

        GPUDeformerEntry.Instance.DeformCShader.Dispatch(kernelHandle, brushSize.x / 8, brushSize.y / 8, 1);

        CopyTexture(_heightMap, _heightMapBuffer, drawPos - new Vector4(scaledHalfSize.x, scaledHalfSize.y), new Vector2(brushSize.x, brushSize.y));

        _recoverValue = -0.1f;
    }

    public void ApplyBrushSimpleDeform(Vector2 uv, Texture brushTexture, float scale, bool mirrorY = false)
    {
        if (brushTexture == null)
            throw new System.Exception("brushTexture is NULL");

        Vector4 drawPos = new Vector4(uv.x * HeightMapSize.x, uv.y * HeightMapSize.y, 0, 0);
        Vector2Int brushSize = new Vector2Int(
                Mathf.RoundToInt(brushTexture.width * scale),
                Mathf.RoundToInt(brushTexture.height * scale)
                );

        Vector2 scaledHalfSize = brushSize / 2;

        int kernelHandle = GPUDeformerEntry.Instance.BrushDeformSimple_Kernel;

        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, GPUDeformerEntry.sp_Result, _heightMap);
        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, GPUDeformerEntry.sp_ReferenceTex, _heightMapBuffer);
        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, GPUDeformerEntry.sp_BrushTex, brushTexture);

        GPUDeformerEntry.Instance.DeformCShader.SetVector(GPUDeformerEntry.sp_Pos, drawPos);

        GPUDeformerEntry.Instance.DeformCShader.SetFloat(GPUDeformerEntry.sp_BrushScale, scale);
        GPUDeformerEntry.Instance.DeformCShader.SetBool(GPUDeformerEntry.sp_BrushMirrorY, mirrorY);
        GPUDeformerEntry.Instance.DeformCShader.SetInt(GPUDeformerEntry.sp_BrushHalfSize, (int)(brushTexture.width * 0.5f));

        GPUDeformerEntry.Instance.DeformCShader.Dispatch(kernelHandle, brushSize.x / 8, brushSize.y / 8, 1);

        CopyTexture(_heightMap, _heightMapBuffer, drawPos - new Vector4(scaledHalfSize.x, scaledHalfSize.y), new Vector2(brushSize.x, brushSize.y));

        _recoverValue = -0.1f;
    }

    //NotOptimized-------------------------------
    public void Recover()
    {
        Recover(1);
    }

    public void Recover(float step = 0.005f)
    {
        int kernelHandle = GPUDeformerEntry.Instance.ReduceUpdate_Kernel;

        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, GPUDeformerEntry.sp_Result, _heightMap);
        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, GPUDeformerEntry.sp_ReferenceTex, _heightMapBuffer);
        GPUDeformerEntry.Instance.DeformCShader.SetTexture(kernelHandle, "ResultTarget", _startHeightMap);

        GPUDeformerEntry.Instance.DeformCShader.SetFloat("Reduce", step);
        GPUDeformerEntry.Instance.DeformCShader.Dispatch(kernelHandle, HeightMapSize.x / 8, HeightMapSize.y / 8, 1);

        CopyTexture(_heightMap, _heightMapBuffer, Vector2.zero, new Vector2(HeightMapSize.x, HeightMapSize.y));
    }

    public void ApplyDeformRectangle_NotOptimized(Vector2 uv, Vector2 uv2, Vector2 uv3, Vector2 uv4, float depth = 1)
    {
        int kernelHandle = GPUDeformerEntry.Instance.DeformRectangle_Kernel;

        GPUDeformerEntry.Instance.DeformCShader.SetTexture(GPUDeformerEntry.Instance.DeformRectangle_Kernel, "Result", _heightMap);

        GPUDeformerEntry.Instance.DeformCShader.SetFloat("Depth", depth);

        GPUDeformerEntry.Instance.DeformCShader.SetVector("Pos1", new Vector4(uv.x * HeightMapSize.x, uv.y * HeightMapSize.y, 0, 0));
        GPUDeformerEntry.Instance.DeformCShader.SetVector("Pos2", new Vector4(uv2.x * HeightMapSize.x, uv2.y * HeightMapSize.y, 0, 0));
        GPUDeformerEntry.Instance.DeformCShader.SetVector("Pos3", new Vector4(uv3.x * HeightMapSize.x, uv3.y * HeightMapSize.y, 0, 0));
        GPUDeformerEntry.Instance.DeformCShader.SetVector("Pos4", new Vector4(uv4.x * HeightMapSize.x, uv4.y * HeightMapSize.y, 0, 0));

        GPUDeformerEntry.Instance.DeformCShader.Dispatch(kernelHandle, HeightMapSize.x / 8, HeightMapSize.y / 8, 1);

        _recoverValue = -0.1f;
    }
    //------------------------------------------------------------------

    #endregion


    public void UpdateMatrix()
    {
        _worldToLocalMatrix = this.transform.worldToLocalMatrix;
    }

    public virtual float GetObjectSize()
    {
        return _renderer.bounds.size.x;
    }

    public virtual float MeterToPixels(float meters)
    {
        float sizeX = GetObjectSize();

        float pixelsInUnit = HeightMapSize.x / sizeX;

        return meters * pixelsInUnit;
    }

    public virtual Vector2 WorldToUVCoord(Vector3 worldPos)
    {
        Vector3 localPos = _worldToLocalMatrix.MultiplyPoint(worldPos);
        localPos.y = localPos.z;
        localPos.z = 0;

        float size = _objectSizeX;

        localPos = localPos / size + new Vector3(0.5f, 0.5f, 0);

        localPos.x = 1 - localPos.x;
        localPos.y = 1 - localPos.y;

        return localPos;
    }

    private void OnGUI()
    {
#if UNITY_EDITOR
        if (EnableDebug)
        {
            GUILayout.Box(_heightMap);
            GUILayout.Box(_heightMapBuffer);

            if (Input.GetKeyDown(KeyCode.G))
            {
                Texture2D img = new Texture2D(_heightMap.width, _heightMap.height);

                RenderTexture.active = _heightMap;

                img.ReadPixels(new Rect(0, 0, _heightMap.width, _heightMap.height), 0, 0);

                img.Apply();

                RenderTexture.active = null;

                byte[] data = img.EncodeToPNG();

                string filePath = UnityEditor.EditorUtility.SaveFilePanelInProject("save", "heightMap", "png", "save");

                System.IO.File.WriteAllBytes(filePath, data);
            }
        }
#endif
    }
}

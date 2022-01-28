using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDepthDeformer : MonoBehaviour
{

    public Camera BrushCamera;
    private RenderTexture _brushTexture;

    public Vector3Int BrushSize = new Vector3Int(64, 64, 1);
    public FilterMode BrushFilterMode = FilterMode.Point;
    public float RenderSize = 1;

    private float _pixelsInMeter = 0;

    [Space]
    public bool EnableDebug = false;

    private void Awake()
    {
        _brushTexture = new RenderTexture(BrushSize.x, BrushSize.y, BrushSize.z, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
        _brushTexture.filterMode = BrushFilterMode;
        _brushTexture.Create();

        BrushCamera.targetTexture = _brushTexture;
        BrushCamera.enabled = false;
        BrushCamera.orthographicSize = RenderSize;
        BrushCamera.renderingPath = RenderingPath.VertexLit;
        BrushCamera.allowHDR = false;
        BrushCamera.allowMSAA = false;
        BrushCamera.depthTextureMode = DepthTextureMode.Depth;
        BrushCamera.eventMask = 0;
        BrushCamera.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.NoDistanceSort;

        _pixelsInMeter = BrushSize.x / BrushCamera.orthographicSize;
    }

    private void OnDestroy()
    {
        if(_brushTexture != null)
        {
            _brushTexture.Release();
        }
    }

    private void LateUpdate()
    {
        RaycastHit rhit;
        if (Physics.Raycast(this.transform.position, Vector3.down, out rhit, 2f, 1<<6, QueryTriggerInteraction.Ignore))
        {
            DeformableGround deformableGround;
            if (rhit.collider.TryGetComponent(out deformableGround))
            {
                BrushCamera.transform.position = rhit.point - Vector3.up * 0.01f;
                BrushCamera.transform.forward = Vector3.up;
                BrushCamera.transform.rotation = Quaternion.LookRotation(rhit.normal, BrushCamera.transform.up); Quaternion.Euler(-90, 0, 0);
                BrushCamera.Render();

                float scale = deformableGround.PixelsInMeter / _pixelsInMeter;
                deformableGround.ApplyBrushSimpleDeform(rhit.textureCoord, _brushTexture, scale * 2, true);
            }

            DeformableTerrain2 deformableTerrain2;
            if (rhit.collider.TryGetComponent(out deformableTerrain2))
            {
                BrushCamera.transform.position = rhit.point - Vector3.up * 0.01f;
                BrushCamera.transform.forward = Vector3.up;
                BrushCamera.transform.rotation = Quaternion.LookRotation(rhit.normal, BrushCamera.transform.up); Quaternion.Euler(-90, 0, 0);
                BrushCamera.Render();
                
                deformableTerrain2.ApplyMap(_brushTexture);
            }
        }
    }

    private void OnGUI()
    {
#if UNITY_EDITOR
        if (EnableDebug)
        {
            if (_brushTexture != null)
                GUILayout.Box(_brushTexture, GUILayout.Width(_brushTexture.width), GUILayout.Height(_brushTexture.height));

            BrushCamera.orthographicSize = RenderSize;
            _pixelsInMeter = BrushSize.x / BrushCamera.orthographicSize;
        }
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUDeformerEntry : MonoBehaviour
{
    public static GPUDeformerEntry Instance { get; private set; }

    public ComputeShader TextureCopyCShader;
    public int TextureCopy_Kernel { get; private set; }
    private static int sp_Source = Shader.PropertyToID("Source");
    private static int sp_Target = Shader.PropertyToID("Target");
    private static int sp_Offset = Shader.PropertyToID("Offset");

    public ComputeShader DeformCShader;
    public int Deform_Kernel { get; private set; }
    public int DeformRectangle_Kernel { get; private set; }
    public int ReduceUpdate_Kernel { get; private set; }
    public int BrushDeform_Kernel { get; private set; }
    public int BrushDeformSimple_Kernel { get; private set; }
    public int GetHeightValue_Kernel { get; private set; }

    #region Deform Shader Properties
    public static int sp_Result = Shader.PropertyToID("Result");
    public static int sp_ReferenceTex = Shader.PropertyToID("ReferenceTex");

    public static int sp_ResultBuffer = Shader.PropertyToID("ResultBuffer");

    public static int sp_Pos = Shader.PropertyToID("Pos");
    public static int sp_Angle = Shader.PropertyToID("Angle");

    public static int sp_DeformDepth = Shader.PropertyToID("DeformDepth");
    public static int sp_DeformPower = Shader.PropertyToID("DeformPower");
    public static int sp_DeformRadius = Shader.PropertyToID("DeformRadius");
    public static int sp_DeformOffset = Shader.PropertyToID("DeformOffset");

    public static int sp_BrushTex = Shader.PropertyToID("BrushTex");
    public static int sp_BrushScale = Shader.PropertyToID("BrushScale");
    public static int sp_BrushHalfSize = Shader.PropertyToID("BrushHalfSize");
    public static int sp_BrushMirrorY = Shader.PropertyToID("BrushMirrorY");
    #endregion

    private void Awake()
    {
        TextureCopy_Kernel = TextureCopyCShader.FindKernel("Copy");

        Deform_Kernel = DeformCShader.FindKernel("Deform");
        ReduceUpdate_Kernel = DeformCShader.FindKernel("ReduceUpdate");
        DeformRectangle_Kernel = DeformCShader.FindKernel("DeformRectangle");
        BrushDeform_Kernel = DeformCShader.FindKernel("BrushDeform");
        BrushDeformSimple_Kernel = DeformCShader.FindKernel("BrushDeformSimple");
        GetHeightValue_Kernel = DeformCShader.FindKernel("GetHeightValue");

        if (Instance == null)
            Instance = this;
    }

    public static void CopyTexture(Texture source, Texture target)
    {
        CopyTexture(source, target, Vector2.zero, new Vector2(source.width, source.height));
    }

    public static void CopyTexture (Texture source, Texture target, Vector2 offset, Vector2 size)
    {
        if (size.x < 1 || size.y < 1)
            throw new System.Exception("Size cannot be less than 1");

        Instance.TextureCopyCShader.SetTexture(Instance.TextureCopy_Kernel, sp_Source, source);
        Instance.TextureCopyCShader.SetTexture(Instance.TextureCopy_Kernel, sp_Target, target);
        Instance.TextureCopyCShader.SetVector(sp_Offset, offset);

        Instance.TextureCopyCShader.Dispatch(Instance.TextureCopy_Kernel, Mathf.CeilToInt(size.x / 8), Mathf.CeilToInt(size.y / 8), 1);
    }
}

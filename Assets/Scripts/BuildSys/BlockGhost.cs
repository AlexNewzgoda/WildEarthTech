using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGhost : MonoBehaviour
{
    List<MeshRenderer> meshRenderers;
    public MeshRenderer SelfMeshRender;
    public MeshFilter meshFilter;


    public void SetGeometry(Mesh mesh)
    {
        meshFilter.mesh = mesh;
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());
    }
   
    public void SetMaterials(Material material)
    {
        SelfMeshRender.material = material;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockConfig : MonoBehaviour
{
    public GameObject ParentMountPoints;
    public GameObject Colliders;
    public MeshFilter MainGeometry;
    public BlockType ThisBlockType;
    public List<MountPoint> MountPoints;

    public Vector3 RotateStep;

    public void Awake()
    {
        ParentMountPoints.SetActive(true);

        foreach(MountPoint m in MountPoints)
        {
            m.CheckSide();
        }

        Colliders.SetActive(true);
    }

    public enum BlockType
    {
        Mechanism,
        Foundation,
        Wall,
        Roof
    }
}


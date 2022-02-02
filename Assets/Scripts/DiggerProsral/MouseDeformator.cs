using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDeformator : MonoBehaviour
{
    public float Radius = 10;
    public float Height = -1f;
    public AnimationCurve DeformProfile;
    public int PaintInLayer = 1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rhit;
            if (Physics.Raycast(ray, out rhit, 1000))
            {
                DeformableTerrain deformableTerrain;
                if(rhit.collider.TryGetComponent(out deformableTerrain))
                    deformableTerrain.ApplyDeformProfile(rhit.textureCoord, Radius, Height, DeformProfile, PaintInLayer);
            }
        }
    }
}

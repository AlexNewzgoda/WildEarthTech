using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayGroundDeformer : MonoBehaviour
{
    public float Diametr = 1;
    public float Power = 1;

    public float DEPTH = 0;

    private Vector3 _prePosition;

    private void Start()
    {
        
    }

    private void LateUpdate()
    {
        if (this.transform.position != _prePosition)
        {
            Ray ray = new Ray(this.transform.position, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 5f, 1 << 6))
            {
                DeformableGround ground;
                if (hit.collider.TryGetComponent(out ground))
                {
                    float distance = Mathf.Max(hit.distance - Diametr * 0.5f, 0);
                    if (distance < ground.MaxHeight)
                    {
                        float depth = Mathf.InverseLerp(ground.MaxHeight, 0, distance);

                        ground.ApplyDeformSphere(hit.textureCoord, Diametr * 0.5f, Power, depth);
                    }
                }
            }

            _prePosition = this.transform.position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        Gizmos.DrawSphere(this.transform.position, Diametr * 0.5f);
    }
}

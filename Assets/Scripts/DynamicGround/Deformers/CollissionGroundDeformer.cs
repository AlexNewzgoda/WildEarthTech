using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CollissionGroundDeformer : MonoBehaviour
{
    private void Start()
    {
        
    }

    public DeformableGround TestTarget;
    public Renderer GetRenderer;

    public Transform Pos1;
    public Transform Pos2;
    public Transform Pos3;
    public Transform Pos4;

    public Transform Point;

    public Transform[] DeformationPoints;
    //public Transform[] DownPoints = new Transform[4];

    public class ReverseComparer<T> : IComparer where T : Transform
    {
        public int Compare(object x, object y)
        {
            //throw new System.NotImplementedException();
            Transform t1 = (Transform)x;
            Transform t2 = (Transform)y;

            return t1.position.y < t2.position.y ? 1 : -1;
        }
    }
    private ReverseComparer<Transform> SortFunc = new ReverseComparer<Transform>();

    private void Update()
    {
        /*Vector2 pos1 = WorldToUVCoord(Pos1.position, TestTarget.transform);
        Vector2 pos2 = WorldToUVCoord(Pos2.position, TestTarget.transform);
        Vector2 pos3 = WorldToUVCoord(Pos3.position, TestTarget.transform);
        Vector2 pos4 = WorldToUVCoord(Pos4.position, TestTarget.transform);*/

        /*Bounds bounds = GetRenderer.bounds;

        Vector2 pos1 = TestTarget.WorldToUVCoord(this.transform.position + new Vector3(-bounds.extents.x, 0, bounds.extents.z));
        Vector2 pos2 = TestTarget.WorldToUVCoord(this.transform.position + new Vector3(bounds.extents.x, 0, bounds.extents.z));
        Vector2 pos3 = TestTarget.WorldToUVCoord(this.transform.position + new Vector3(bounds.extents.x, 0, -bounds.extents.z));
        Vector2 pos4 = TestTarget.WorldToUVCoord(this.transform.position + new Vector3(-bounds.extents.x, 0, -bounds.extents.z));*/

        //System.Array.Sort(DeformationPoints, SortFunc);

        Vector2 pos1 = TestTarget.WorldToUVCoord(DeformationPoints[0].position);
        Vector2 pos2 = TestTarget.WorldToUVCoord(DeformationPoints[1].position);
        Vector2 pos3 = TestTarget.WorldToUVCoord(DeformationPoints[2].position);
        Vector2 pos4 = TestTarget.WorldToUVCoord(DeformationPoints[3].position);

        TestTarget.ApplyDeformRectangle_NotOptimized(
            pos1,
            pos2,
            pos3,
            pos4,
            1f);
    }

    private Vector2 LocalToUVCoord(Vector3 localPos)
    {
        localPos.y = localPos.z;
        localPos.z = 0;

        float size = 10f;

        localPos = localPos / size + new Vector3(0.5f, 0.5f, 0);

        localPos.x = 1 - localPos.x;
        localPos.y = 1 - localPos.y;

        return localPos;
    }

    bool pointInRectangle(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 m)
    {
        Vector2 AB = vect2d(A, B); float C1 = -1 * (AB.y * A.x + AB.x * A.y); float D1 = (AB.y * m.x + AB.x * m.y) + C1;
        Vector2 AD = vect2d(A, D); float C2 = -1 * (AD.y * A.x + AD.x * A.y); float D2 = (AD.y * m.x + AD.x * m.y) + C2;
        Vector2 BC = vect2d(B, C); float C3 = -1 * (BC.y * B.x + BC.x * B.y); float D3 = (BC.y * m.x + BC.x * m.y) + C3;
        Vector2 CD = vect2d(C, D); float C4 = -1 * (CD.y * C.x + CD.x * C.y); float D4 = (CD.y * m.x + CD.x * m.y) + C4;
        return 0 >= D1 && 0 >= D4 && 0 <= D2 && 0 >= D3;
    }

    Vector2 vect2d(Vector2 p1, Vector2 p2)
    {
        Vector2 temp;
        temp.x = (p2.x - p1.x);
        temp.y = -1 * (p2.y - p1.y);
        return temp;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnDrawGizmos()
    {
        if (GetRenderer == null)
            return;

        //Gizmos.matrix = this.transform.localToWorldMatrix;

        Bounds bounds = GetRenderer.bounds;

        Vector2 pos1 = new Vector3(-bounds.extents.x, 0, bounds.extents.z);
        Vector2 pos2 = new Vector3(bounds.extents.x, 0, bounds.extents.z);
        Vector2 pos3 = new Vector3(bounds.extents.x, 0, -bounds.extents.z);
        Vector2 pos4 = new Vector3(-bounds.extents.x, 0, -bounds.extents.z);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos1, 0.2f);
        Gizmos.DrawWireSphere(pos2, 0.2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(pos3, 0.2f);
        Gizmos.DrawSphere(pos4, 0.2f);
    }
}

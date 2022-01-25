using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace krkr
{
    public class BezieTools : MonoBehaviour
    {
        public static Vector3[] Bezie(Vector3 p0, Vector3 p1, Vector3 p2, int pointscount)
        {
            pointscount = pointscount < 3 ? 3 : pointscount;
            Vector3[] points = new Vector3[pointscount];

            for (int q = 0; q < pointscount; q++)
            {
                float t = (float)q / (pointscount - 1);
                points[q] = (1f - t) * (1f - t) * p0 + 2f * t * (1f - t) * p1 + t * t * p2;
            }
            return points;
        }
    }
}

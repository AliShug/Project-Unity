using UnityEngine;
using System.Collections;

namespace MeshExtensions
{
    public static class MeshExtensionsClass
    {
        public static Vector3 ClosestPointOnTriangle(Vector3 t0, Vector3 t1, Vector3 t2, Vector3 point)
        {
            Vector3 edge0, edge1, v0;
            float a, b, c, d, e, det, s, t;

            edge0 = t1 - t0;
            edge1 = t2 - t0;
            v0 = t0 - point;

            a = Vector3.Dot(edge0, edge0);
            b = Vector3.Dot(edge0, edge1);
            c = Vector3.Dot(edge1, edge1);
            d = Vector3.Dot(edge0, v0);
            e = Vector3.Dot(edge1, v0);

            det = a*c - b*b;
            s = b*e - c*d;
            t = b*d - a*e;

            if (s + t < det)
            {
                if (s < 0.0f)
                {
                    if (t < 0.0f)
                    {
                        if (d < 0.0f)
                        {
                            s = Mathf.Clamp01(-d/a);
                            t = 0.0f;
                        }
                        else
                        {
                            s = 0.0f;
                            t = Mathf.Clamp01(-e/c);
                        }
                    }
                    else
                    {
                        s = 0.0f;
                        t = Mathf.Clamp01(-e/c);
                    }
                }
                else if (t < 0.0f)
                {
                    s = Mathf.Clamp01(-d/a);
                    t = 0.0f;
                }
                else
                {
                    float invDet = 1.0f/det;
                    s *= invDet;
                    t *= invDet;
                }
            }
            else
            {
                if (s < 0.0f)
                {
                    float tmp0 = b + d;
                    float tmp1 = c + e;
                    if (tmp1 > tmp0)
                    {
                        float numer = tmp1 - tmp0;
                        float denom = a - 2*b + c;
                        s = Mathf.Clamp01(numer/denom);
                        t = 1.0f - s;
                    }
                    else
                    {
                        t = Mathf.Clamp01(-e/c);
                        s = 0.0f;
                    }
                }
                else if (t < 0.0f)
                {
                    if (a + d > b + e)
                    {
                        float numer = c + e - b - d;
                        float denom = a - 2*b + c;
                        s = Mathf.Clamp01(numer/denom);
                        t = 1.0f - s;
                    }
                    else
                    {
                        s = Mathf.Clamp01(-e/c);
                        t = 0.0f;
                    }
                }
                else
                {
                    float numer = c + e - b - d;
                    float denom = a - 2*b + c;
                    s = Mathf.Clamp01(numer/denom);
                    t = 1.0f - s;
                }
            }

            return t0 + s*edge0 + t*edge1;
        }

        /**
		 * Finds the nearest point on the mesh surface to the given input point
		 */
        public static Vector3 NearestPoint(this Mesh mesh, Vector3 point, out Vector3 normal)
        {
            Vector3 closestPoint = new Vector3();
            float closestSqDist = Mathf.Infinity;

            Vector3[] n = new Vector3[3];
            Vector3[] t = new Vector3[3];

            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                for (int j = 0; j < 3; j++)
                    t[j] = mesh.vertices[mesh.triangles[i + j]];

                Vector3 checkPoint = ClosestPointOnTriangle(t[0], t[1], t[2], point);
                Vector3 delta = point - checkPoint;
                float sqDist = delta.sqrMagnitude;

                if (sqDist < closestSqDist)
                {
                    closestSqDist = sqDist;
                    closestPoint = checkPoint;

                    for (int j = 0; j < 3; j++)
                        n[j] = mesh.normals[mesh.triangles[i + j]];
                }
            }

            // Normalised average of triangle normals
            normal = (n[0] + n[1] + n[2])/3;
            normal.Normalize();
            return closestPoint;
        }
    }
}

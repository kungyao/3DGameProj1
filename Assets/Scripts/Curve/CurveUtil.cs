using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CurveUtil
{
    public static List<float> arcLength(List<Transform> cps, int detail, float weight)
    {
        List<float> arcList = new List<float>(0);
        for (int i = 0; i < cps.Count - 3; i++)
        {
            bool first = true;
            Vector3 pre = Vector3.zero;
            float tmpdis = 0;
            for(int j = 0; j < detail; j++)
            {
                float t = (float)j / detail;
                if (first)
                {
                    pre = Sample(cps[i].position, cps[i + 1].position, cps[i + 2].position, cps[i + 3].position, weight, t);
                    first = false;
                }
                else
                {
                    Vector3 now = Sample(cps[i].position, cps[i + 1].position, cps[i + 2].position, cps[i + 3].position, weight, t);
                    tmpdis += (now - pre).magnitude;
                    pre = now;
                }
            }
            arcList.Add(tmpdis);
        }
        return arcList;
    }

    public static Vector3 Sample(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float weight, float t)
    {
        Vector3 t3 = (p0 * -weight) + (p1 * (2 - weight)) + (p2 * (-2 + weight)) + (p3 * weight);
        Vector3 t2 = (p0 * (2 * weight)) + (p1 * (-3 + weight)) + (p2 * (3 - 2 * weight)) + (p3 * -weight);
        Vector3 t1 = (p0 * -weight) + (p2 * weight);
        Vector3 t0 = p1;
        return t * t * t * t3 + t * t * t2 + t * t1 + t0;
    }
    public static Vector3 Tangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float weight, float t)
    {
        Vector3 t3 = (p0 * -weight) + (p1 * (2 - weight)) + (p2 * (-2 + weight)) + (p3 * weight);
        Vector3 t2 = (p0 * (2 * weight)) + (p1 * (-3 + weight)) + (p2 * (3 - 2 * weight)) + (p3 * -weight);
        Vector3 t1 = (p0 * -weight) + (p2 * weight);
        Vector3 n = 3 * t * t * t3 + 2 * t * t2 + t1;
        return n.normalized;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CurveUtil
{
    public static Vector3 LineSample(Vector3 p0, Vector3 p1, float t)
    {
        return (1 - t) * p0 + t * p1;
    }
    public static Vector3 QBezSample(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (1 - t) * (1 - t) * p0 + 
            t * (1 - t) * p1 + 
            t * t * p2;
    }
    public static Vector3 CBezSample(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return (1 - t) * (1 - t) * (1 - t) * p0 +
            t * (1 - t) * (1 - t) * p1 +
            t * t * (1 - t) * p2 +
            t * t * t * p3;
    }

    public static Vector3 LineTangent(Vector3 p0, Vector3 p1)
    {
        return (p1 - p0).normalized;
    }
    public static Vector3 QBezTangent(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 dp0 = 2 * (p1 - p0);
        Vector3 dp1 = 2 * (p2 - p1);
        return LineSample(dp0, dp1, t).normalized;
    }
    public static Vector3 CBezTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 dp0 = 3 * (p1 - p0);
        Vector3 dp1 = 3 * (p2 - p1);
        Vector3 dp2 = 3 * (p3 - p2);
        return QBezSample(dp0, dp1, dp2, t).normalized;
    }

    public static Vector3 Sample(Segment pre, Segment next, float t)
    {
        if (pre.ifUseFull1 && pre.ifUseFull0) return CBezSample(pre.Point, pre.Handle1, next.Handle0, next.Point, t);
        else if (!pre.ifUseFull1 && !pre.ifUseFull0) return LineSample(pre.Point, next.Point, t);
        else if (pre.ifUseFull1 && !pre.ifUseFull0) return QBezSample(pre.Point, pre.Handle1, next.Point, t);
        else if (!pre.ifUseFull1 && pre.ifUseFull0) return QBezSample(pre.Point, next.Handle0, next.Point, t);
        return Vector3.zero;
    }
    public static Vector3 Tangent(Segment pre, Segment next, float t)
    {
        if (pre.ifUseFull1 && pre.ifUseFull0) return CBezTangent(pre.Point, pre.Handle1, next.Handle0, next.Point, t);
        else if (!pre.ifUseFull1 && !pre.ifUseFull0) return LineTangent(pre.Point, next.Point);
        else if (pre.ifUseFull1 && !pre.ifUseFull0) return QBezTangent(pre.Point, pre.Handle1, next.Point, t);
        else if (!pre.ifUseFull1 && pre.ifUseFull0) return QBezTangent(pre.Point, next.Handle0, next.Point, t);
        return Vector3.zero;
    }
}

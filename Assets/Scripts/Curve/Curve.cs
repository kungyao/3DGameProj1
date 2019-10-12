using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment
{
    public bool isDirty;
    public bool ifUseFull0;
    public bool ifUseFull1;
    public Vector3 Handle0;
    public Vector3 Point;
    public Vector3 Handle1;
    public Segment(Vector3 pt)
    {
        isDirty = false;
        ifUseFull0 = false;
        ifUseFull1 = false;
        Handle0 = Vector3.zero;
        Point = pt;
        Handle1 = Vector3.zero;
    }
}

public class BezCurve
{
    public int detail = 20;
    public List<float> arcLength = new List<float>();
    public List<Segment> segments = new List<Segment>();
    public Vector3 getSampleByT(float t)
    {
        if (segments.Count == 0) return Vector3.zero;
        int index = (int)t;
        float segt = t - index;
        if (index < 0)
        {
            return segments[0].Point;
        }
        else if (index >= segments.Count)
        {
            return segments[segments.Count - 1].Point;
        }
        else
        {
            return CurveUtil.Sample(segments[index], segments[index + 1], segt);
        }
    }
    public Vector3 getSampleByDis(float dis)
    {
        if (segments.Count == 0) return Vector3.zero;
        float segt = -1;
        int index = -1;
        for (int i = 0; i < arcLength.Count; i++) 
        {
            float length = arcLength[i];
            if (dis < length)
            {
                index = i;
                segt = dis / length;
            }
            else
            {
                dis -= length;
            }
        }
        if (index == -1) return segments[segments.Count - 1].Point;
        else
        {
            return CurveUtil.Tangent(segments[index], segments[index + 1], segt);
        }
    }
}

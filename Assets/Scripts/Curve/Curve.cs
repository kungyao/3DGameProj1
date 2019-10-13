using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour
{
    public GameObject pre;

    public bool dirty = false;

    public float arcDis = 1;

    public float disNow = 0;
    public int indexNow = 0;
    public float tNow = 0;

    public float weight = 0.5f;
    public int detail = 30;
    public List<float> arcLength = new List<float>();
    public List<Transform> cps = new List<Transform>();

    public void InitializeDis()
    {
        indexNow = -1;
        disNow = 0;
        tNow = -1;
    }

    public void Initialize(GameObject prefab)
    {
        indexNow = -1;
        disNow = 0;
        tNow = -1;
        dirty = true;
        pre = prefab;
        cps.Clear();
        arcLength.Clear();
        AddPoint(new Vector3(-352, 0, 0));
        AddPoint(new Vector3(-128, 0, -235));
        AddPoint(new Vector3(263, 0, 0));
        AddPoint(new Vector3(518, 0, -282));
        UpdateArc();
    }

    public void EnableCps()
    {
        foreach (Transform cp in cps)
        {
            cp.gameObject.SetActive(true);
        }
    }

    public void DisAbleCps()
    {
        foreach (Transform cp in cps)
        {
            cp.gameObject.SetActive(false);
        }
    }

    public Transform AddPoint(Vector3 offset)
    {
        dirty = true;
        indexNow = 0;
        tNow = 0;
        disNow = 0;
        GameObject tmp = GameObject.Instantiate(pre);
        tmp.transform.position = tmp.transform.position + offset;
        cps.Add(tmp.transform);
        return tmp.transform;
    }

    public void RemovePoint(Transform index)
    {
        dirty = true;
        indexNow = 0;
        tNow = 0;
        disNow = 0;
        cps.Remove(index);
    }

    public void UpdateArc()
    {
        if (!dirty) return;
        arcLength = CurveUtil.arcLength(cps, detail, weight);
        dirty = false;
    }

    public void CalcT()
    {
        UpdateArc();
        if (cps.Count < 4)
        {
            InitializeDis();
            return;
        }
        float tmpDis = disNow;
        float segt = -1;
        int index = -1;
        for (int i = 0; i < arcLength.Count; i++)
        {
            float length = arcLength[i];
            if (tmpDis < length)
            {
                index = i;
                segt = tmpDis / length;
                tmpDis = -1;
                break;
            }
            else
            {
                tmpDis -= length;
            }
        }
        if (tmpDis > 0)
        {
            index = 0;
            segt = 0;
            disNow = 0;
        }
        indexNow = index;
        tNow = segt;
        disNow += arcDis;
    }

    public Vector3 getSampleByT(float t)
    {
        if (cps.Count < 4) return Vector3.zero;
        int index = (int)t;
        float segt = t - index;
        if (index < 0)
        {
            return cps[0].position;
        }
        else if (index >= cps.Count - 3)
        {
            return cps[cps.Count - 1].position;
        }
        else
        {
            return CurveUtil.Sample(cps[index].position, cps[index + 1].position, cps[index + 2].position, cps[index + 3].position, weight, segt);
        }
    }

    public Vector3 getSampleByDis()
    {
        if (indexNow < 0)
        {
            return Vector3.zero;
        }
        return CurveUtil.Sample(cps[indexNow].position, cps[indexNow + 1].position, cps[indexNow + 2].position, cps[indexNow + 3].position, weight, tNow);
    }

    public Vector3 getTangentByDis()
    {
        if (indexNow < 0)
        {
            return Vector3.forward;
        }
        return CurveUtil.Tangent(cps[indexNow].position, cps[indexNow + 1].position, cps[indexNow + 2].position, cps[indexNow + 3].position, weight, tNow);
    }

    public void DrawLine()
    {
        LineRenderer tmpRen = GameManager.One.lineRen;
        int segCount = cps.Count - 3;
        if (segCount <= 0) return;
        Vector3 pre = Vector3.zero;
        int _detail = detail;
        tmpRen.positionCount = _detail * segCount + 1;
        for (int i = 0; i < segCount; i++)
        {
            for (int j = 0; j < _detail; j++)
            {
                float t = (float)j / _detail;
                Vector3 now = CurveUtil.Sample(cps[i].position, cps[i + 1].position, cps[i + 2].position, cps[i + 3].position, weight, t);
                tmpRen.SetPosition(i * _detail + j, now);
            }
            if (i == segCount - 1)
            {
                tmpRen.SetPosition(i * _detail + _detail, cps[i + 2].position);
            }
        }
    }

    public void DrawGizmos(int frames)
    {
        int segCount = cps.Count - 3;
        if (segCount <= 0) return;
        Vector3 pre = Vector3.zero;
        int _detail = frames / segCount;
        bool first = true;
        for (int i = 0; i < segCount; i++) 
        {
            if (i == segCount - 1)
            {
                _detail = _detail + (frames - _detail * segCount);
            }
            for (int j = 0; j < _detail; j++) 
            {
                float t = (float)j / _detail;
                if (first)
                {
                    pre = CurveUtil.Sample(cps[i].position, cps[i + 1].position, cps[i + 2].position, cps[i + 3].position, weight, t);
                    first = false;
                }
                else
                {
                    Vector3 now = CurveUtil.Sample(cps[i].position, cps[i + 1].position, cps[i + 2].position, cps[i + 3].position, weight, t);
                    Gizmos.DrawLine(pre, now);
                    pre = now;
                }
            }
        }
    }
}

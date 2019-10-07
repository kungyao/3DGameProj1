//Parser

using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Text;
using Roots = System.Collections.Generic.List<Joint>;
using BvhDatas = System.Collections.Generic.List<BvhData>;

public enum BVH{
    NONE = 0,
    HIERARCHY = 1,
    ROOT = 2,
    OFFSET = 3,
    CHANNELS = 4,
    JOINT = 5,
    End = 6,
    MOTION = 7,
    FRAMES = 8,
    FRAME_TIME = 9,
    BRACKET_ADD = 10,
    BRACKET_SUB = 11
}

public class BvhStep
{
    public BVH cur = BVH.NONE;
    public List<BVH> bvhNext = new List<BVH>();

    public bool Check(BVH now, string[] items)
    {
        foreach (BVH b in bvhNext)
        {
            if (b == now)
            {
                if (now == BVH.OFFSET && items.Length != 4)
                {
                    Debug.Log("now == BVH.OFFSET && items.Length != 4");
                    return false;
                }
                else if (now == BVH.CHANNELS)
                {
                    int cs = 0;
                    int.TryParse(items[1], out cs);
                    if (items.Length != cs + 2)
                    {
                        Debug.Log("now == BVH.CHANNELS && items.Length != cs + 2");
                        return false;
                    }
                }
                return true;
            }
        }
        Debug.Log("now not in bvhNext");
        return false;
    }

    public void AddNext(BVH now)
    {
        cur = now;
        bvhNext.Clear();
        if (now == BVH.HIERARCHY)
        {
            bvhNext.Add(BVH.ROOT);
            bvhNext.Add(BVH.MOTION);
        }
        else if (now == BVH.ROOT)
        {
            bvhNext.Add(BVH.OFFSET);
        }
        else if (now == BVH.OFFSET)
        {
            bvhNext.Add(BVH.CHANNELS);
        }
        else if (now == BVH.CHANNELS)
        {
            bvhNext.Add(BVH.JOINT);
            bvhNext.Add(BVH.End);
        }
        else if (now == BVH.JOINT)
        {
            bvhNext.Add(BVH.JOINT);
            bvhNext.Add(BVH.OFFSET);
        }
        else if (now == BVH.End)
        {
            bvhNext.Add(BVH.OFFSET);
        }
        else if (now == BVH.MOTION)
        {
            bvhNext.Add(BVH.FRAMES);
        }
        else if (now == BVH.FRAMES)
        {
            bvhNext.Add(BVH.FRAME_TIME);
        }
        else if (now == BVH.FRAME_TIME)
        {
            bvhNext.Add(BVH.MOTION);
        }
    }
}

public class BvhSteps
{
    public int top = -1;
    public List<BvhStep> steps = new List<BvhStep>();
    public BVH Check(string[] items)
    {
        //Debug.Log(top);
        if (top == -1)
        {
            if (items.Length == 1)
            {
                if (items[0] == BVH.HIERARCHY.ToString())
                {
                    steps.Add(new BvhStep());
                    steps[++top].AddNext(BVH.HIERARCHY);
                }
                else
                {
                    return BVH.NONE;
                }
            }
            else
            {
                return BVH.NONE;
            }
        }
        else
        {
            if (items[0] == "{")
                return BVH.BRACKET_ADD;
            else if (items[0] == "}")
            {
                steps.RemoveAt(steps.Count - 1);
                top--;
                return BVH.BRACKET_SUB;
            }
            BVH now = BVH.NONE;
            if (items[0] == "OFFSET") now = BVH.OFFSET;
            else if (items[0] == "CHANNELS") now = BVH.CHANNELS;
            else if (items[0] == "JOINT") now = BVH.JOINT;
            else if (items[0] == "End") now = BVH.End;
            else if (items[0] == "ROOT") now = BVH.ROOT;
            else if (items[0] == "MOTION") now = BVH.MOTION;
            else if (items[0] == "Frames:") now = BVH.FRAMES;
            else if (items[0] == "Frame") now = BVH.FRAME_TIME;
            //foreach (string str in items)
            //    Debug.Log(str);
            if (!steps[top].Check(now, items))
            {
                return BVH.NONE;
            }
            else
            {
                if (now == BVH.JOINT || now == BVH.ROOT || now == BVH.End)
                {
                    steps.Add(new BvhStep());
                    top++;
                }
                steps[top].AddNext(now);
            }
        }
        return steps[top].cur;
    }
}

public class BvhUtility
{
    static private char[] seperator = { '\t', ' ' };
    static private void setJointData(Joint jt, int rTop, BVH now, string[] items)
    {
        int childCount = jt.jointChilds.Count;
        if (childCount == 0)
        {
            if (now == BVH.OFFSET)
            {
                jt.offset = new Vector3(
                    float.Parse(items[1]),
                    float.Parse(items[2]),
                    float.Parse(items[3])
                    );
            }
            else if (now == BVH.CHANNELS)
            {

                for (int i = 2; i < items.Length; i++)
                {
                    jt.channels.Add(items[i]);
                }
            }
        }
        else
        {
            setJointData(jt.jointChilds[childCount - 1], childCount - 1, now, items);
        }
    }

    static private void addJoint(Joint jt, int rTop, int top, BVH now, string[] items)
    {
        int l = jt.jointChilds.Count;
        if (top == 0)
        {
            jt.jointChilds.Add(new Joint());
            jt.jointChilds[l].name = items[1];
            jt.jointChilds[l].type = JType.JOINT;
            if (now == BVH.End)
                jt.jointChilds[l].type = JType.ENDSITE;
        }
        else
        {
            addJoint(jt.jointChilds[l - 1], l - 1, top - 1, now, items);
        }
    }

    static private void readAniData(StreamReader fileContent, Motion motion)
    {
        string lineStr = "";
        while ((lineStr = fileContent.ReadLine()) != null)
        {
            string[] items = lineStr.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries);
            List<float> tmpFloat = new List<float>(items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                tmpFloat.Add(float.Parse(items[i]));
            }
            motion.aniData.Add(tmpFloat);
        }
    }

    static public void ParseBvhData(ref BvhDatas roots, string path)
    {
        BvhSteps c = new BvhSteps();
        StreamReader fileContent = new StreamReader(path);
        BVH now = BVH.NONE;
        string lineStr = "";
        int rTop = roots.Count - 1, top = -1;
        while ((lineStr = fileContent.ReadLine()) != null)
        {
            //Debug.Log(lineStr);
            string[] items = lineStr.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries);
            if ((now = c.Check(items)) != BVH.NONE) 
            {
                if (now == BVH.HIERARCHY)
                {
                    continue;
                }
                else if(now == BVH.ROOT)
                {
                    roots.Add(new BvhData());
                    rTop++;
                    roots[rTop].joint = new Joint();
                    roots[rTop].joint.name = items[1];
                    roots[rTop].joint.type = JType.ROOT;
                    continue;
                }
                else if (now == BVH.OFFSET)
                {
                    setJointData(roots[rTop].joint, rTop, now, items);
                    continue;
                }
                else if (now == BVH.CHANNELS)
                {
                    setJointData(roots[rTop].joint, rTop, now, items);
                    continue;
                }
                else if (now == BVH.JOINT)
                {
                    addJoint(roots[rTop].joint, rTop, top, now, items);
                    continue;
                }
                else if (now == BVH.End)
                {
                    items[1] = "End Site";
                    addJoint(roots[rTop].joint, rTop, top, now, items);
                    continue;
                }
                else if (now == BVH.BRACKET_ADD)
                {
                    top++;
                }
                else if (now == BVH.BRACKET_SUB)
                {
                    top--;
                }
                else if (now == BVH.MOTION)
                {
                    roots[rTop].motion = new Motion();
                }
                else if (now == BVH.FRAMES)
                {
                    roots[rTop].motion.frames = int.Parse(items[1]);
                    roots[rTop].motion.aniData = new List<List<float>>(roots[rTop].motion.frames);
                }
                else if (now == BVH.FRAME_TIME)
                {
                    roots[rTop].motion.frame_time = float.Parse(items[2]);
                    readAniData(fileContent, roots[rTop].motion);
                }
            }
            else
            {
                Debug.Log("Error");
                return;
            }
        }

        // print joint data
        //foreach (BvhData bd in roots)
        //{
        //    bd.joint.Print();
        //    bd.motion.Print();
        //}
    }
}
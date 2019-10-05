using System.Collections.Generic;
using UnityEngine;

public class Pair<T1, T2>
{
    public T1 First;
    public T2 Second;
}

public enum JType
{
    ROOT = 0,
    JOINT = 1,
    ENDSITE = 2
}

public class Joint
{
    public JType type = JType.ENDSITE;
    public string name = "";
    public Vector3 offset = new Vector3();
    public List<string> channels = new List<string>();
    public List<Joint> jointChilds = new List<Joint>();

    public void Print()
    {
        Debug.Log("Type: " + type.ToString());
        Debug.Log("name: " + name);
        Debug.Log("Offset: " + offset);
        if (channels.Count != 0)
        {
            string chanStr = "Channel:";
            foreach (string channel in channels)
            {
                chanStr = string.Concat(chanStr, " \\ " + channel);
            }
            Debug.Log(chanStr);
        }
        if (jointChilds.Count != 0)
        {
            Debug.Log("---jointChilds print start---");
            foreach (Joint jt in jointChilds)
            {
                jt.Print();
            }
            Debug.Log("---jointChilds print end---");
        }
    }
}

public class Motion
{
    public int frames = 0;
    public float frame_time = 0;
    public List<List<float>> aniData = new List<List<float>>();

    public void Print()
    {
        Debug.Log("Frames: " + frames);
        Debug.Log("Frame Time: " + frame_time);
        Debug.Log("---Animation Data---");
        foreach (List<float> row in aniData)
        {
            string ani = "";
            foreach (float column in row)
            {
                ani = string.Concat(ani, column.ToString() + " ");
            }
            Debug.Log(ani);
        }
    }
    //public Motion(int _frames, float _frame_time)
    //{
    //    frames = _frames;
    //    frame_time = _frame_time;
    //    aniData = new List<List<float>>(_frames);
    //}
}

public class BvhData
{
    public Joint joint;
    public Motion motion;
}
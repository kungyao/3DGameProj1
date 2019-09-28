using System.Collections.Generic;
using UnityEngine;

public class Pair<T1, T2>
{
    public T1 First {
        get
        {
            return First;
        }
        set
        {
            First = value;
        }
    }
    public T2 Second
    {
        get
        {
            return Second;
        }
        set
        {
            Second = value;
        }
    }
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
    public List<Pair<string, float>> channels = new List<Pair<string, float>>();
    public List<Joint> jointChilds = new List<Joint>();
}
//資料結構
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
    //存關節的GameObject
    public List<Transform> jointObject = new List<Transform>();

    public int interpolation = 1;

    public string fileName = "";

    //現在的動作(楨數)
    private int nowFrame = 0;

    //目前是什麼動作
    private int motionType = 0;

    //load 下一個動作(楨)
    public void JumpNextFrame()
    {
        //Root關節
        assignData(jointObject[0], this.joint, 0);
        nowFrame++;
        nowFrame %= this.motion.frames;
    }

    //內插至下一楨
    public void InterpolationNextFrame()
    {

    }

    //畫骨架
    public void draw()
    {

    }

    //根據關節recursive位移Gameobject 回傳MotionIndex
    private int assignData(Transform _object, Joint _joint, int _motionIndex)
    {

        Vector3 tmpVec = new Vector3();
        Vector3 tmpqua = new Vector3();
        for (int i = 0; i < _joint.channels.Count; i++)
        {
            if (_joint.channels[i] == "Xposition")
            {
                tmpVec.x = this.motion.aniData[this.nowFrame][_motionIndex++];
            }
            else if (_joint.channels[i] == "Yposition")
            {
                tmpVec.y = this.motion.aniData[this.nowFrame][_motionIndex++];
            }
            else if (_joint.channels[i] == "Zposition")
            {
                tmpVec.z = this.motion.aniData[this.nowFrame][_motionIndex++];
            }
            else if (_joint.channels[i] == "Xrotation")
            {
                tmpqua.x = this.motion.aniData[this.nowFrame][_motionIndex++];
            }
            else if (_joint.channels[i] == "Yrotation")
            {
                tmpqua.y = this.motion.aniData[this.nowFrame][_motionIndex++];
            }
            else if (_joint.channels[i] == "Zrotation")
            {
                tmpqua.z = this.motion.aniData[this.nowFrame][_motionIndex++];
            }
        }
        if (_joint.type == JType.ROOT)
            _object.localPosition = _joint.offset + tmpVec;
        else
            _object.localPosition = _joint.offset;
        _object.localRotation = Quaternion.Euler(tmpqua);

        //recursive 剩下的關節
        foreach (Joint jt in _joint.jointChilds)
        {
            int nextObjIndex = ((_motionIndex - 6) / 3) + 1;
            //end 直接return
            if (jt.type == JType.ENDSITE)
                continue;
            _motionIndex = assignData(this.jointObject[nextObjIndex], jt, _motionIndex);
        }
        return _motionIndex;
    }

}
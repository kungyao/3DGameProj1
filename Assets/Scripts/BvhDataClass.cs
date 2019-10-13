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

    public static Pair<int, int> diff(Motion m1, Motion m2)
    {
        Pair<int, int> tmp = new Pair<int, int>();
        float minPoint = 1000;
        for (int i = 0; i < m1.aniData.Count; i++)
        {
            for (int j = 0; j < m2.aniData.Count; j++)
            {
                float tmpPoint = 0;
                tmpPoint += Mathf.Abs(m1.aniData[i][1] - m2.aniData[j][1]);
                for (int k = 3; k < m2.aniData[j].Count; k++)
                {
                    tmpPoint += Mathf.Abs(m1.aniData[i][k] - m2.aniData[j][k]);
                }
                if (tmpPoint < minPoint)
                {
                    minPoint = tmpPoint;
                    tmp.First = i;
                    tmp.Second = j;
                }
            }
        }
        return tmp;
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
    public List<Motion> motion = new List<Motion>();

    //存關節的GameObject
    public List<Transform> jointObject = new List<Transform>();

    public List<Pair<Transform, Transform>> bonePair = new List<Pair<Transform, Transform>>();

    //要畫出來的骨頭方塊
    public List<Transform> boneObject = new List<Transform>();

    //內插進度 0~1 => 0~100
    public float interpolation = 0;

    //目前是什麼動作
    public int motionType = 0;

    //現在的動作(楨數)
    public int nowFrame = 0;

    //哪兩個motion切換
    public Pair<int, int> motionPair = new Pair<int, int>();

    //哪兩個frame切換
    public Pair<int, int> framePair = new Pair<int, int>();

    //parse 到motion的地幾個
    private int motionIndex = 0;

    //load 下一個動作(楨)
    public void JumpNextFrame()
    {
        //Root關節
        assignData(jointObject[0], this.joint, 0, false);
        nowFrame++;
        if (GameManager.One.isFollowLine)
            jointObject[0].localPosition = new Vector3(0, jointObject[0].localPosition.y, 0);
        nowFrame %= this.motion[motionType].frames;
    }

    //內插至下一楨
    public void InterpolationNextFrame()
    {
        //Root關節
        assignData(jointObject[0], this.joint, 0, true);
        interpolation += 0.1f;
        if (interpolation > 1)
        {
            interpolation = 0;
            nowFrame++;
        }
        nowFrame %= this.motion[motionType].frames;
    }

    //----------------------------------------兩motion切換
    public void TwoMotionFrame()
    {
        //Root關節
        assignData(jointObject[0], this.joint, 0, true);
        interpolation += 0.1f;
        if (interpolation > 1)
        {
            interpolation = 0;
            nowFrame++;
            if(motionType == motionPair.First)
            {
                if (nowFrame == framePair.First)
                {
                    nowFrame = framePair.Second;
                    motionType = motionPair.Second;
                }
                    
            }
            else
            {
                if (nowFrame == motion[motionPair.Second].frames-1)
                {
                    nowFrame = 0;
                    motionType = motionPair.First;
                }
            }
        }
    }

    //是否與此bvh相同 true : 相同
    public static bool equal(Joint _bvh, Joint _bvh2)
    {
        if (_bvh.jointChilds.Count != _bvh2.jointChilds.Count)
            return false;
        for (int i = 0; i < _bvh.jointChilds.Count; i++)
        {
            if (_bvh.name != _bvh2.name)
                return false;
            else
            {
                if (!equal(_bvh.jointChilds[i], _bvh2.jointChilds[i]))
                    return false;
            }
        }
        return true;
    }

    public void addMotion(BvhData _bvh)
    {
        motion.Add(_bvh.motion[0]);
    }

    //畫骨架
    public void draw()
    {
        int i = 0;
        foreach (Pair<Transform, Transform> kp in bonePair)
        {
            Vector3 between = kp.Second.position - kp.First.position;
            float distance = between.magnitude;
            boneObject[i].localPosition = new Vector3(distance, boneObject[i].localPosition.y, boneObject[i].localPosition.z);
            boneObject[i].position = kp.First.position + between / 2;
            boneObject[i].LookAt(kp.Second.position);
            boneObject[i].localScale = new Vector3(0.5f, 1, between.magnitude);
            i++;
        }
    }

    //根據關節recursive位移Gameobject 回傳MotionIndex
    private int assignData(Transform _object, Joint _joint, int _motionIndex, bool isLerp)
    {

        Vector3 tmpVec = new Vector3();
        Vector3 tmpqua = new Vector3();
        for (int i = 0; i < _joint.channels.Count; i++)
        {
            if (_joint.channels[i] == "Xposition")
            {
                tmpVec.x = this.motion[motionType].aniData[this.nowFrame][_motionIndex++];
            }
            else if (_joint.channels[i] == "Yposition")
            {
                tmpVec.y = this.motion[motionType].aniData[this.nowFrame][_motionIndex++];
            }
            else if (_joint.channels[i] == "Zposition")
            {
                tmpVec.z = this.motion[motionType].aniData[this.nowFrame][_motionIndex++];
            }
            else if (_joint.channels[i] == "Xrotation")
            {
                tmpqua.x = this.motion[motionType].aniData[this.nowFrame][_motionIndex++];
            }
            else if (_joint.channels[i] == "Yrotation")
            {
                tmpqua.y = this.motion[motionType].aniData[this.nowFrame][_motionIndex++];
            }
            else if (_joint.channels[i] == "Zrotation")
            {
                tmpqua.z = this.motion[motionType].aniData[this.nowFrame][_motionIndex++];
            }
        }
        //調整root的pos & rot
        if (_joint.type == JType.ROOT)
        {
            if (GameManager.One.isFollowLine)
            {
                tmpVec.x = 0;
                tmpVec.z = 0;
            }
            if (isLerp)
            {
                _object.localPosition = Vector3.Lerp(_object.localPosition, _joint.offset + tmpVec, interpolation);
            }
            else
            {
                _object.localPosition = _joint.offset + tmpVec;
            }
        }
            
        else
            _object.localPosition = _joint.offset;
        if (isLerp)
        {
            _object.localRotation = Quaternion.Slerp(_object.localRotation, Quaternion.Euler(tmpqua), interpolation);
        }
        else
        {
            _object.localRotation = Quaternion.Euler(tmpqua);
        }

        //recursive 剩下的關節
        foreach (Joint jt in _joint.jointChilds)
        {
            int nextObjIndex = ((_motionIndex - 6) / 3) + 1;
            //end 直接return
            if (jt.type == JType.ENDSITE)
                continue;
            _motionIndex = assignData(this.jointObject[nextObjIndex], jt, _motionIndex, isLerp);
        }
        return _motionIndex;
    }

}
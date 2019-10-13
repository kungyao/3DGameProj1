using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BvhDatas = System.Collections.Generic.List<BvhData>;

/*
 *
2.路徑
3. camera UI that includes tracking
4.Advanced path editing (arc-length parameterizations)
---------------------------------------
2.內插      
5.連接動作  播完改index
 * */
public class GameManager : MonoBehaviour
{
    public static GameManager One;
    public BvhDatas datas = new BvhDatas();
    //管理場讓所有人物的
    public List<GameObject> objects = new List<GameObject>();
    //目前哪個人物正在運作
    public int dataIndex = 0;
    //false = stop  true = paly
    public bool stop_And_Play = false;
    //是否使用內插
    public bool isInterpolation = false;
    // Start is called before the first frame update

    private void Awake()
    {
        One = this;
    }

    void Start()
    {
        LoadFile(Application.dataPath + "/Resources/jumpkick.bvh");
    }

    // Update is called once per frame
    void Update()
    {
        //根據dataIndex切人物 撥放人物動畫-------------------
        if(stop_And_Play)
        {
            //人物paly--------------------------------
            if(isInterpolation)
            {
                //內插---------------------
            }
            else
            {
                //不要內插----------------
                datas[dataIndex].JumpNextFrame(new Vector3(), new Vector3());
            }
        }
        //draw
        datas[dataIndex].draw();
    }

    private GameObject CreateCubeObjSub(Joint jtParent, Vector3 offset, GameObject parent)
    {
        GameObject jtp = new GameObject(jtParent.name);
        datas[datas.Count - 1].jointObject.Add(jtp.transform);
        jtp.transform.position = jtParent.offset + offset;
        jtp.transform.parent = parent.transform;
        foreach (Joint jt in jtParent.jointChilds)
        {
            if (jt.type == JType.ENDSITE)
                continue;
            GameObject tmpChild = CreateCubeObjSub(jt, jtp.transform.position, jtp);
            Pair<Transform, Transform> tmp = new Pair<Transform, Transform>();
            tmp.First = jtp.transform;
            tmp.Second = tmpChild.transform;
            datas[datas.Count - 1].bonePair.Add(tmp);
            GameObject tmpBone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tmpBone.transform.parent = objects[objects.Count - 1].transform;
            datas[datas.Count - 1].boneObject.Add(tmpBone.transform);
        }
        return jtp;
    }

    public void CreateCubeObj()
    {
        if (objects.Count != 0)
            objects[dataIndex].active = false;
        GameObject tmp = new GameObject("tmp" + datas.Count);
        objects.Add(tmp);
        dataIndex = datas.Count - 1;
        tmp.transform.position = Vector3.zero;
        CreateCubeObjSub(datas[datas.Count - 1].joint, tmp.transform.position, tmp);
    }

    //error return 1
    public bool LoadFile(string path)
    {
        int error = BvhUtility.ParseBvhData(ref datas, path);
        if (error == 1)
        {
            datas.RemoveAt(datas.Count - 1);
            return true;
        }
        CreateCubeObj();
        return false;
    }
}

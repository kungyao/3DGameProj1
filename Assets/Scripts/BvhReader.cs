//Ini
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BvhDatas = System.Collections.Generic.List<BvhData>;

public class BvhReader : MonoBehaviour
{
    public BvhDatas datas = new BvhDatas();
    public List<Pair<Transform, Transform>> tmpPair = new List<Pair<Transform, Transform>>();
    // Start is called before the first frame update
    void Start()
    {
        print(Application.dataPath + "/Resources/actorNoMotion.bvh");
        LoadFile(Application.dataPath + "/Resources/actorNoMotion.bvh");
    }

    private void Update()
    {
        datas[0].JumpNextFrame();
    }

    private void OnDrawGizmos()
    {
        if (datas.Count > 0)
        {
            foreach (Pair<Transform, Transform> kp in tmpPair)
            {
                Gizmos.DrawLine(kp.First.position, kp.Second.position);
            }
        }
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
            tmpPair.Add(tmp);
        }
        return jtp;
    }

    public void CreateCubeObj()
    {
        GameObject tmp = new GameObject("tmp" + datas.Count);
        tmp.transform.position = Vector3.zero;
        CreateCubeObjSub(datas[datas.Count - 1].joint, tmp.transform.position, tmp);
    }

    public void LoadFile(string path)
    {
        //roots.Clear();
        BvhUtility.ParseBvhData(ref datas, path);
        CreateCubeObj();
        //string fileContent = File.ReadAllText(path);
    }
}

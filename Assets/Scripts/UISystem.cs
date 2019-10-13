﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    public Text FileName;
    public Text Log;
    public Text PlayStop;

    //讀多個檔案
    //加入人物(Create obj)
    public void LoadFileCN()
    {
        print(FileName.text);
        bool error = GameManager.One.LoadFile(Application.dataPath + "/Resources/" + FileName.text + ".bvh");
        if (error)
            Log.text = "error";
    }

    public void LoadFileAM()
    {
        List<BvhData> tmp = new List<BvhData>();
        int error = BvhUtility.ParseBvhData(ref tmp, Application.dataPath + "/Resources/" + FileName.text + ".bvh");
        if (error == 1)
            Log.text = "error";
        else if (BvhData.equal(GameManager.One.datas[GameManager.One.dataIndex].joint, tmp[0].joint))
        {
            GameManager.One.datas[GameManager.One.dataIndex].addMotion(tmp[0]);
        }
        else
            Log.text = "error";
    }

    //切換骨架的button
    void OnGUI()
    {
        //切換該人物的動畫
        for (int i = 0; i < GameManager.One.datas[GameManager.One.dataIndex].motion.Count; i++)
        {
            if (GUI.Button(new Rect(500, i * 30, 100, 30), i.ToString()))
            {
                GameManager.One.datas[GameManager.One.dataIndex].motionType = i;
                GameManager.One.datas[GameManager.One.dataIndex].nowFrame = 0;
            }
        }
        //切換人物
        for (int i = 0; i < GameManager.One.datas.Count; i++)
        {
            if (GUI.Button(new Rect(0, i * 30, 100, 30), i.ToString()))
            {
                GameManager.One.dataIndex = i;
                for (int j = 0; j < GameManager.One.datas.Count; j++)
                {
                    if (j == GameManager.One.dataIndex)
                        GameManager.One.objects[j].active = true;
                    else
                        GameManager.One.objects[j].active = false;
                }
            }
        }

    }

    //控制播放
    public void sp()
    {
        GameManager.One.stop_And_Play = !GameManager.One.stop_And_Play;
        if (GameManager.One.stop_And_Play)
            PlayStop.text = "Stop";
        else
            PlayStop.text = "Play";
    }

    //切換內插
    public void Interpolation()
    {
        GameManager.One.isInterpolation = !GameManager.One.isInterpolation;
    }

}
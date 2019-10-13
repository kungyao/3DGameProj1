using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Windows.Forms;

public class UISystem : MonoBehaviour
{
    public Text FileName;
    public Text Log;
    public Text PlayStop;
    public Text twoMotion;
    public Text DisText;

    public GameObject addptBtn;
    public GameObject rmptBtn;

    //讀多個檔案
    //加入人物(Create obj)
    //public void LoadFileCN()
    //{
    //    print(FileName.text);
    //    bool error = GameManager.One.LoadFile(Application.dataPath + "/Resources/" + FileName.text + ".bvh");
    //    if (error)
    //        Log.text = "error";
    //}
    public void LoadFileCN()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            print(FileName.text);
            bool error = GameManager.One.LoadFile(dialog.FileName);
            if (error)
                Log.text = "error";
        }
    }

    //public void LoadFileAM()
    //{
    //    List<BvhData> tmp = new List<BvhData>();
    //    int error = BvhUtility.ParseBvhData(ref tmp, Application.dataPath + "/Resources/" + FileName.text + ".bvh");
    //    if (error == 1)
    //        Log.text = "error";
    //    else if (BvhData.equal(GameManager.One.datas[GameManager.One.dataIndex].joint, tmp[0].joint))
    //    {
    //        GameManager.One.datas[GameManager.One.dataIndex].addMotion(tmp[0]);
    //    }
    //    else
    //        Log.text = "error";
    //}
    public void LoadFileAM()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            List<BvhData> tmp = new List<BvhData>();
            int error = BvhUtility.ParseBvhData(ref tmp, dialog.FileName);
            if (error == 1)
                Log.text = "error";
            else if (BvhData.equal(GameManager.One.datas[GameManager.One.dataIndex].joint, tmp[0].joint))
            {
                GameManager.One.datas[GameManager.One.dataIndex].addMotion(tmp[0]);
            }
            else
                Log.text = "error";
        }
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
                        GameManager.One.SetObjectActive(j, true);
                    else
                        GameManager.One.SetObjectActive(j, false);
                }
            }
        }

        if (GameManager.One.dataIndex != -1)
        {
            addptBtn.SetActive(true);
            rmptBtn.SetActive(true);
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
        GameManager.One.datas[GameManager.One.dataIndex].interpolation = 0;
    }

    //切換跟線
    public void InFollowLine()
    {
        GameManager.One.isFollowLine = !GameManager.One.isFollowLine;
    }

    public void IsBlend()
    {
        GameManager.One.isBlend = !GameManager.One.isBlend;
        if(GameManager.One.isBlend)
        {
            string[] splitArray = twoMotion.text.Split(char.Parse(" "));
            int index1 = int.Parse(splitArray[0]);
            int index2 = int.Parse(splitArray[1]);
            int frame1 = int.Parse(splitArray[2]);
            int frame2 = int.Parse(splitArray[3]);
            GameManager.One.datas[GameManager.One.dataIndex].framePair.First = frame1;
            GameManager.One.datas[GameManager.One.dataIndex].framePair.First = frame2;
            GameManager.One.datas[GameManager.One.dataIndex].interpolation = 0;
            GameManager.One.datas[GameManager.One.dataIndex].motionPair.First = index1;
            GameManager.One.datas[GameManager.One.dataIndex].motionPair.Second = index2;
            GameManager.One.datas[GameManager.One.dataIndex].motionType = index1;
        }
    }

    public void SetDis()
    {
        GameManager.One.SetDis(DisText.text);
    }
}

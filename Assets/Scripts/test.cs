using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class test : MonoBehaviour
{
    public Vector3 pA;
    public Vector3 pB;


    void Update()
    {
        string s = "1 2";
        print(s.Split(char.Parse("_")));
    }

    void OnGUI()
    {
       
    }
}



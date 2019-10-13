using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class test : MonoBehaviour
{
    public Vector3 pA;
    public Vector3 pB;


    void Update()
    {
        // Assuming this is run on a unit cube.
        Vector3 between = pB - pA;
        float distance = between.magnitude;
        transform.localPosition = new Vector3(distance, transform.localPosition.y, transform.localPosition.z);
        transform.position = pA + between / 2;
        transform.LookAt(pB);
        transform.localScale = new Vector3(1, 1, between.magnitude);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 10, 100, 30), "Open File"))
        {
            Application.OpenURL((Application.dataPath) + "/Folder/File.Type");
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Roots = System.Collections.Generic.List<Joint>;

public class BvhReader : MonoBehaviour
{
    public Roots roots = new Roots();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadFile(string path)
    {
        roots.Clear();
        //string fileContent = File.ReadAllText(path);
    }
}

using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using Roots = System.Collections.Generic.List<Joint>;

static public class BvhStep
{
    //static private int Step { get; set; }
    //static private List<Pair<string, int>> pairs;

}

public class BvhUtility
{
    

    static public void ParseBvhData(ref Roots roots, string path)
    {
        StreamReader fileContent = new StreamReader(path);
        string lineStr = fileContent.ReadLine();
        if (lineStr == "HIERARCHY")
        {
            while ((lineStr = fileContent.ReadLine()) != null)
            {

            }
        }
    }
}
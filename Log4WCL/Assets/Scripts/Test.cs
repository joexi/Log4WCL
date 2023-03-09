using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        WCLParser p = new WCLParser();
        var files = p.SplitFiles("D:/Data/4.txt");
        foreach (var file in files)
        {
            p.Clear();
            p.Parse("D:/Data/" + file);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

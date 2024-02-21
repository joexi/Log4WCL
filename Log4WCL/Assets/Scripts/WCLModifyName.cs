using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCLModifyName : WCLModifier
{
    
    public override void Run()
    {
        base.Run();
        Dictionary<string, string> dic = new Dictionary<string, string>();
        foreach (var m in WCLParser.Members)
        {
            if (!dic.ContainsKey(m.MemberName)) {
                dic[m.MemberName] = "\""+GetRandomName(7)+"\"";
                Debug.Log("change name from " + m.MemberName + " to " + dic[m.MemberName]);
            }
        }
        foreach (var k in dic.Keys)
        {
            foreach (var e in WCLParser.Events)
            {
                if (e.Type != EWCLEventType.UNKNOWN)
                {
                    e.SrcData = e.SrcData.Replace(k, dic[k]);
                }
            }
        }
    }

    public string GetRandomName(int length)
    {
        string result = string.Empty;
        for (int i = 0; i < length; i++)
        { 
            int v = UnityEngine.Random.Range(97, 97 + 26);
            char c = (char)v;
            result += c;
        }
        return result;
    }
}

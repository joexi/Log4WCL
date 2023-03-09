using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WCLParser
{
    public static List<WCLCombat> Combats = new List<WCLCombat>();
    public static List<WCLEvent> Events = new List<WCLEvent>();
    public static List<WCLMember> Members = new List<WCLMember>();
    public static Dictionary<string, WCLMember> MembersDic = new Dictionary<string, WCLMember>();


    public WCLParser()
    {
       
    }

    public void Parse(string path)
    {
        //SplitFiles(path);
        ParseCombat(path);
    }
    public string[] SplitFiles(string path)
    {
        var lines = File.ReadAllLines(path);
        int index = 0;
        int lastIndex = 0;
        int combatIndex = 0;
        List<string> fileNames = new List<string>();
        foreach (string lineData in lines)
        {
            int idx = lineData.IndexOf("ENCOUNTER_END");
            if (idx >= 0)
            {
                string[] data = lineData.Substring(idx).Split(',');
                string[] fileContent = new string[index - lastIndex + 1];
                for (int i = 0; i < fileContent.Length; i++)
                {
                    fileContent[i] = lines[lastIndex + i];
                }
                combatIndex++;
                var fileName = data[2] + "-" + combatIndex + ".txt";
                fileName = fileName.Replace("\"","");
                fileNames.Add(fileName);
                var newPath = path.Replace(Path.GetFileNameWithoutExtension(path) + ".txt", fileName);
                newPath = newPath.Replace("\"", "");
                File.WriteAllLines(newPath, fileContent);
                lastIndex = index;
            }
            index++;
        }
        return fileNames.ToArray();
    }

    public void ParseCombat(string path)
    {
        WCLCombat combat = null;
        var lines = File.ReadLines(path);
        foreach (string lineData in lines)
        {
            WCLEvent e = new WCLEvent(lineData);
            if (e.Type == EWCLEventType.ENCOUNTER_START)
            {
                combat = new WCLCombat(e);
                combat.Start(e);
                Combats.Add(combat);
            }
            else if (e.Type == EWCLEventType.ENCOUNTER_END)
            {
                if (combat != null)
                {
                    combat.End(e);
                    combat = null;
                }
            }
            e.Combat = combat;
            Events.Add(e);
        }
        //foreach (var cbt in Combats)
        //{
        //    WCLLogger.LogDeath(cbt);
        //    //WCLLogger.LogDPS(cbt);
        //    //WCLLogger.LogHPS(cbt);
        //}

        foreach (var cbt in Combats)
        {
            Debug.Log("======================" + cbt.CombatName + "=========================");
            if (cbt.CombatName.Contains("弗蕾亚"))
            {
                Debug.Log("======================弗蕾亚无效伤害(P1)=========================");

                var date = WCLLogger.GetFlyP2Time(cbt);
                Dictionary<WCLMember, long> dps = new Dictionary<WCLMember, long>();
                foreach (var e in Events)
                {
                    if ((date - e.Date).TotalSeconds > 0 && e.IsDamage() && e.Target != null && e.Target.MemberName.Contains("弗蕾亚"))
                    {
                        if (!dps.ContainsKey(e.Member))
                        {
                            dps[e.Member] = 0;
                        }
                        dps[e.Member] += e.GetEventValue();
                        //e.Log();
                    }
                }
                List<KeyValuePair<WCLMember, long>> list = new List<KeyValuePair<WCLMember, long>>();
                foreach (var v in dps)
                {
                    list.Add(v);
                }
                list.Sort((a, b) =>
                {
                    return b.Value.CompareTo(a.Value);
                });
                foreach (var v in list)
                {
                    Debug.Log("dmg:" + " " + v.Key.MemberName + " " + v.Value);
                }
            }
        }
    }

    public void Clear()
    {
        Combats.Clear();
        Events.Clear();
        Members.Clear();
        MembersDic.Clear();
    }

    public void GetLog()
    { 
        
    }
}

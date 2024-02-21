using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //WCLParser p = new WCLParser();
        //var files = p.SplitFiles("D:/4.txt");
        //foreach (var file in files)
        //{
        //    p.Clear();
        //    p.Parse("D:/" + file);
        //}

        WCLParser p = new WCLParser();
        p.Parse("D:/4.txt");
        WCLLogger.LogDPS(WCLParser.Events);
        WCLModifierDamage mod = new WCLModifierDamage();
        mod.DamageSrcSkillName = new string[] { "\"内部腐烂\"" };
        mod.DamageDstRoleName = new string[] { "\"再见幼儿园-銀翼要塞\"" };
        mod.DamageDstSkillName = new string[] { "\"剑刃乱舞\"" };
        mod.Value = 1000;
        mod.Run();
        WCLLogger.LogDPS(WCLParser.Events);
        mod = new WCLModifierDamage();
        mod.DamageSrcSkillName = new string[] { "\"审判\"" };
        mod.DamageDstRoleName = new string[] { "\"再见幼儿园-銀翼要塞\"" };
        mod.DamageDstSkillName = new string[] { "\"伏击\"" };
        mod.Value = 1000;
        mod.Run();
        WCLLogger.LogDPS(WCLParser.Events);
        WCLParser.Save("D:/5.txt");

        //WCLParser.Save("D:/5.txt");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

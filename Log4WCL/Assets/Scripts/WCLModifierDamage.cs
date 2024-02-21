using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCLModifierDamage : WCLModifier
{
    public string[] DamageSrcRoleName = new string[0];
    public string[] DamageSrcSkillName = new string[0];
    public string[] DamageDstRoleName = new string[0];
    public string[] DamageDstSkillName = new string[0];
    public int Value;
    public int CurValue;

    public override void Run()
    {
        base.Run();
        Debug.Log("==========》seek damage from " + string.Join(",", DamageSrcRoleName) + " " + string.Join(",", DamageSrcSkillName));
        Debug.Log("==========》add damage to " + string.Join(",", DamageDstRoleName) + " " + string.Join(",", DamageDstSkillName));

        List<WCLEvent> srcEvents = new List<WCLEvent>();
        List<WCLEvent> dstEvents = new List<WCLEvent>();

        foreach (var e in WCLParser.Events)
        {
            if (e.IsDamage() && IsMemberMatch(e.Member, DamageSrcRoleName) && IsSkillMatch(e, DamageSrcSkillName))
            {
                srcEvents.Add(e);
            }
        }
        int per = Value / srcEvents.Count;
        foreach (var e in srcEvents)
        {
            e.SetEventValue(e.GetEventValue() - per);
            CurValue += per;
        }
        foreach (var e in WCLParser.Events)
        {
            if (e.IsDamage() && IsMemberMatch(e.Member, DamageDstRoleName) && IsSkillMatch(e, DamageDstSkillName))
            {
                dstEvents.Add(e);
            }
        }
        int perDst = CurValue / dstEvents.Count;
        foreach (var e in dstEvents)
        {
            if (CurValue > perDst)
            {
                e.SetEventValue(e.GetEventValue() + perDst);
            }
            else {
                e.SetEventValue(e.GetEventValue() + CurValue);
            }
            CurValue -= per;
            if (CurValue <= 0)
            {
                break;
            }
        }
    }

    public bool IsMemberMatch(WCLMember member, string[] roleName)
    {
        if (roleName.Length == 0)
        {
            return true;
        }
        foreach (string name in roleName)
        {
            if (member.MemberName.Equals(name))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsSkillMatch(WCLEvent e, string[] skillName)
    {
        if (skillName.Length == 0)
        {
            return true;
        }
        foreach (string name in skillName)
        {
            if (e.GetEventValue(10).Equals(name))
            {
                return true;
            }
            //Debug.LogError(e.GetEventValue(10));

        }
        return false;
    }
}

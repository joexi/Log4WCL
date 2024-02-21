using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWCLLoggerType
{ 
    HPS = 1,
    DPS = 2,
    Death = 3
}

public class WCLLogger
{
    public static void Log(WCLCombat combat, EWCLLoggerType type)
    {
        switch (type)
        {
            case EWCLLoggerType.DPS:
                LogDPS(combat);
                break;
            case EWCLLoggerType.HPS:
                LogHPS(combat);
                break;
            case EWCLLoggerType.Death:
                LogDeath(combat);
                break;
        }
    }

    public static void LogHPS(WCLCombat combat)
    {
        Dictionary<WCLMember, double> count = new Dictionary<WCLMember, double>();
        foreach (var e in WCLParser.Events)
        {
            if ((e.Type == EWCLEventType.SPELL_HEAL || e.Type ==  EWCLEventType.SPELL_PERIODIC_HEAL) && e.Combat == combat)
            {
                if (e.Member != null && e.Member.Type == EMemberType.Player)
                {
                    if (!count.ContainsKey(e.Member))
                    {
                        count[e.Member] = e.GetEventValue() / combat.CombatDuration;
                    }
                    else
                    {
                        count[e.Member] += e.GetEventValue() / combat.CombatDuration;
                    }
                }
            }
        }
        List<KeyValuePair<WCLMember, double>> list = new List<KeyValuePair<WCLMember, double>>();
        foreach (var v in count)
        {
            list.Add(v);
        }
        list.Sort((a, b) =>
        {
            return b.Value.CompareTo(a.Value);
        });
        foreach (var v in list)
        {
            Debug.Log("hps:" + " " + v.Key.MemberName + " " + v.Value);
        }
    }

    public static void LogDPS(WCLCombat combat)
    {
        Dictionary<WCLMember, double> count = new Dictionary<WCLMember, double>();
        foreach (var e in WCLParser.Events)
        {
            if ((e.Type == EWCLEventType.SPELL_DAMAGE ||
                e.Type == EWCLEventType.SWING_DAMAGE ||
                e.Type == EWCLEventType.DAMAGE_SHIELD) && e.Combat == combat)
            {
                if (e.Member != null && e.Member.Type == EMemberType.Player)
                {
                    if (!count.ContainsKey(e.Member))
                    {
                        count[e.Member] = e.GetEventValue() / combat.CombatDuration;
                    }
                    else
                    {
                        count[e.Member] += e.GetEventValue() / combat.CombatDuration;
                    }
                }
            }
        }
        List<KeyValuePair<WCLMember, double>> list = new List<KeyValuePair<WCLMember, double>>();
        foreach (var v in count)
        {
            list.Add(v);
        }
        list.Sort((a, b) =>
        {
            return b.Value.CompareTo(a.Value);
        });
        foreach (var v in list)
        {
            Debug.Log("dps:" + " " + v.Key.MemberName + " " + v.Value);
        }
    }

    public static void LogDPS(List<WCLEvent> events)
    {
        Dictionary<WCLMember, double> count = new Dictionary<WCLMember, double>();
        var duration = (events[events.Count - 1].Date - events[0].Date).TotalSeconds;
        foreach (var e in events)
        {
            if (e.IsDamage())
            {
                if (e.Member != null && e.Member.Type == EMemberType.Player)
                {
                    if (!count.ContainsKey(e.Member))
                    {
                        count[e.Member] = e.GetEventValue() / duration;
                    }
                    else
                    {
                        count[e.Member] += e.GetEventValue() / duration;
                    }
                }
            }
        }
        List<KeyValuePair<WCLMember, double>> list = new List<KeyValuePair<WCLMember, double>>();
        foreach (var v in count)
        {
            list.Add(v);
        }
        list.Sort((a, b) =>
        {
            return b.Value.CompareTo(a.Value);
        });
        foreach (var v in list)
        {
            Debug.Log("dps:" + " " + v.Key.MemberName + " " + v.Value);
        }
    }

    public static System.DateTime GetFirstDiedTime(WCLCombat combat)
    {
        foreach (var e in WCLParser.Events)
        {
            if (e.Type == EWCLEventType.UNIT_DIED && e.Combat == combat && e.Member.Type == EMemberType.Player)
            {
                return e.Date;
            }
        }
        return combat.CombatEndTime;
    }

    public static System.DateTime GetFlyP2Time(WCLCombat combat)
    {
        Dictionary<string, int> count = new Dictionary<string, int>();
        count["\"古代水之精魂\""] = 4;
        count["\"风暴鞭笞者\""] = 4;
        count["\"迅疾鞭笞者\""] = 4;
        count["\"远古监护者\""] = 2;
        count["\"爆炸鞭笞者\""] = 20;
        foreach (var e in WCLParser.Events)
        {
            if (e.Type == EWCLEventType.UNIT_DIED && e.Combat == combat)
            {
                if (e.Member != null)
                {
                    if (count.ContainsKey(e.Member.MemberName)) {
                        count[e.Member.MemberName]--;
                    }
                }
            }
            bool clear = true;
            foreach (var kv in count)
            {
                if (kv.Value != 0)
                {
                    clear = false;
                }
            }
            if (clear)
            {
                return e.Date;
            }
        }
        foreach (var kv in count)
        {
            Debug.LogError(kv.Key + "" + kv.Value);
        }
        return combat.CombatEndTime;
    }

    public static void LogDeath(WCLCombat combat)
    {
        Dictionary<WCLMember, int> count = new Dictionary<WCLMember, int>();
        foreach (var e in WCLParser.Events)
        {
            if (e.Type == EWCLEventType.UNIT_DIED && e.Combat == combat)
            {
                if (e.Member != null && e.Member.Type == EMemberType.Player)
                {
                    List<WCLEvent> events = GetRelationedEvents(combat, e.Member, e);
                    for (int i = 0; i < events.Count; i++)
                    {
                        var de = events[i];
                        var s1 = (de.Date - e.Combat.CombatStartTime).TotalSeconds;
                        var t1 = (s1 / 60).ToString("00") + ":" + (s1 % 60).ToString("00") + "." + (((long)(s1 * 1000)) % 1000).ToString("000");
                        if (de.IsDamage())
                        { 
                            Debug.Log(t1 + " " + e.Member.MemberName + "受到伤害".ToColor16(Color.yellow) + de.GetEventValue() + " 来自"+ de.Member.MemberName + "的" + de.GetDetailName());
                        }
                        if (de.IsHeal())
                        {
                            Debug.Log(t1 + " " + e.Member.MemberName + "受到治疗".ToColor16(Color.green) + de.GetEventValue() + " 来自" + de.Member.MemberName + "的" + de.GetDetailName());
                        }
                    }
                    var s2 = (e.Date - e.Combat.CombatStartTime).TotalSeconds;
                    var t2 = (s2 / 60).ToString("00") + ":" + (s2 % 60).ToString("00") + "." + (((long)(s2 * 1000)) % 1000).ToString("000");
                    Debug.Log(t2 + " " + e.Member.MemberName + "在" + combat.CombatName + "的战斗中" + "死亡".ToColor16(Color.red));
                }
            }
        }
    }


    public static void LogBuff(WCLCombat combat, string buffName)
    {
        Dictionary<WCLMember, int> count = new Dictionary<WCLMember, int>();
        foreach (var e in WCLParser.Events)
        {
            if (e.Type == EWCLEventType.SPELL_AURA_APPLIED && e.Combat == combat)
            {
                if (e.Member != null && e.Member.Type == EMemberType.Player && e.GetDetailName().Contains(buffName))
                {
                    var s2 = (e.Date - e.Combat.CombatStartTime).TotalSeconds;
                    var t2 = (s2 / 60).ToString("00") + ":" + (s2 % 60).ToString("00") + "." + (((long)(s2 * 1000)) % 1000).ToString("000");
                    Debug.Log(t2 + " " + e.Member.MemberName + "获得Buff " + buffName);
                }
            }
        }

        // LogCommon(EWCLEventType.UNIT_DIED, EMemberType.Player, combat);
    }

    public static List<WCLEvent> GetRelationedEvents(WCLCombat combat, WCLMember member, WCLEvent death)
    {
        List<WCLEvent> results = new List<WCLEvent>();
        foreach (var e in WCLParser.Events)
        {
            if (e == death)
            {
                break;
            }
            var t = (death.Date - e.Date).TotalSeconds;
            if (e.Combat == combat &&
                t < 5 &&
                (e.Target == member))
            {
                results.Add(e);
            }
        }
        return results;
    }

    public static void LogCommon(EWCLEventType type, EMemberType memberType, WCLCombat cbt)
    {
        Dictionary<WCLMember, int> count = new Dictionary<WCLMember, int>();
        foreach (var e in WCLParser.Events)
        {
            if (e.Type == type && e.Combat == cbt)
            {
                if (e.Member != null && e.Member.Type == memberType)
                {
                    if (!count.ContainsKey(e.Member))
                    {
                        count[e.Member] = e.GetEventValue();
                    }
                    else
                    {
                        count[e.Member] += e.GetEventValue();
                    }
                }
            }
        }
        List<KeyValuePair<WCLMember, int>> list = new List<KeyValuePair<WCLMember, int>>();
        foreach (var v in count)
        {
            list.Add(v);
        }
        list.Sort((a, b) =>
        {
            return b.Value.CompareTo(a.Value);
        });
        foreach (var v in list)
        {
            Debug.Log(type + " " + v.Key.MemberName + " " + v.Value);
        }
    }
}

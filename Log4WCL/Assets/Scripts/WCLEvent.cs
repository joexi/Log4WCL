using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum EWCLEventType
{
    COMBAT_LOG_VERSION,
    ZONE_CHANGE,
    MAP_CHANGE,
    SPELL_PERIODIC_HEAL,
    SPELL_AURA_APPLIED,
    SPELL_AURA_REFRESH,
    SPELL_CAST_SUCCESS,
    SPELL_AURA_REMOVED,
    SPELL_CREATE,
    SPELL_CAST_START,
    SPELL_ENERGIZE,
    SPELL_CAST_FAILED,
    SPELL_HEAL,
    ENVIRONMENTAL_DAMAGE,
    SPELL_SUMMON,
    SPELL_DAMAGE,
    UNIT_DIED,
    SPELL_BUILDING_DAMAGE,
    SWING_DAMAGE,
    SPELL_MISSED,
    SPELL_DRAIN,
    SPELL_PERIODIC_DAMAGE,
    SPELL_AURA_APPLIED_DOSE,
    SPELL_INTERRUPT,
    SPELL_ABSORBED,
    SPELL_RESURRECT,
    DAMAGE_SPLIT,
    ENCOUNTER_START,
    EMOTE,
    SPELL_INSTAKILL,
    ENCOUNTER_END,
    SPELL_PERIODIC_ENERGIZE,
    PARTY_KILL,
    SPELL_AURA_REMOVED_DOSE,
    SWING_MISSED,
    RANGE_DAMAGE,
    UNIT_DESTROYED,
    SPELL_DISPEL,
    SPELL_PERIODIC_MISSED,
    SPELL_EXTRA_ATTACKS,
    DAMAGE_SHIELD,
    DAMAGE_SHIELD_MISSED,
    SPELL_STOLEN,
    SPELL_DISPEL_FAILED,
    SPELL_AURA_BROKEN_SPELL,
    SPELL_AURA_BROKEN,
    RANGE_MISSED,
    SPELL_PERIODIC_LEECH,
    SWING_DAMAGE_LANDED,
    ENCHANT_APPLIED,
    COMBATANT_INFO,
    ENCHANT_REMOVED,
    CHALLENGE_MODE_END,
    CHALLENGE_MODE_START,
    UNKNOWN = 999,
}
public class WCLEvent
{
    public static System.Collections.Generic.List<string> TypeSet = new System.Collections.Generic.List<string>();
    public DateTime Date;
    public string SrcData;
    public string[] EventData;
    public string EventName;
    public EWCLEventType Type;

    public WCLMember Member;
    public WCLMember Target;
    public WCLCombat Combat;

    public override string ToString()
    {
        return Type + "|" + Member;
    }

    public WCLEvent(string data)
    {
        this.SrcData = data;
        int index = data.IndexOf("  ");
        if (index < 0)
        {
            this.Type = EWCLEventType.UNKNOWN;
            return;
        }
        var d = data.Substring(0, index);
        var md = d.Split(' ');
        var dt = md[0].Split('/');
        var month = int.Parse(dt[0]);
        var day = int.Parse(dt[1]);
        var time = md[1].Split(':');
        var hours = int.Parse(time[0]);
        var minutes = int.Parse(time[1]);
        var seconds = double.Parse(time[2]);

        Date = new DateTime(DateTime.Now.Year, month, day, hours, minutes, 0);
        Date = Date.AddSeconds(seconds);
     

        EventData = data.Substring(index + 2).Split(',');

        if (EventData.Length > 0)
        {
            if (!TypeSet.Contains(EventData[0]))
            {
                TypeSet.Add(EventData[0]);
            }
            try
            {
                Type = (EWCLEventType)System.Enum.Parse(typeof(EWCLEventType), EventData[0]);
            }
            catch
            {
                Debug.LogError("found new type "+ EventData[0]);
            }
        }
        this.InitMember();
        if (Type == EWCLEventType.SPELL_SUMMON)
        {
            this.Target.Parent = this.Member;
        }
    }

    public void InitMember()
    {
        if (this.Type == EWCLEventType.COMBAT_LOG_VERSION ||
            this.Type == EWCLEventType.CHALLENGE_MODE_END ||
            this.Type == EWCLEventType.CHALLENGE_MODE_START ||
            this.Type == EWCLEventType.COMBATANT_INFO)
        {
            return;
        }
        var casterType = this.GetMemberType();
        var casterName = this.GetMemberName();

        
        if (!WCLParser.MembersDic.ContainsKey(casterName))
        {
            WCLMember member = new WCLMember(casterType, casterName);
            WCLParser.Members.Add(member);
            //Debug.LogError((WCLParser.Events.Count + 1) +" "  + member.MemberName);
            WCLParser.MembersDic[casterName] = member;
        }
        this.Member = WCLParser.MembersDic[casterName];

        var targetName = this.GetTargetName();
        var targetType = this.GetTargetType();
        if (targetName.Length > 0)
        {
            if (!WCLParser.MembersDic.ContainsKey(targetName))
            {
                WCLMember member = new WCLMember(targetType, targetName);
                WCLParser.Members.Add(member);
                WCLParser.MembersDic[targetName] = member;
            }
            this.Target = WCLParser.MembersDic[targetName];
        }
        
    }

    public int GetEventValue()
    {
        switch (this.Type)
        {
            case EWCLEventType.SPELL_DAMAGE:
            case EWCLEventType.SWING_DAMAGE:
            case EWCLEventType.SPELL_HEAL:
            case EWCLEventType.SPELL_PERIODIC_DAMAGE:
            case EWCLEventType.SPELL_BUILDING_DAMAGE:
                return int.Parse(EventData[28]);
            case EWCLEventType.SWING_DAMAGE_LANDED:
                return int.Parse(EventData[18]);
            case EWCLEventType.UNIT_DIED:
                return 1;
        }
        return 0;
    }

    public string GetEventValue(int index)
    {
        if (EventData.Length > index)
        {
            return EventData[index];
        }
        return string.Empty;
    }

    public void SetEventValue(int v)
    {
        switch (this.Type)
        {
            case EWCLEventType.SPELL_DAMAGE:
            case EWCLEventType.SWING_DAMAGE:
            case EWCLEventType.SPELL_HEAL:
            case EWCLEventType.SPELL_PERIODIC_DAMAGE:
            case EWCLEventType.SPELL_BUILDING_DAMAGE:
                EventData[28] = v.ToString();
                break;
            case EWCLEventType.SWING_DAMAGE_LANDED:
                EventData[18] = v.ToString();
                break;
            case EWCLEventType.UNIT_DIED:
                break;
        }
        UpdateSrc();
    }

    public void UpdateSrc()
    {
        if (EventData.Length > 0)
        {
            this.SrcData = this.Date.ToString("MM/dd HH:mm:ss.fff") + "  " + string.Join(",", EventData);
        }
    }

    public string GetMemberType()
    {
        switch (this.Type)
        {
            case EWCLEventType.UNIT_DIED:
                return EventData[5];
        }
        return EventData[1];
    }

    public string GetMemberName()
    {
        switch (this.Type)
        {
            case EWCLEventType.UNIT_DIED:
                return EventData[6];
        }
        return EventData[2];
    }

    public bool IsDamage()
    {
        return this.Type == EWCLEventType.SPELL_DAMAGE ||
            this.Type == EWCLEventType.SWING_DAMAGE ||
            this.Type == EWCLEventType.SWING_DAMAGE_LANDED ||
            this.Type == EWCLEventType.ENVIRONMENTAL_DAMAGE ||
            this.Type == EWCLEventType.SPELL_PERIODIC_DAMAGE ||
            this.Type == EWCLEventType.RANGE_DAMAGE;
    }

    public void Log()
    {
        if (this.IsDamage())
        { 
             Debug.Log(this.Date + " " + this.Target.MemberName + "受到伤害".ToColor16(Color.yellow) + this.GetEventValue() + " 来自"+ this.Member.MemberName + "的" + this.GetDetailName());
        }
    }
    public bool IsHeal()
    {
        return this.Type == EWCLEventType.SPELL_HEAL || this.Type == EWCLEventType.SPELL_PERIODIC_HEAL;
    }

    public string GetTargetType()
    {
        switch (this.Type)
        {
            case EWCLEventType.SPELL_DAMAGE:
            case EWCLEventType.SWING_DAMAGE:
            case EWCLEventType.SPELL_HEAL:
            case EWCLEventType.SWING_DAMAGE_LANDED:
                return EventData[5];
            case EWCLEventType.SPELL_SUMMON:
                return EventData[5];
        }
        return string.Empty;
    }

    public string GetDetailName()
    {
        switch (this.Type)
        {
            case EWCLEventType.SPELL_DAMAGE:
            case EWCLEventType.SPELL_HEAL:
            case EWCLEventType.SPELL_AURA_APPLIED:
                return EventData[10];
            case EWCLEventType.SWING_DAMAGE:
            case EWCLEventType.SWING_DAMAGE_LANDED:
                return "普攻";
        }
        return string.Empty;
    }

    public string GetTargetName()
    {
        switch (this.Type)
        {
            case EWCLEventType.SPELL_DAMAGE:
            case EWCLEventType.SWING_DAMAGE_LANDED:
                return EventData[6];
            case EWCLEventType.SPELL_HEAL:
                return EventData[6];
            case EWCLEventType.SPELL_SUMMON:
                return EventData[6];
        }
        return string.Empty;
    }
}

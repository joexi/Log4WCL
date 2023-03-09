using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMemberType
{
    Other = 0,
    Player = 1,
    Pet = 2,
    Creature = 3,
}

public enum EMemberClass { 
    None = 0,
    DeathKnight = 1,
    Druid = 2,
    Hunter = 3,
    Mage = 4,
    Paladin = 5,
    Priest = 6,
    Rogue = 7,
    Shaman = 8,
    Warlock = 9,
    Warrior = 10
}
public class WCLMember
{
    public EMemberType Type;
    public EMemberClass Class;
    public string MemberName;
    public string MemberID;

    public WCLMember Parent;

    public WCLMember(string type, string name)
    {
        MemberName = name;
        MemberID = type;
        if (type.Contains("Player"))
        {
            Type = EMemberType.Player;
        }
        else if (type.Contains("Creature"))
        {
            Type = EMemberType.Creature;
        }
        else if (type.Contains("Pet"))
        {
            Type = EMemberType.Pet;
        }
        else
        {
            Type = EMemberType.Other;
        }
        //if (Type == EMemberType.Creature)
        //{
        //    InitParent();
        //}
    }

    public override string ToString()
    {
        return Type + " " + Class + " " + MemberName;
    }

    public void InitParent()
    {
        if (this.Parent != null)
        {
            return;
        }
        foreach (var e in WCLParser.Events)
        {
            if (e.Type == EWCLEventType.SPELL_SUMMON)
            {
                if (e.Target != null && e.Target.MemberID == this.MemberID)
                {
                    this.Parent = e.Member;
                }
            }
        }
    }

    public bool Is(string key)
    {
        return this.MemberName.Contains(key);
    }
}

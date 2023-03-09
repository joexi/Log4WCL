using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WCLCombat
{
    public string CombatName;
    public DateTime CombatStartTime;
    public DateTime CombatEndTime;
    public double CombatDuration;
    public List<WCLMember> Members;

        
    public WCLCombat(WCLEvent e)
    {
        CombatName = e.EventData[2];
    }

    public void Start(WCLEvent e)
    {
        CombatStartTime = e.Date;
    }

    public void End(WCLEvent e)
    {
        CombatEndTime = e.Date;
        CombatDuration = (CombatEndTime - CombatStartTime).TotalSeconds;
    }
}

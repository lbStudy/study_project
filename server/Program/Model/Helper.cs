using System;
using System.Collections.Generic;
using Base;
using Data;

public static class Helper
{
    public static void OverlayAward(AwardInfo from, AwardInfo to)
    {
        if (from == null || to == null)
            return;
        if (from.itemids == null || from.itemids.Count == 0)
            return;
        if (from.counts == null || from.counts.Count == 0)
            return;
        if (from.itemids.Count != from.counts.Count)
            return;
            
        if (to.itemids == null)
        {
            to.itemids = new List<int>();
        }
        if (to.counts == null)
        {
            to.counts = new List<int>();
        }
        to.itemids.AddRange(from.itemids);
        to.counts.AddRange(from.counts);
    }
    public static void AddAward(Player player, AwardInfo award, LogAction logAction)
    {
        if(award == null 
            || award.itemids == null 
            || award.itemids.Count == 0 
            || award.counts == null 
            || award.counts.Count == 0)
        {
            return;
        }
        for(int i = 0; i < award.itemids.Count; i++)
        {
            if(i < award.counts.Count)
            {
                if (award.itemids[i] == 1)
                    player.CommonData.Add(D_AttributeType.roomcard, award.counts[i] * 100, true, logAction);
            }
            else
            {
                break;
            }
        }
        player.CommonData.SynchroData();
    }
}

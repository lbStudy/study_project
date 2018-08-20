using System;
using System.Collections.Generic;
using Data;
using Base;

public partial class PlayerDataHandle
{
    private static PlayerDataHandle instance;
    public static PlayerDataHandle Instance {
        get
        {
            if(instance == null)
            {
                instance = new PlayerDataHandle();
            }
            return instance;
        }
    }

    private PlayerAttribute[] attributeHandleFuncs = new PlayerAttribute[(int)D_AttributeType.Max];

    public void Set(Player player, PlayerDetailData detail, int attrType, long val, LogAction logAction)
    {
        if (attributeHandleFuncs[attrType] != null)
            attributeHandleFuncs[attrType].SetValue(player, detail, val, logAction);
        else
        {
            Console.WriteLine($"error: Not exist player attribute({((D_AttributeType)attrType).ToString()}) handle.");
        }
    }
    public void Add(Player player, PlayerDetailData detail, int attrType, long val, LogAction logAction)
    {
        if (attributeHandleFuncs[attrType] != null)
            attributeHandleFuncs[attrType].AddValue(player, detail, val, logAction);
        else
            Console.WriteLine($"error: Not exist player attribute({((D_AttributeType)attrType).ToString()}) handle.");
    }
    public void Sub(Player player, PlayerDetailData detail, int attrType, long val, LogAction logAction)
    {
        if (attributeHandleFuncs[attrType] != null)
            attributeHandleFuncs[attrType].SubValue(player, detail, val, logAction);
        else
            Console.WriteLine($"error: Not exist player attribute({((D_AttributeType)attrType).ToString()}) handle.");
    }
    public long Get(Player player, int attrType)
    {
        if (attributeHandleFuncs[attrType] != null)
            return attributeHandleFuncs[attrType].GetValue(player);
        else
        {
            if(attrType < (int)D_AttributeType.Max)
            {
                Console.WriteLine($"error: Not exist player attribute({((D_AttributeType)attrType).ToString()}) handle.");
            }
            else
            {
                Console.WriteLine($"error: Not exist player attribute({attrType}) handle.");
            }
        }
            
        return -1;
    }
}

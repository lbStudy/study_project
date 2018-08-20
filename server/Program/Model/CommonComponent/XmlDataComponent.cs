using System;
using System.Collections.Generic;
using Base;
using System.IO;
using Config;

public class XmlDataComponent : Component,IAwake
{
    public static XmlDataComponent Instance;
    public WorldMapConfig worldMapConfig;
    public void Awake()
    {
        Instance = this;
        LoadXml();
    }
    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        base.Dispose();
        Instance = null;
    }
    public void LoadXml()
    {
        string content = File.ReadAllText(ConstDefine.xmlPath + "worldmap.xml");
        worldMapConfig = new WorldMapConfig();
        worldMapConfig.ParseXml(content);
    }
    public RoomConfig LoadRoomConfig(ref string cfname)
    {
        string content = File.ReadAllText(ConstDefine.xmlPath + cfname + ".xml");
        RoomConfig roomConfig = new RoomConfig();
        roomConfig.ParseXml(ref content);
        return roomConfig;
    }
}

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Base;

public class ConstConfigComponent : Component, IAwake
{
    public static ConstConfigComponent Instance;
    static ConstConfig constConfig;
    public static ConstConfig ConstConfig { get { return constConfig; } }
    public void Awake()
    {
        Instance = this;
        Load();
    }
    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        base.Dispose();
        Instance = null;
        constConfig = null;
    }
    public void Load()
    {
        XmlDocument xmldoc = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        XmlReader reader = XmlReader.Create("../../constconfig.xml", settings);
        xmldoc.Load(reader);
        using (StringReader sr = new StringReader(xmldoc.OuterXml))
        {
            XmlSerializer xmldes = new XmlSerializer(typeof(ConstConfig));
            constConfig = xmldes.Deserialize(sr) as ConstConfig;
        }
        reader.Close();
    }
}
[XmlRoot("ConstConfig")]
public class ConstConfig
{
    [XmlElement("PlayerDataSaveCount")]
    public int PlayerDataSaveCount;

    [XmlElement("PlayerDetectionInterval")]
    public int PlayerDetectionInterval;

    [XmlElement("PlayerQuickDetectionInterval")]
    public int PlayerQuickDetectionInterval;

    [XmlElement("TimerDetectionInterval")]
    public int TimerDetectionInterval;

    [XmlElement("PlayerDetectionCount")]
    public int PlayerDetectionCount;

    [XmlElement("PlayerDataWriteToDBInterval")]
    public int PlayerDataWriteToDBInterval;

    [XmlElement("QuickWriteToDBInterval")]
    public int QuickWriteToDBInterval;

    [XmlElement("PlayerDataWriteToDBCount")]
    public int PlayerDataWriteToDBCount;

    [XmlElement("GridWidth")]
    public int GridWidth;

    [XmlElement("GridHeigh")]
    public int GridHeigh;

    [XmlElement("GateVerifyWaitTime")]
    public int GateVerifyWaitTime;

    [XmlElement("TotalMemberCountInRoom")]
    public int TotalMemberCountInRoom;

    [XmlElement("SynAreaLayer")]
    public int SynAreaLayer;

    [XmlElement("PingTime")]
    public int PingTime;

    [XmlElement("ServerPingTime")]
    public int ServerPingTime;
    
    [XmlElement("ServerSendPing")]
    public int ServerSendPing;

    [XmlElement("OfflineTime")]
    public int OfflineTime;

    [XmlElement("LeaveRoomVoteTime")]
    public long LeaveRoomVoteTime;

    [XmlElement("RoomEmptyTime")]
    public long RoomEmptyTime;

    [XmlElement("paistr")]
    public string paistr;

    [XmlElement("RecordRemoveTime")]
    public long RecordRemoveTime;

    [XmlElement("ServerInfoTime")]
    public long ServerInfoTime;

    [XmlElement("IsNoSDKLogin")]
    public int IsNoSDKLogin;

    [XmlElement("mincount")]
    public int mincount;

    [XmlElement("WaitEnterRoomTime")]
    public int WaitEnterRoomTime;

    [XmlElement("PreparetTime")]
    public int PreparetTime;

    [XmlElement("Score")]
    public int Score;

    [XmlArray("CostRoomCardNums")]
    [XmlArrayItem("val")]
    public List<int> CostRoomCardNums;

    [XmlArray("RoomPeopleNums")]
    [XmlArrayItem("val")]
    public List<int> RoomPeopleNums;

    [XmlArray("Jushus")]
    [XmlArrayItem("val")]
    public List<int> Jushus;

    [XmlArray("Mapais")]
    [XmlArrayItem("val")]
    public List<byte> Mapais;

    [XmlArray("Specialpais_val")]
    [XmlArrayItem("val")]
    public List<int> Specialpais_val;

    [XmlArray("Specialpais_daoshu")]
    [XmlArrayItem("val")]
    public List<int> Specialpais_daoshu;

    [XmlArray("Shangduns_daoshu")]
    [XmlArrayItem("val")]
    public List<int> Shangduns_daoshu;

    [XmlArray("Zhongduns_daoshu")]
    [XmlArrayItem("val")]
    public List<int> Zhongduns_daoshu;

    [XmlArray("Xiaduns_daoshu")]
    [XmlArrayItem("val")]
    public List<int> Xiaduns_daoshu;

    [XmlElement("InitRoomCard")]
    public int InitRoomCard;

    [XmlArray("Activity_login")]
    [XmlArrayItem("val")]
    public List<string> Activity_login;

    [XmlArray("Activity_7daylogin")]
    [XmlArrayItem("val")]
    public List<string> Activity_7daylogin;

    [XmlArray("Activity_share")]
    [XmlArrayItem("val")]
    public List<string> Activity_share;

    [XmlArray("Activity_bing")]
    [XmlArrayItem("val")]
    public List<string> Activity_bing;

    [XmlArray("Activity_daili")]
    [XmlArrayItem("val")]
    public List<string> Activity_daili;
}
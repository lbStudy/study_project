using Data;
using Base;
using System.Collections.Generic;

public partial class PlayerCommonData
{
    private PlayerDetailData detailData;            //详细数据
    private PlayerExtraData extraData;              //额外数据

    List<int> changeDatas = new List<int>();
    
    Player player;

    public string name { get { return detailData.name; } }
    public List<int> Finish7DayAwards { get { return detailData.finish7DayAwards; } }
    public void Init(Player player, PlayerDetailData detailData, PlayerExtraData extraData)
    {
        this.detailData = detailData;
        this.extraData = extraData;
        this.player = player;
        RefreshData();
    }
    public void RefreshData()
    {
        if(detailData.firstLoginTime == 0)
        {
            detailData.firstLoginTime = Game.Instance.Sec;
        }

        bool isChange = false;
        
        if(extraData.activitys != null && extraData.activitys.Count > 0)
        {
            for(int i = extraData.activitys.Count - 1; i >= 0; i--)
            {
                ActivityInfo info = ActivityManagerComponent.Instance.FindById(extraData.activitys[i].id);
                if(info == null)
                {
                    extraData.activitys.RemoveAt(i);
                    isChange = true;
                }
            }
        }

        if (isChange)
        {
            this.player.SetDataModule(PlayerDataModule.Extra);
        }
    }
    public void Set(D_AttributeType attrType, long val, bool isSynchro, LogAction logAction = LogAction.None)
    {
        PlayerDataHandle.Instance.Set(player, detailData, (int)attrType, val, logAction);
        if (isSynchro)
            changeDatas.Add((int)attrType);
        this.player.SetDataModule(PlayerDataModule.Detail);
    }
    public void Add(D_AttributeType attrType, long val, bool isSynchro, LogAction logAction = LogAction.None)
    {
        PlayerDataHandle.Instance.Add(player, detailData, (int)attrType, val, logAction);
        if (isSynchro)
            changeDatas.Add((int)attrType);
        this.player.SetDataModule(PlayerDataModule.Detail);
    }
    public void Sub(D_AttributeType attrType, long val, bool isSynchro, LogAction logAction = LogAction.None)
    {
        PlayerDataHandle.Instance.Sub(player, detailData, (int)attrType, val, logAction);
        if (isSynchro)
            changeDatas.Add((int)attrType);
        this.player.SetDataModule(PlayerDataModule.Detail);
    }
    private long Get(int attrType)
    {
        return PlayerDataHandle.Instance.Get(player, (int)attrType);
    }
    public PlayerActivityInfo FindActivity(int activityid)
    {
        if (extraData.activitys == null)
            return null;
        return extraData.activitys.Find(x => x.id == activityid);
    }
    public void AddActivity(PlayerActivityInfo activityInfo)
    {
        if (extraData.activitys == null)
        {
            extraData.activitys = new List<PlayerActivityInfo>();
        }
        extraData.activitys.Add(activityInfo);
        this.player.SetDataModule(PlayerDataModule.Extra);
    }
    public bool RemoveActivity(int activityid, bool isSynchro)
    {
        if (extraData.activitys == null)
        {
            return false;
        }
        int count = extraData.activitys.RemoveAll(x => x.id == activityid);
        if(count > 0)
        {
            this.player.SetDataModule(PlayerDataModule.Extra);
            if (isSynchro)
            {
                SynchroActivity();
            }
            return true;
        }
        return false;
    }
    public void SynchroActivity()
    {
        SC_RefreshPlayerActivityMessage gate2c = new SC_RefreshPlayerActivityMessage();
        gate2c.id = player.Id;
        gate2c.playerActivitys = extraData.activitys;
        TranspondComponent.instance.ToClient(gate2c, player.Id);
    }
    public void Add7DayLoginAward(int day)
    {
        if (detailData.finish7DayAwards == null)
            detailData.finish7DayAwards = new List<int>();
        detailData.finish7DayAwards.Add(day);
        this.player.SetDataModule(PlayerDataModule.Detail);
    }
    public bool Is7DayLoginAward(int day)
    {
        return detailData.finish7DayAwards != null && detailData.finish7DayAwards.Contains(day);
    }
    public int Award7DayLoginCount()
    {
        return detailData.finish7DayAwards == null ? 0 : detailData.finish7DayAwards.Count;
    }
    public void SetRoomid(long roomid)
    {
        detailData.roomid = roomid;
        this.player.SetDataModule(PlayerDataModule.Detail);
    }
    //同步数据
    public void SynchroData()
    {
        if(changeDatas.Count == 0)
        {
            return;
        }
        SC_PlayerCommonDataSynchroMessage msg = ProtocolDispatcher.Instance.Take<SC_PlayerCommonDataSynchroMessage>((int)ProtoEnum.SC_PlayerCommonDataSynchroMessage);
        try
        {
            msg.id = player.Id;
            msg.changeDataDic = new Dictionary<int, long>();
            for (int i = 0; i < changeDatas.Count; i++)
            {
                msg.changeDataDic.Add(changeDatas[i], Get(changeDatas[i]));
            }
            TranspondComponent.instance.ToClient(msg, player.Id);
            changeDatas.Clear();
        }
        finally
        {
            ProtocolDispatcher.Instance.Back(msg);
        }
    }
}

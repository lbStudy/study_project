using Base;
using Data;
using MongoDB.Driver;
using System.Collections.Generic;
using System.IO;

public enum PlayerState
{
    None,
    Online,         //在线
    Offline,        //离线
    Background      //切入后台
}
public enum PlayerTag
{
    CreatingRoom,
    RequestMatch,
    Max
}
public enum PlayerDataModule
{
    Detail = 1,
    Extra = 2,
}
public class Player : Entity
{
    Dictionary<AppType, int> serverIdDic = new Dictionary<AppType, int>();
    public PlayerState state;

    public int GetServerId(AppType appType)
    {
        return serverIdDic[appType];
    }

    private bool[] tags;
    private int dataModule;
    private PlayerData data;
    public PlayerData Data { get { return data; } }
    private PlayerCommonData commonData;
    public PlayerCommonData CommonData { get { return commonData; } }
    private PlayerTemporaryData temporaryData;
    public PlayerTemporaryData TemporaryData { get { return temporaryData; } }
    public override void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }
        base.Dispose();

        serverIdDic.Clear();
        state = PlayerState.None;
    }
    public Player() : base(EntityType.Player)
    {
        dataModule = 0;
        tags = new bool[(int)PlayerTag.Max];
        temporaryData = new PlayerTemporaryData(this);
    }
    public void Init(PlayerData data)
    {
        this.data = data;
        commonData = new PlayerCommonData();
        commonData.Init(this, data.detailData, data.extraData);
        temporaryData.firstLoginDay = TimeHelper.TotalDays(data.detailData.firstLoginTime);
    }
    public void SetState(PlayerState state)
    {
        this.state = state;
    }
    public bool IsState(PlayerState state)
    {
        return this.state == state;
    }
    public void SetTag(PlayerTag tag)
    {
        this.tags[(int)tag] = true;
    }
    public void RemoveTag(PlayerTag tag)
    {
        this.tags[(int)tag] = false;
    }
    public bool IsExistTag(PlayerTag tag)
    {
        return this.tags[(int)tag];
    }
    public void ResetTag()
    {
        for(int i = 0; i < tags.Length; i++)
        {
            tags[i] = false;
        }
    }
    public void SetDataModule(PlayerDataModule dataModule)
    {
        this.dataModule |= (int)dataModule;
    }
    public bool IsCanRemove()
    {
        return IsState(PlayerState.Offline) && (Game.Instance.Msec - temporaryData.offlineTime > ConstConfigComponent.ConstConfig.OfflineTime);
    }
    List<UpdateDefinition<PlayerData>> updateParams = new List<UpdateDefinition<PlayerData>>();
    public List<UpdateDefinition<PlayerData>> GetSaveData()
    {
        if (this.dataModule == 0)
            return null;
        updateParams.Clear();
        if ((this.dataModule & (int)PlayerDataModule.Detail) > 0)
        {
            updateParams.Add(Builders<PlayerData>.Update.Set(x => x.detailData, data.detailData));
        }
        if((this.dataModule & (int)PlayerDataModule.Extra) > 0)
        {
            updateParams.Add(Builders<PlayerData>.Update.Set(x => x.extraData, data.extraData));
        }

        this.dataModule = 0;
        return updateParams;
    }
    public bool IsNeedSave()
    {
        return this.dataModule > 0;
    }
    public void SendMsg(MemoryStream stream)
    {

    }
}

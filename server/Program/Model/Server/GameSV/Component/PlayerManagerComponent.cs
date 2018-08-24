using Base;
using System;
using System.Collections.Generic;
using Data;
using System.Threading.Tasks;
using System.IO;
using MongoDB.Driver;

public class PlayerManagerComponent : Component, IAwake
{
    LinkedList<Player> players = new LinkedList<Player>();
    Dictionary<long, LinkedListNode<Player>> plyaerDic = new Dictionary<long, LinkedListNode<Player>>();
    IntervalTask playerDataSaveTask;
    public static PlayerManagerComponent Instance;

    LinkedListNode<Player> curNode;
    List<long> removePlayers = new List<long>();
    List<WriteModel<PlayerData>> saveDatas = new List<WriteModel<PlayerData>>();
    public void Awake()
    {
        Instance = this;
        playerDataSaveTask = new IntervalTask(ConstConfigComponent.ConstConfig.PlayerDetectionInterval, Detection);
        TimeManagerComponent.Instance.Add(playerDataSaveTask);
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
    async void Detection()
    {
        if(curNode == null)
        {
            curNode = players.First;
        }
        LinkedListNode<Player> nextNode = null;
        int count = 0;
        int detectionCount = 0;
        List<UpdateDefinition<PlayerData>> updateParams = null;
        saveDatas.Clear();
        while (curNode != null)
        {
            nextNode = curNode.Next;
            detectionCount++;
            updateParams = curNode.Value.GetSaveData();
            if (updateParams != null && updateParams.Count > 0)
            {//需要有保存的数据
                var updates = Builders<PlayerData>.Update.Combine(updateParams);
                var filter = Builders<PlayerData>.Filter.Eq(f => f.id, curNode.Value.Data.id);
                saveDatas.Add(new UpdateOneModel<PlayerData>(filter, updates));
                count++;
            }
            if (curNode.Value.IsCanRemove())
            {//有离开的玩家
                players.Remove(curNode);
                plyaerDic.Remove(curNode.Value.Id);
                curNode.Value.Dispose();
                removePlayers.Add(curNode.Value.Id);
            }
            curNode = nextNode;
            if (count >= ConstConfigComponent.ConstConfig.PlayerDataSaveCount || detectionCount >= ConstConfigComponent.ConstConfig.PlayerDetectionCount)
            {
                break;
            }
        }
        if(saveDatas.Count > 0)
        {
            BulkWriteResult result = await DBOperateComponent.Instance.UpdatePlayerDatasAsync(saveDatas);
            if (result.MatchedCount != saveDatas.Count)
            {
                Console.WriteLine("警告：向数据库更新玩家数据时，有玩家id未查询到，导致数据未保存到数据库。");
            }
            saveDatas.Clear();
        }
        if(removePlayers.Count > 0)
        {
            SendRemove();
            removePlayers.Clear();
        }
    }
    public bool IsNeedSave()
    {
        foreach(Player player in players)
        {
            if(player.IsNeedSave())
            {
                return true;
            }
        }
        return false;
    }
    async void SendRemove()
    {
        //SS_RemovePlayerRequest req = new SS_RemovePlayerRequest();
        //req.playerids = removePlayers;
        //Session managerSession = NetInnerComponent.Instance.GetByAppID(ServerConfigComponent.Instance.ManagerAppId);
        //SS_RemovePlayerResponse respFromM = await managerSession.Call(req) as SS_RemovePlayerResponse;
    }
    public void Add(Player player)
    {
        if(plyaerDic.ContainsKey(player.Id))
        {
            return;
        }
        LinkedListNode<Player> node = players.AddLast(player);
        plyaerDic.Add(player.Id, node);
    }
    //private void Remove(long id)
    //{
    //    LinkedListNode<Player> node = null;
    //    if(plyaerDic.TryGetValue(id, out node))
    //    {
    //        players.Remove(node);
    //        plyaerDic.Remove(id);
    //    }
    //}
    public Player Find(long id)
    {
        LinkedListNode<Player> node = null;
        if (plyaerDic.TryGetValue(id, out node))
        {
            return node.Value;
        }
        return null;
    }
    public bool IsExist(long id)
    {
        return plyaerDic.ContainsKey(id);
    }
    public void OpenQuickSave()
    {
        playerDataSaveTask.SetInterval(ConstConfigComponent.ConstConfig.PlayerQuickDetectionInterval);
        ShowInfo();
    }
    async void ShowInfo()
    {
        while (true)
        {
            await Task.Delay(10000);
            LinkedListNode<Player> cur = players.First;
            int count = 0;
            while (cur != null)
            {
                if (cur.Value.IsNeedSave())
                {//需要有保存的数据
                    count++;
                }
                cur = cur.Next;
            }
            Console.WriteLine($"{count}个玩家数据正在保存...");
        }
    }
    public void SendMsgToAllPlayer(object msg)
    {
        Packet packet = Packet.Take();
        try
        {
            Session.FillContent(packet.Stream, msg, 0, Game.Instance.Appid);
            foreach (Player player in players)
            {
                player.SendMsg(packet.Stream);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
        finally
        {
            Packet.Back(packet);
        }
    }
    public void RemoveActivity(List<ActivityInfo> infos)
    {
        bool isSynchro = false;
        foreach (Player player in players)
        {
            isSynchro = false;
            for (int i = 0; i < infos.Count; i++)
            {
                if (player.CommonData.RemoveActivity(infos[i].id, false))
                    isSynchro = true;
            }
            if(isSynchro)
            {
                player.CommonData.SynchroActivity();
            }
        }
    }
}

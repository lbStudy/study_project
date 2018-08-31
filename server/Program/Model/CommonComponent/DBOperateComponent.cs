using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base;
using Data;

public class DBOperateComponent : Component, IAwake
{
    private const string playerTable = "player";        //对象在数据库的集合名称,类似表的名字
    private string databaseName = "mud_";               //用户信息数据库
    private const string roomrecordTable = "record";
    private string strconn;
    IMongoDatabase gameDatabase;
    MongoClient client;

    public static DBOperateComponent Instance;
    public void Awake()
    {
        Instance = this;
        databaseName += Game.Instance.BigAreaId;
        Connect();
        CreateDatabase();
        PlayerHandlerInit();
        IntervalTask interValTask = new IntervalTask(3000, Detection);
        TimeManagerComponent.Instance.Add(interValTask);
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
    public void Detection()
    {
        if (IsConnect())
        {
            if(!Game.Instance.IsFinishModule(InitModule.DBConnect))
                Game.Instance.SetInitFinishModule(InitModule.DBConnect);
        }
        else
        {
            Console.WriteLine("error: db disConnect.");
        }
    }
    public bool IsConnect()
    {
        return gameDatabase.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected;
    }
    public void Connect()
    {
        strconn = ServerConfigComponent.Instance.DBUrl + "/" + databaseName;
        client = new MongoClient(strconn);
        //client = new MongoClient(ServerConfigComponent.Instance.PlayerDBUrl);
        gameDatabase = client.GetDatabase(databaseName);
    }
    /// <summary>
    /// 根据数据库配置信息创建MongoDatabase对象，如果不指定配置信息，则从默认信息创建
    /// </summary>
    /// <param name="databaseName">数据库名称，默认空为local</param>
    /// <returns></returns>
    public void CreateDatabase()
    {
        try
        {
            IMongoCollection<PlayerData> playerCollections = gameDatabase.GetCollection<PlayerData>(playerTable);
            if (playerCollections == null)
            {
                gameDatabase.CreateCollection(playerTable);
                Console.WriteLine($"创建数据集：{databaseName}:{playerTable}");
            }

            //IMongoCollection<BattleRecord> recordCollections = gameDatabase.GetCollection<BattleRecord>(roomrecordTable);
            //if (recordCollections == null)
            //{
            //    gameDatabase.CreateCollection(roomrecordTable);
            //    Console.WriteLine($"创建数据集：{databaseName}:{roomrecordTable}");
            //}
            //if (playerCollections.Indexes.List().FirstOrDefault() == null)
            //{//创建索引
            //    playerCollections.Indexes.CreateOne(Builders<PlayerData>.IndexKeys.Ascending(x => x.detailData.name));
            //    Console.WriteLine("create player table index : basic.name");
            //}
            //else
            //{
            //    List<BsonDocument> indexes = playerCollections.Indexes.List().ToList();
            //    bool isExistNameIndex = false;
            //    foreach (BsonDocument val in indexes)
            //    {//查找指定索引是否存在
            //        if (val.GetValue("key").AsBsonDocument.Contains("name"))
            //        {
            //            isExistNameIndex = true;
            //            break;
            //        }
            //    }
            //    if (!isExistNameIndex)
            //    {//创建指定索引
            //        playerCollections.Indexes.CreateOne(Builders<PlayerData>.IndexKeys.Ascending(x => x.detailData.name));
            //    }
            //}
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    CountOptions playerCountOption = new CountOptions();
    FindOptions<PlayerData> playerFindOptions = new FindOptions<PlayerData>();
    FindOptions<PlayerData, long> playerIdFindOption = new FindOptions<PlayerData, long>();
    FindOptions<PlayerData, PlayerDetailData> playerBasicFindOption = new FindOptions<PlayerData, PlayerDetailData>();
    FindOptions<PlayerData, PlayerDetailData> playerBasicsFindOption = new FindOptions<PlayerData, PlayerDetailData>();
    FindOptions<PlayerData, long> playerUnionFindOption = new FindOptions<PlayerData, long>();
    //FindOptions<BattleRecord> recordFindOptions = new FindOptions<BattleRecord>();
    public void PlayerHandlerInit()
    {
        playerCountOption.Limit = 1;
        playerFindOptions.Limit = 1;
        //recordFindOptions.Limit = 1;


        playerIdFindOption.Projection = Builders<PlayerData>.Projection.Expression(x => x.id);
        playerIdFindOption.Limit = 1;

        playerBasicFindOption.Projection = Builders<PlayerData>.Projection.Expression(x => x.detailData);
        playerBasicFindOption.Limit = 1;

        playerBasicsFindOption.Projection = Builders<PlayerData>.Projection.Expression(x => x.detailData);

        //playerUnionFindOption.Projection = Builders<PlayerData>.Projection.Expression(x => x.detailData.unionID);
        //playerUnionFindOption.Limit = 1;
    }

    /// <summary>
    /// 异步查找玩家数据
    /// </summary>
    /// <param name="id"></param>
    /// <param name="serverID"></param>
    /// <returns></returns>
    public async Task<PlayerData> FindPlayerDataAsync(long id)
    {
        IMongoCollection<PlayerData> collections = gameDatabase.GetCollection<PlayerData>(playerTable);
        return await collections.FindAsync(s => s.id == id, playerFindOptions).Result.FirstOrDefaultAsync();
    }
    //public async Task<BattleRecord> FindBattleRecordAsync(long id)
    //{
    //    IMongoCollection<BattleRecord> collections = gameDatabase.GetCollection<BattleRecord>(roomrecordTable);
    //    return await collections.FindAsync(s => s.id == id, recordFindOptions).Result.FirstOrDefaultAsync();
    //}
    //public async void InsertRoomRecord(BattleRecord data)
    //{
    //    //使用 IsUpsert = true ，如果没有记录则写入
    //    IMongoCollection<BattleRecord> collections = gameDatabase.GetCollection<BattleRecord>(roomrecordTable);
    //    await collections.ReplaceOneAsync(s => s.id == data.id, data, new UpdateOptions() { IsUpsert = true });
    //}
    //public async void InsertRecord(long id, BattleInfo data)
    //{
    //    IMongoCollection<BattleRecord> collections = gameDatabase.GetCollection<BattleRecord>(roomrecordTable);
    //    await collections.UpdateOneAsync(s => s.id == id, Builders<BattleRecord>.Update.Push(x => x.battles, data));
    //}
    public async Task<BulkWriteResult> UpdatePlayerDatasAsync(List<WriteModel<PlayerData>> datas)
    {
        if (datas.Count == 0) return null;
        IMongoCollection<PlayerData> collections = gameDatabase.GetCollection<PlayerData>(playerTable);
        return await collections.BulkWriteAsync(datas);
    }
    //public async Task<BulkWriteResult> UpdateBattleRecordAsync(List<WriteModel<BattleRecord>> datas)
    //{
    //    if (datas.Count == 0) return null;
    //    IMongoCollection<BattleRecord> collections = gameDatabase.GetCollection<BattleRecord>(roomrecordTable);
    //    return await collections.BulkWriteAsync(datas);
    //}
    /// <summary>
    /// 插入玩家数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="serverID"></param>
    public async Task InsertPlayerDataAsync(PlayerData data)
    {
        IMongoCollection<PlayerData> collections = gameDatabase.GetCollection<PlayerData>(playerTable);
        await collections.InsertOneAsync(data);
    }
    public async Task<int> AddRoomCardAsync(long id, int count)
    {
        UpdateDefinition<PlayerData> updateDef = Builders<PlayerData>.Update.Inc(x => x.detailData.roomcard, count);
        IMongoCollection<PlayerData> collections = gameDatabase.GetCollection<PlayerData>(playerTable);
        UpdateResult result = await collections.UpdateOneAsync(s => s.id == id, updateDef);
        return (int)result.ModifiedCount;
    }
    public async Task<PlayerDetailData> FindDetaildata(long id)
    {
        IMongoCollection<PlayerData> collections = gameDatabase.GetCollection<PlayerData>(playerTable);
        FilterDefinition<PlayerData> filterDef = Builders<PlayerData>.Filter.Eq(x => x.id, id);
        FindOptions<PlayerData, PlayerDetailData> detailFindOption = new FindOptions<PlayerData, PlayerDetailData>();
        detailFindOption.Limit = 1;
        detailFindOption.Projection = Builders<PlayerData>.Projection.Expression(x => x.detailData);
        return await collections.FindAsync(filterDef, detailFindOption).Result.FirstOrDefaultAsync();
    }
    public async Task<int> FindRoomcardAsync(long id, int count)
    {
        IMongoCollection<PlayerData> collections = gameDatabase.GetCollection<PlayerData>(playerTable);
        //FindOptions<PlayerData, int> detailFindOption = new FindOptions<PlayerData, int>();
        //detailFindOption.Projection = Builders<PlayerData>.Projection.Expression(x => x.detailData.roomcard);
        FilterDefinition<PlayerData> filterDef = Builders<PlayerData>.Filter.Eq(x => x.id, id);
        UpdateDefinition<PlayerData> updateDef = Builders<PlayerData>.Update.Inc(x => x.detailData.roomcard, count);
        FindOneAndUpdateOptions<PlayerData, int> fuo = new FindOneAndUpdateOptions<PlayerData, int>();
        fuo.Projection = Builders<PlayerData>.Projection.Expression(x => x.detailData.roomcard);
        fuo.IsUpsert = false;
        return await collections.FindOneAndUpdateAsync(x => x.id == id, updateDef, fuo);
    }
    public async Task<List<PlayerDetailData>> FindPlayerDetailData(List<long> ids)
    {
        IMongoCollection<PlayerData> collections = gameDatabase.GetCollection<PlayerData>(playerTable);
        FilterDefinition<PlayerData> filterDef = Builders<PlayerData>.Filter.In(x => x.id, ids);
        FindOptions<PlayerData, PlayerDetailData> detailFindOption = new FindOptions<PlayerData, PlayerDetailData>();
        detailFindOption.Projection = Builders<PlayerData>.Projection.Expression(x => x.detailData);
        return await collections.FindSync(filterDef, detailFindOption).ToListAsync();
    }
    public async Task<List<PlayerDetailData>> FindPlayerDetailData()
    {
        IMongoCollection<PlayerData> collections = gameDatabase.GetCollection<PlayerData>(playerTable);
        FindOptions<PlayerData, PlayerDetailData> unionBasicDataFindOption = new FindOptions<PlayerData, PlayerDetailData>();
        unionBasicDataFindOption.Projection = Builders<PlayerData>.Projection.Expression(x => x.detailData);
        FilterDefinition<PlayerData> fileterDef = Builders<PlayerData>.Filter.Gt(x => x.id, 0);
        return await collections.FindAsync(fileterDef, unionBasicDataFindOption).Result.ToListAsync();
    }
    //public async Task<DeleteResult> RefreshRecord(long recordRemoveTime)
    //{
    //    IMongoCollection<BattleRecord> collections = gameDatabase.GetCollection<BattleRecord>(roomrecordTable);
    //    FilterDefinition<BattleRecord> filter = Builders<BattleRecord>.Filter.Lt(x => x.createTime, recordRemoveTime);
    //    return await collections.DeleteManyAsync(filter);
    //}
}
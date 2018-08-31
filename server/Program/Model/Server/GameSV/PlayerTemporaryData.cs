//临时数据
public class PlayerTemporaryData
{
    private int battleAppid = 0;
    public int BattleAppid { get { return battleAppid; } }
    public string checkCode;
    public long offlineTime;
    public string iconUrl;
    public int sex;
    public int costRoomCard;
    public long preBattleid;
    public int firstLoginDay;

    Player player;

    public PlayerTemporaryData(Player player)
    {
        this.player = player;
    }
    public void SetBattleAppid(int appid)
    {
        battleAppid = appid;
        //TranspondComponent.instance.SetBattleAppid(player.Id, appid);
    }
}

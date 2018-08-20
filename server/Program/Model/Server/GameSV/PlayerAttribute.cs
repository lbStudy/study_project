namespace Data
{
    public abstract class PlayerAttribute
    {
        //加
        public virtual void AddValue(Player player, PlayerDetailData detail, long val, LogAction logAction) { }
        //减
        public virtual void SubValue(Player player, PlayerDetailData detail, long val, LogAction logAction) { }
        //设置
        public virtual void SetValue(Player player, PlayerDetailData detail, long val, LogAction logAction) { }
        //返回值
        public abstract long GetValue(Player player);
    }
}
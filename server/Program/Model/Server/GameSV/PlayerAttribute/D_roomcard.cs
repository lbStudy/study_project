using System;
using Data;
using Base;

public class D_roomcard : PlayerAttribute
{
    public const int maxVal = 2147483647;
	//¼Ó
    public override void AddValue(Player player, PlayerDetailData detail, long val, LogAction logAction)
	{
        if(logAction != LogAction.None)
            Log.Info(LogDatabase.fengyuncard, ServerConfigComponent.Instance.Projectid, player.Id, 1, player.CommonData.name, LogType.roomcard.ToString(), logAction.ToString(), detail.roomcard, (int)val);
        detail.roomcard += (int)val;
		if (detail.roomcard > maxVal)
		{
			detail.roomcard = maxVal;
		}
		else if (detail.roomcard < 0)
		{
			detail.roomcard = 0;
		}
	}
	//¼õ
    public override void SubValue(Player player, PlayerDetailData detail, long val, LogAction logAction)
	{
        if (logAction != LogAction.None)
            Log.Info(LogDatabase.fengyuncard, ServerConfigComponent.Instance.Projectid, player.Id, 1, player.CommonData.name, LogType.roomcard.ToString(), logAction.ToString(), detail.roomcard, -((int)val));
        detail.roomcard -= (int)val;
		if (detail.roomcard > maxVal)
		{
			detail.roomcard = maxVal;
		}
		else if (detail.roomcard < 0)
		{
			detail.roomcard = 0;
		}
	}
	//ÉèÖÃ
    public override void SetValue(Player player, PlayerDetailData detail, long val, LogAction logAction)
	{
		detail.roomcard = (int)val;
		if (detail.roomcard > maxVal)
		{
			detail.roomcard = maxVal;
		}
		else if (detail.roomcard < 0)
		{
			detail.roomcard = 0;
		}
	}
	
    public override long GetValue(Player player)
	{
	    return player.CommonData.Roomcard;
	}
}
